using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MongoDB.Driver;
using Sdl.Core.Globalization.NumberMetadata;
using TMX_Lib.TmxFormat;
using TMX_Lib.Utils;

namespace TMX_Lib.Db
{
    // example:
    // TmxMongoDb db = new TmxMongoDb("localhost:27017", "mydb");
    public class TmxMongoDb
    {
	    // cap the results, make the searches faster
	    //
	    // the idea: we can end up with lots of results from the database - just process the first batch, since some results could be wrong
	    // but we don't want to end up processing too many possible results either, since that could put a strain on both the db, and on us
		//
		// this is especially true on fuzzy searches
	    private const int MAX_RESULTS = 256;

        // FIXME test with Atlas credentials as well
		private string _url;
        private string _databaseName;
        private bool _connected;
        private bool _initialized;

        private MongoClient _client;
        private IMongoDatabase _database;

        private IMongoCollection<TmxMeta> _metas;
        private IMongoCollection<TmxLanguage> _languages;
        private IMongoCollection<TmxTranslationUnit> _translationUnits;
        private IMongoCollection<TmxText> _texts;

        private Task _importTask;
        private CancellationTokenSource _importTokenSource = new CancellationTokenSource();
		// if null - no import running
		// true or false - import is either running or complete
        private bool? _importComplete = null;
        private TmxParser _parser;

        private CultureDictionary _cultures = new CultureDictionary();


		public TmxMongoDb(string url, string databaseName)
        {
            _url = url;
            var notAllowedChars = " \t\r\n:.()_";
            _databaseName = new string(databaseName.Where(ch => !notAllowedChars.Contains(ch)).ToArray());
            Connect();
        }

		private bool IsLocalConnection() => _url.StartsWith("mongodb://");

        private void Connect()
        {
            try
            {
	            if (!_url.StartsWith("mongo"))
		            _url = $"mongodb://{_url}";

				var url = new MongoUrl(_url);
                _client = new MongoClient(url);
                _database = _client.GetDatabase(_databaseName);
                _connected = true;
				// if local -> fast connection
                if (IsLocalConnection())
                {
	                Initialize();
	                lock (this)
						_initialized = true;
                }
			}
            catch (TimeoutException e)
            {
                throw new TmxException($"Timeout connecting to {_databaseName}");
            }
            catch (Exception e)
            {
                throw new TmxException($"Can't connect to {_databaseName} database." , e);
            }
        }

        public async Task InitAsync()
        {
	        try
	        {
				lock(this)
					if (_initialized)
						return;

		        await Task.Run(Initialize);
	        }
			finally
	        {
		        lock (this)
			        _initialized = true;
	        }
        }

		// this connects to the server, be it local or remote - which can block the current thread
        private void Initialize()
        {
            if (!_connected)
                return;
            try
            {
	            var names = _database.ListCollectionNames().ToList();
	            if (!names.Any())
	            {
		            _database.CreateCollection("languages");
		            _database.CreateCollection("meta");
		            _database.CreateCollection("texts");
		            _database.CreateCollection("translation_units");
	            }

	            _metas = _database.GetCollection<TmxMeta>("meta");
	            _languages = _database.GetCollection<TmxLanguage>("languages");
	            _translationUnits = _database.GetCollection<TmxTranslationUnit>("translation_units");
	            _texts = _database.GetCollection<TmxText>("texts");
            }
            catch (TimeoutException te)
            {
	            throw new TmxException($"Timeout connecting to database {_url}", te);
            }
			catch (Exception e)
            {
                throw new TmxException($"Can't connect to database {_url}", e);
            }
        }

        public async Task ClearAsync()
        {
            try
            {
                if (HasAnyData())
                {
                    // remove everything, we'll restart the import from scratch
                    await _metas.DeleteManyAsync(Builders<TmxMeta>.Filter.Empty);
                    await _languages.DeleteManyAsync(Builders<TmxLanguage>.Filter.Empty);
                    await _translationUnits.DeleteManyAsync(Builders<TmxTranslationUnit>.Filter.Empty);
                    await _texts.DeleteManyAsync(Builders<TmxText>.Filter.Empty);
                }
            }
            catch (Exception e)
            {
                throw new TmxException("Can't initialize db", e);
            }
        }

        public bool HasAnyData()
        {
            try
            {
                return _metas.EstimatedDocumentCount() > 0;
            }
            catch (Exception e)
            {
                throw new TmxException("Error accessing db - HasAnyData", e);
            }
        }

