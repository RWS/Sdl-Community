using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TMX_Lib.TmxFormat;

namespace TMX_Lib.Db
{
    // example:
    // TmxMongoDb db = new TmxMongoDb("localhost:27017", "mydb");
    public class TmxMongoDb
    {
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



		public TmxMongoDb(string url, string databaseName)
        {
            _url = url;
            _databaseName = databaseName;
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
                throw new TmxException($"Can't connect to {_databaseName}", e);
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

	            var indexTextByLangText = Builders<TmxText>.IndexKeys.Ascending(i => i.Language).Ascending(i => i.Text);
	            await _texts.Indexes.CreateOneAsync(new CreateIndexModel<TmxText>(indexTextByLangText));
	            var indexTextByLangTU = Builders<TmxText>.IndexKeys.Ascending(i => i.Language).Ascending(i => i.TranslationUnitID);
	            await _texts.Indexes.CreateOneAsync(new CreateIndexModel<TmxText>(indexTextByLangTU));
            }
			catch (Exception e)
            {
                throw new TmxException("Can't create indexes", e);
            }
        }


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
		        Language = t.Language,
		        Text = t.Text,
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
		        return Math.Max(progress * 0.95, 0.95);
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
		        await AddMetasAsync(new[]
		        {
			        new TmxMeta { Type = "Header", Value = parser.Header.Xml,},
			        new TmxMeta { Type = "Source Language", Value = parser.Header.SourceLanguage,},
			        new TmxMeta { Type = "Target Language", Value = parser.Header.TargetLanguage,},
			        new TmxMeta { Type = "Domains", Value = string.Join(", ", parser.Header.Domains),},
			        new TmxMeta { Type = "Creation Date", Value = parser.Header.CreationDate?.ToLongDateString() ?? "unknown",},
			        new TmxMeta { Type = "Author", Value = parser.Header.Author,},
		        }, token);

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
    }
}
