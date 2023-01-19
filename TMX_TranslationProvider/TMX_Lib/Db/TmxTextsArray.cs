using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using NLog;
using NLog.Fluent;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using TMX_Lib.Utils;

namespace TMX_Lib.Db
{
	public class TmxTextsArray
	{
		private static readonly Logger log = NLog.LogManager.GetCurrentClassLogger();

		// cap the results, make the searches faster
		//
		// the idea: we can end up with lots of results from the database - just process the first batch, since some results could be wrong
		// but we don't want to end up processing too many possible results either, since that could put a strain on both the db, and on us
		//
		// this is especially true on fuzzy searches
		private const int MAX_RESULTS = 256;

		private const string TABLE_PREFIX = "text-";

		private MongoClient _client;

		private IMongoDatabase _database;
		private int _entriesPerTable;

		private IMongoCollection<TmxMeta> _metas;

		// Key: language.<tableindex>
		private Dictionary< string, IMongoCollection<TmxText>> _texts = new Dictionary<string, IMongoCollection<TmxText>>();

		public TmxTextsArray(MongoClient client, IMongoDatabase database, int entriesPerTable)
		{
			_client = client;
			_database = database;
			_entriesPerTable = entriesPerTable;
			Load();
		}

		private void Load()
		{
			_metas = _database.GetCollection<TmxMeta>("meta");
			var names = _database.ListCollectionNames().ToList();
			foreach (var name in names.Where(n => n.StartsWith(TABLE_PREFIX)))
				_texts.Add( name.Substring(TABLE_PREFIX.Length), _database.GetCollection<TmxText>(name) );
		}
		public async Task ClearAsync()
		{
			foreach (var textTable in _texts.Values)
				await textTable.DeleteManyAsync(Builders<TmxText>.Filter.Empty);
		}

		public async Task CreateIndexesAsync()
		{
			var filter = Builders<TmxMeta>.Filter.Empty;
			var cursor = await _metas.FindAsync(filter);
			HashSet<string> indexedTables = new HashSet<string>();
			await cursor.ForEachAsync(m =>
			{
				if (m.Value == "language-table")
					indexedTables.Add(m.Type);
			});

			Dictionary<string, IMongoCollection<TmxText>> textsToIndexNow;
			lock (this)
				textsToIndexNow = _texts.Where(tt => !indexedTables.Contains(tt.Key)).ToDictionary(kv =>kv.Key, kv => kv.Value);

			List<Task> tasks = new List<Task>();
			foreach (var textTable in textsToIndexNow) {
				var copy = textTable;
				var task = Task.Run(async() => await CreateIndexesAsyncForTable(copy.Value, copy.Key));
				tasks.Add(task);
			}
			await Task.WhenAll(tasks);
		}
		private async Task CreateIndexesAsyncForTable(IMongoCollection<TmxText> textTable, string name)
		{
			// at most one text index allowed
			var indexText = Builders<TmxText>.IndexKeys.Text(i => i.LocaseText).Ascending(i => i.NormalizedLanguage);
			await textTable.Indexes.CreateOneAsync(new CreateIndexModel<TmxText>(indexText));

			// IMPORTANT: first, ID, then language, since when looking for a translation in a specific language, I already know the TU ID
			var indexTextByLangTU = Builders<TmxText>.IndexKeys.Ascending(i => i.TranslationUnitID).Ascending(i => i.NormalizedLanguage);
			await textTable.Indexes.CreateOneAsync(new CreateIndexModel<TmxText>(indexTextByLangTU));

			await _metas.InsertOneAsync(new TmxMeta { 
				Type = name, Value = "language-table",
			}, null);
		}

		private string LanguageKey(ulong translationID, string language) {
			var languageSuffix = translationID / (ulong)_entriesPerTable;
			var languageKey = $"{language}.{languageSuffix}";
			return languageKey;
		}