        public async Task<string> ImportedFileNameAsync()
        {
	        try
	        {
		        var result = await _metas.FindAsync(Builders<TmxMeta>.Filter.Where(m => m.Type == "FileName"));
		        var metas = new List<string>();
		        await result.ForEachAsync(l => metas.Add(l.Value));
		        return metas.Count > 0 ? metas[0] : "";
	        }
			catch (Exception e)
	        {
		        throw new TmxException("Can't get imported file name", e);
	        }
		}

        public async Task<IReadOnlyList<string>> GetAllLanguagesAsync()
        {
	        try
	        {
		        var result = await _languages.FindAsync(Builders<TmxLanguage>.Filter.Empty);
		        var languages = new List<string>();
		        await result.ForEachAsync(l => languages.Add(l.Language));
		        return languages;
	        }
	        catch (Exception e)
	        {
		        throw new TmxException("Can't get all languages", e);
	        }
        }

        private async Task CreateIndexesAsync()
        {
            try
            {
	            var list = await _translationUnits.Indexes.ListAsync();
	            int count = 0;
	            await list.ForEachAsync(c => ++count);
	            if (count > 1)
		            return;

				var indexTU = Builders<TmxTranslationUnit>.IndexKeys.Ascending(i => i.ID);
	            await _translationUnits.Indexes.CreateOneAsync(new CreateIndexModel<TmxTranslationUnit>(indexTU));

				// at most one text index allowed
	            var indexText = Builders<TmxText>.IndexKeys.Text(i => i.LocaseText).Ascending(i => i.NormalizedLanguage);
	            await _texts.Indexes.CreateOneAsync(new CreateIndexModel<TmxText>(indexText));

				// IMPORTANT: first, ID, then language, since when looking for a translation in a specific language, I already know the TU ID
				var indexTextByLangTU = Builders<TmxText>.IndexKeys.Ascending(i => i.TranslationUnitID).Ascending(i => i.NormalizedLanguage);
	            await _texts.Indexes.CreateOneAsync(new CreateIndexModel<TmxText>(indexTextByLangTU));
            }
			catch (Exception e)
            {
                throw new TmxException("Can't create indexes", e);
            }
        }
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// Start of FIND

		private string XmlUnformatText(string formattedText)
		{
			XmlDocument doc = new XmlDocument();
			formattedText = formattedText.Replace("<", "&lt;").Replace(">", "&gt;");
			doc.LoadXml($"<tmx>{formattedText}</tmx>");
			var unformatted = doc.SelectSingleNode("tmx").InnerText;
			return unformatted;
		}

		// if not found, returns null
		private async Task<TmxSegment> TryGetSegment(TmxText text, string targetLanguage)
		{
			var textFilter = Builders<TmxText>.Filter.Where(f => f.TranslationUnitID == text.TranslationUnitID && f.NormalizedLanguage == targetLanguage );
			var textCursor = await _texts.FindAsync(textFilter, new FindOptions<TmxText>() { Limit = 1 });
			var targetTexts = new List<TmxText>();
			await textCursor.ForEachAsync(t => targetTexts.Add(t));
			if (targetTexts.Count == 0)
				// we don't have a translation
				return null;

			var tuFilter = Builders<TmxTranslationUnit>.Filter.Where(f => f.ID == text.TranslationUnitID);
			var tuCursor = await _translationUnits.FindAsync(tuFilter, new FindOptions<TmxTranslationUnit> { Limit = 1 });
			var tus = new List<TmxTranslationUnit>();
			await tuCursor.ForEachAsync(t => tus.Add(t));
			if (tus.Count < 1)
				throw new TmxException($"Can't find Translation Unit with ID {text.TranslationUnitID}");

			var segment = new TmxSegment
			{
				DbTU = tus[0], 
				DbSourceText = text, 
				DbTargetText = targetTexts[0],
				SourceText = XmlUnformatText(text.FormattedText),
				TargetText = XmlUnformatText(targetTexts[0].FormattedText),
			};

			return segment;
		}

		// this just performs the search and returns the results -- does NOT perform any other analyses, like, compare score and such
		public async Task<IReadOnlyList<TmxSegment>> ExactSearch(string text, string sourceLanguage, string targetLanguage)
		{
			text = Util.TextToDbText(text, _cultures.Culture(sourceLanguage));
			sourceLanguage = Util.NormalizeLanguage(sourceLanguage);
			targetLanguage = Util.NormalizeLanguage(targetLanguage);
			// old:
			//var filter = Builders<TmxText>.Filter.Where(f => f.NormalizedLanguage == sourceLanguage && f.LocaseText == text);

			// surround text in quotes -> so that we perform an exact search
			text = $"\"{text.Replace("\"", "\\\"")}\"" ;
			var filter = Builders<TmxText>.Filter.Text(text , new TextSearchOptions { CaseSensitive = false, DiacriticSensitive = false,});
			var cursor = await _texts.FindAsync(filter, new FindOptions<TmxText>() { Limit = MAX_RESULTS });
			var texts = new List<TmxText>();
			await cursor.ForEachAsync(t =>
			{
				if (t.NormalizedLanguage == sourceLanguage)
					texts.Add(t);
			});
			var segments = new List<TmxSegment>();
			foreach (var t in texts)
			{
				var segment = await TryGetSegment(t, targetLanguage);
				if (segment != null)
					segments.Add(segment);
			}
			return segments;
		}
		// this just performs the search and returns the results -- does NOT perform any other analyses, like, compare score and such
		public async Task<IReadOnlyList<TmxSegment>> FuzzySearch(string text, string sourceLanguage, string targetLanguage)
		{
			text = Util.TextToDbText(text, _cultures.Culture(sourceLanguage));
			sourceLanguage = Util.NormalizeLanguage(sourceLanguage);
			targetLanguage = Util.NormalizeLanguage(targetLanguage);
			var filter = Builders<TmxText>.Filter.Text(text, new TextSearchOptions { CaseSensitive = false, DiacriticSensitive = false, });

			// FIXME use projection, to find out the textScore, and sort by it.

			var cursor = await _texts.FindAsync(filter, new FindOptions<TmxText>() { Limit = MAX_RESULTS, });
			var texts = new List<TmxText>();
			await cursor.ForEachAsync(t =>
			{
				if (t.NormalizedLanguage == sourceLanguage)
					texts.Add(t);
			});
			var segments = new List<TmxSegment>();
			foreach (var t in texts)
			{
				var segment = await TryGetSegment(t, targetLanguage);
				if (segment != null)
					segments.Add(segment);
			}
			return segments;
		}

		// End of FIND
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// Start of ADDs

		public async Task AddMetasAsync(IEnumerable<TmxMeta> metas, CancellationToken token = default(CancellationToken))
        {
            try
            {
                await _metas.InsertManyAsync(metas, null, token);
            }
            catch (Exception e)
            {
                throw new TmxException($"Can't add Metas", e);
            }
        }
        public async Task AddLanguagesAsync(IEnumerable<TmxLanguage> languages, CancellationToken token = default(CancellationToken))
        {
            try
            {
                await _languages.InsertManyAsync(languages, null, token);
            }
            catch (Exception e)
            {
                throw new TmxException($"Can't add Languages", e);
            }
        }
        public async Task AddTextsAsync(IEnumerable<TmxText> texts, CancellationToken token = default(CancellationToken))
        {
            try
            {
                await _texts.InsertManyAsync(texts, null, token);
            }
            catch (Exception e)
            {
                throw new TmxException($"Can't add Texts", e);
            }
        }
        public async Task AddTranslationUnitsAsync(IEnumerable<TmxTranslationUnit> translationUnits, CancellationToken token = default(CancellationToken))
        {
            try
            {
                await _translationUnits.InsertManyAsync(translationUnits, null, token);
            }
            catch (Exception e)
            {
                throw new TmxException($"Can't add TranslationUnits", e);
            }
        }
        // End of ADDs
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// Start of IMPORT

		private static (IReadOnlyList<Db.TmxText> texts, Db.TmxTranslationUnit tu) TmxTranslationUnitToMongo(TmxFormat.TmxTranslationUnit tu, ulong id)
		{
			Db.TmxTranslationUnit dbTU = new Db.TmxTranslationUnit
			{
				ID = id,
				CreationAuthor = tu.CreationAuthor,
				CreationDate = tu.CreationTime,
				ChangeAuthor = tu.ChangeAuthor,
				ChangeDate = tu.ChangeTime,
				XmlProperties = tu.XmlProperties,
				TuAttributes = tu.TuAttributes,
			};

			var texts = tu.Texts.Select(t => new Db.TmxText
			{
				TranslationUnitID = id,
				NormalizedLanguage = Util.NormalizeLanguage(t.Language) ,
				LocaseText = t.Text,
				FormattedText = t.FormattedText,
			}).ToList();

			return (texts, dbTU);
		}

		// when i have a new database connection - this is the first question - have we already imported everything?
		public async Task<bool> HasImportBeenDoneBeforeAsync()
        {
	        try
	        {
		        var list = await _translationUnits.Indexes.ListAsync();
		        int count = 0;
		        await list.ForEachAsync(c => ++count);

		        // the idea - create indexes is the last step
		        // if translation units table has an extra index, we've done it
		        return count > 1;
	        }
	        catch (Exception e)
	        {
		        throw new TmxException("Can't check if import is complete", e);
	        }
        }

        public bool IsImportInProgress()
        {
	        lock (this)
		        return _importComplete != null;
        }

        public bool IsImportComplete()
        {
	        lock (this)
		        return _importComplete == true;
        }

        public double ImportProgress()
        {
	        lock (this)
	        {
		        if (_importComplete == true)
			        return 1;
		        var progress = _parser?.Progress() ?? 0d;
				// the idea - once the parsing is complete, I'm creating the indexes. 
				// I assume that will take some time as well
		        return Math.Min(progress * 0.95, 0.95);
	        }
        }

        public void CancelImport()
        {
	        lock (this)
	        {
		        if (_importTask != null)
			        return;
	        }

	        _importTokenSource.Cancel();
        }

        public async Task ImportToDbAsync(string fileName)
        {
	        Task import;
	        lock (this)
	        {
		        if (_importComplete != null)
			        throw new TmxException("Import already in progress");
		        if (_importTask == null)
			        _importTask = Task.Run(() => ImportToDbAsyncImpl(_parser = new TmxParser(fileName), _importTokenSource.Token));

		        import = _importTask;
		        _importComplete = false;
	        }

	        await import;
        }


		private async Task ImportToDbAsyncImpl(TmxParser parser, CancellationToken token)
        {
	        Stopwatch watch;
	        try
	        {
		        await ClearAsync();
		        watch = Stopwatch.StartNew();

		        ulong id = 1;
		        while (true)
		        {
			        if (token.IsCancellationRequested)
				        break;
			        var TUs = parser.TryReadNextTUs();
			        if (TUs == null)
				        break;

			        var dbTexts = new List<TmxText>();
			        var dbTUs = new List<TmxTranslationUnit>();
			        foreach (var tu in TUs)
			        {
				        var dbTU = TmxTranslationUnitToMongo(tu, id++);
				        dbTexts.AddRange(dbTU.texts);
				        dbTUs.Add(dbTU.tu);
			        }

			        await AddTranslationUnitsAsync(dbTUs, token);
			        await AddTextsAsync(dbTexts, token);
		        }

		        // languages are known only after everything has been imported
		        var languages = parser.Languages().Select(l => new TmxLanguage { Language = l }).ToList();
		        await AddLanguagesAsync(languages, token);

				// the idea -> add them at the end, easily know if the import went to the end or not
		        await AddMetasAsync(new[]
		        {
			        new TmxMeta { Type = "Header", Value = parser.Header.Xml,},
					// Version:
					// 1 - initial version
					// ... - increase this on each breaking change
			        new TmxMeta { Type = "Version", Value = "1",},
			        new TmxMeta { Type = "Source Language", Value = parser.Header.SourceLanguage,},
			        new TmxMeta { Type = "Target Language", Value = parser.Header.TargetLanguage,},
			        new TmxMeta { Type = "Domains", Value = string.Join(", ", parser.Header.Domains),},
			        new TmxMeta { Type = "Creation Date", Value = parser.Header.CreationDate?.ToLongDateString() ?? "unknown",},
			        new TmxMeta { Type = "Author", Value = parser.Header.Author,},
			        new TmxMeta { Type = "FileName", Value = Path.GetFileName(parser.FileName)},
			        new TmxMeta { Type = "FullFileName", Value = parser.FileName},
		        }, token);

				// best practice - create indexes after everything has been imported
				if (!token.IsCancellationRequested)
					await CreateIndexesAsync();
	        }
	        catch (Exception e)
	        {
		        throw new TmxException("Import to db failed", e);
	        }

	        lock (this)
	        {
		        _importComplete = true;
		        _importTask = null;
		        _importTokenSource = new CancellationTokenSource();
	        }
	        Debug.WriteLine($"import complete, took {watch.ElapsedMilliseconds} ms");
		}

		// End of IMPORT
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
}