		private IMongoCollection<TmxText> GetTextTable(ulong translationID, string language)
		{
			var languageKey = LanguageKey(translationID, language);
			lock(this)
				if (_texts.TryGetValue(languageKey, out var table))
					return table;
			return null;
		}
		private IReadOnlyList<IMongoCollection<TmxText>> GetTextTables(string language)
		{
			lock (this)
				return _texts.Where(l => l.Key.StartsWith(language + ".")).Select(l => l.Value).ToList();
		}
		private IReadOnlyList<IMongoCollection<TmxText>> GetTextTablesForTU(ulong translationID)
		{
			var languageSuffixIndex = translationID % (ulong)_entriesPerTable;
			var languageSuffix = $".{languageSuffixIndex}";
			lock (this)
				return _texts.Where(l => l.Key.EndsWith(languageSuffix)).Select(l => l.Value).ToList();
		}


		public async Task<TmxText> TryFindTranslation(ulong translationID, string targetLanguage)
		{
			var textTable = GetTextTable(translationID, targetLanguage);
			if (textTable == null)
				return null;

			var textFilter = Builders<TmxText>.Filter.Where(f => f.TranslationUnitID == translationID && f.NormalizedLanguage == targetLanguage);
			var textCursor = await textTable.FindAsync(textFilter, new FindOptions<TmxText>() { Limit = 1 });
			var targetTexts = new List<TmxText>();
			await textCursor.ForEachAsync(t => targetTexts.Add(t));
			if (targetTexts.Count == 0)
				// we don't have a translation
				return null;
			return targetTexts[0];
		}

		// this just performs the search and returns the results -- does NOT perform any other analyses, like, compare score and such
		public async Task<IReadOnlyList<TmxText>> ExactSearch(string text, string sourceLanguage)
		{
			sourceLanguage = Util.NormalizeLanguage(sourceLanguage);
			// old:
			//var filter = Builders<TmxText>.Filter.Where(f => f.NormalizedLanguage == sourceLanguage && f.LocaseText == text);

			// surround text in quotes -> so that we perform an exact search
			text = $"\"{text.Replace("\"", "\\\"")}\"";
			var filter = Builders<TmxText>.Filter.Text(text, new TextSearchOptions { CaseSensitive = false, DiacriticSensitive = false, });
			log.Debug($"START Exact search for {text} {sourceLanguage} ");
			var textTables = GetTextTables(sourceLanguage);
			var tasks = new List<Task>();
			var texts = new List<TmxText>();
			foreach (var tt in textTables)
			{
				var copy = tt;
				var task = Task.Run(async () => {
					var cursor = await copy.FindAsync(filter, new FindOptions<TmxText>() { Limit = MAX_RESULTS });
					var taskTexts = new List<TmxText>();
					await cursor.ForEachAsync(t =>
					{
						if (t.NormalizedLanguage == sourceLanguage)
							taskTexts.Add(t);
					});
					lock (this)
						texts.AddRange(taskTexts);
				});
				tasks.Add(task);
			}
			await Task.WhenAll(tasks);

			log.Debug($"END Exact search for {text} {sourceLanguage} ");
			return texts;
		}

		public async Task<IReadOnlyList<TmxText>> FuzzySearch(string text, string sourceLanguage) {
			var filter = Builders<TmxText>.Filter.Text(text, new TextSearchOptions { CaseSensitive = false, DiacriticSensitive = false, });

			var projection = Builders<TmxText>.Projection.MetaTextScore("Score")
				.Include(p => p.LocaseText).Include(p => p.NormalizedLanguage).Include(p => p.TranslationUnitID).Include(p => p.FormattedText);
			var sort = Builders<TmxText>.Sort.MetaTextScore("Score");

			log.Debug($"START Fuzzy search for {text} {sourceLanguage} ");
			var textTables = GetTextTables(sourceLanguage);
			var tasks = new List<Task>();
			var texts = new List<TmxText>();
			foreach (var tt in textTables)
			{
				var copy = tt;
				var task = Task.Run(async () => {
					// note: this works, but would not expose the sort, so we would not know the score differences between results
					//
					//var cursor = await _texts.Aggregate()
					//	.Match(filter)
					//	.Sort(Builders<TmxText>.Sort.MetaTextScore("Score"))
					//	.Limit(MAX_RESULTS)
					//	.ToListAsync();
					var cursor = await copy
						.Aggregate()
						.Match(filter)
						.Sort(sort)
						.Limit(MAX_RESULTS)
						.Project(projection)
						.ToListAsync();
					var taskTexts = new List<TmxText>();
					foreach (var p in cursor)
					{
						var t = BsonSerializer.Deserialize<TmxText>(p);
						if (t.NormalizedLanguage == sourceLanguage)
							taskTexts.Add(t);
					}
					lock (this)
						texts.AddRange(taskTexts);
				});
				tasks.Add(task);
			}
			await Task.WhenAll(tasks);

			texts = texts.OrderByDescending(t => t.Score).ToList();
			log.Debug($"END Fuzzy search for {text} {sourceLanguage} ");

			return texts;
		}

		public async Task<IReadOnlyList<TmxText>> ConcordanceSearch(string text, string sourceLanguage)
		{
			var filter = Builders<TmxText>.Filter.Text(text, new TextSearchOptions { CaseSensitive = false, DiacriticSensitive = false, });
			log.Debug($"START Concordance search for {text} {sourceLanguage} ");
			var textTables = GetTextTables(sourceLanguage);
			var tasks = new List<Task>();
			var texts = new List<TmxText>();
			foreach (var tt in textTables)
			{
				var copy = tt;
				var task = Task.Run(async () => {
					var cursor = await copy.FindAsync(filter, new FindOptions<TmxText>() { Limit = MAX_RESULTS });
					var taskTexts = new List<TmxText>();
					await cursor.ForEachAsync(t =>
					{
						if (t.NormalizedLanguage == sourceLanguage)
							taskTexts.Add(t);
					});
					lock (this)
						texts.AddRange(taskTexts);
				});
				tasks.Add(task);
			}
			await Task.WhenAll(tasks);

			log.Debug($"END Concordance search for {text} {sourceLanguage} ");
			return texts;
		}

		public async Task<IReadOnlyList<TmxText>> FindTextsAsync(ulong id)
		{
			var filter = Builders<TmxText>.Filter.Where(tu => tu.TranslationUnitID == id);
			var textTables = GetTextTablesForTU(id);

			var tasks = new List<Task>();
			var texts = new List<TmxText>();
			foreach (var tt in textTables)
			{
				var copy = tt;
				var task = Task.Run(async () => {
					var cursor = await copy.FindAsync(filter);
					var taskTexts = new List<TmxText>();
					await cursor.ForEachAsync(t =>
					{
						taskTexts.Add(t);
					});
					lock (this)
						texts.AddRange(taskTexts);
				});
				tasks.Add(task);
			}
			await Task.WhenAll(tasks);
			return texts;
		}

		private async Task<IMongoCollection<TmxText>> GetOrInsertTextTableAsync(string languageKey) {
			lock (this) {
				if (_texts.TryGetValue(languageKey, out var table))
					return table;
			}
			try
			{
				var tableName = TABLE_PREFIX + languageKey;
				await _database.CreateCollectionAsync(tableName);
				var table = _database.GetCollection<TmxText>(tableName);
				lock (this)
					if (!_texts.ContainsKey(languageKey))
						_texts.Add(languageKey, table);
				return table;
			}
			catch { 
				// assume maybe another thread created it?
			}

			// here, in case another thread created it, just get it
			try
			{
				var tableName = TABLE_PREFIX + languageKey;
				var table = _database.GetCollection<TmxText>(tableName);
				lock (this)
					if (!_texts.ContainsKey(languageKey))
						_texts.Add(languageKey, table);
				return table;
			}
			catch(Exception e)
			{
				throw new TmxException($"Can't create text database {languageKey}", e);
			}
		}


		public async Task AddTextsAsync(IEnumerable<TmxText> texts, CancellationToken token = default(CancellationToken))
		{
			try
			{
				Dictionary<string, List<TmxText>> textsByTable = new Dictionary<string, List<TmxText>>();
				foreach (var text in texts) {
					var key = LanguageKey(text.TranslationUnitID, text.NormalizedLanguage);
					if (textsByTable.TryGetValue(key, out var list))
						list.Add(text);
					else
						textsByTable.Add(key, new List<TmxText> { text });
				}
				var tasks = new List<Task>();
				foreach (var tt in textsByTable) {
					var copy = tt;
					var task = Task.Run(async() => {
						var table = await GetOrInsertTextTableAsync(copy.Key);
						await table.InsertManyAsync(texts, null, token);
					});
					tasks.Add(task);
				}
				await Task.WhenAll(tasks);
			}
			catch (Exception e)
			{
				throw new TmxException($"Can't add Texts", e);
			}
		}


	}
}
