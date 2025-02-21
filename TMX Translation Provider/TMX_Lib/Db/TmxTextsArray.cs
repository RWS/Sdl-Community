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
	internal class TmxTextsArray
	{
		private static readonly Logger log = NLog.LogManager.GetCurrentClassLogger();

		// cap the results, make the searches faster
		//
		// the idea: we can end up with lots of results from the database - just process the first batch, since some results could be wrong
		// but we don't want to end up processing too many possible results either, since that could put a strain on both the db, and on us
		//
		// this is especially true on fuzzy searches
		//
		// IMPORTANT: I played with certain less values (16, 24 -- they almost made no difference)
		private const int MAX_RESULTS_PER_TASK = 32;
		private const int MAX_RESULTS = 128;

		private const string TABLE_PREFIX = "text-";

		private IMongoDatabase _database;
		public int EntriesPerTable = TmxMongoDb.DEFAULT_ENTRIES_PER_TEXT_TABLE;

		private IMongoCollection<TmxMeta> _metas;

		// Key: language.<tableindex>
		private Dictionary< string, IMongoCollection<TmxText>> _texts = new Dictionary<string, IMongoCollection<TmxText>>();

		// for debugging
		private bool _logSearches;

		public TmxTextsArray(IMongoDatabase database, bool logSearches)
		{
			_database = database;
			_logSearches = logSearches;
			Load();
		}

		private void Load()
		{
			_metas = _database.GetCollection<TmxMeta>("meta");
			var names = _database.ListCollectionNames().ToList();
			foreach (var name in names.Where(n => n.StartsWith(TABLE_PREFIX)))
				_texts.Add( name.Substring(TABLE_PREFIX.Length), _database.GetCollection<TmxText>(name) );

		}

		public async Task InitAsync()
		{
			// the idea: on the first import, i set the entries per table
			var filter = Builders<TmxMeta>.Filter.Empty;
			var cursor = await _metas.FindAsync(filter);
			await cursor.ForEachAsync(m =>
			{
				if (m.Type == "Entries Per Table")
					EntriesPerTable = int.Parse(m.Value);
			});
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
			// the idea - creating too many indexes at the same time can simply end up in timeout exceptions
			const int CONCURRENT_INDEX_COUNT = 5;
			for (int i = 0; i < tasks.Count; ++i) {
				var countNow = i + CONCURRENT_INDEX_COUNT < tasks.Count ? CONCURRENT_INDEX_COUNT : tasks.Count - i;
				var tasksNow = tasks.GetRange(i, countNow);
				await Task.WhenAll(tasksNow);
			}
		}
		private async Task CreateIndexesAsyncForTable(IMongoCollection<TmxText> textTable, string name)
		{
			const int RETRY_COUNT = 10;
			const int SLEEP_ON_FAIL_MS = 30000;
			var step = 0;
			for (int idx = 0; idx < RETRY_COUNT; ++idx) {
				try
				{
					// at most one text index allowed
					if (step == 0)
					{
						var indexText = Builders<TmxText>.IndexKeys.Text(i => i.LocaseText);
						await textTable.Indexes.CreateOneAsync(new CreateIndexModel<TmxText>(indexText));
						step = 1;
					}

					if (step == 1) {
						var indexTextByLangTU = Builders<TmxText>.IndexKeys.Ascending(i => i.TranslationUnitID);
						await textTable.Indexes.CreateOneAsync(new CreateIndexModel<TmxText>(indexTextByLangTU));
						step = 2;
					}

					if (step == 2) {
						await _metas.InsertOneAsync(new TmxMeta
						{
							Type = name,
							Value = "language-table",
						}, null);
					}
					log.Debug($"indexes for language {name} created");
					break;
				}
				catch {
					// timeout
					await Task.Delay(SLEEP_ON_FAIL_MS);
				}
			}
		}

		private string LanguageKey(ulong translationID, string language) {
			var languageSuffix = translationID / (ulong)EntriesPerTable;
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
			var languageSuffixIndex = translationID / (ulong)EntriesPerTable;
			var languageSuffix = $".{languageSuffixIndex}";
			lock (this)
				return _texts.Where(l => l.Key.EndsWith(languageSuffix)).Select(l => l.Value).ToList();
		}


		public async Task<TmxText> TryFindTranslation(ulong translationID, string targetLanguage, string targetLocale, bool careForLocale)
		{
			var textTable = GetTextTable(translationID, targetLanguage);
			if (textTable == null)
				return null;

			var textFilter = Builders<TmxText>.Filter.Where(f => f.TranslationUnitID == translationID 
														    && f.NormalizedLanguage == targetLanguage 
															&& (!careForLocale || f.NormalizedLocale == targetLocale));
			var textCursor = await textTable.FindAsync(textFilter, new FindOptions<TmxText>() { Limit = 1 });
			var targetTexts = new List<TmxText>();
			await textCursor.ForEachAsync(t => targetTexts.Add(t));
			if (targetTexts.Count == 0)
				// we don't have a translation
				return null;
			return targetTexts[0];
		}

		// this just performs the search and returns the results -- does NOT perform any other analyses, like, compare score and such
		public async Task<IReadOnlyList<TmxText>> ExactSearch(string text, string sourceLanguage, string sourceLocale, bool careForLocale)
		{
			// old:
			//var filter = Builders<TmxText>.Filter.Where(f => f.NormalizedLanguage == sourceLanguage && f.LocaseText == text);

			// surround text in quotes -> so that we perform an exact search
			text = $"\"{text.Replace("\"", "\\\"")}\"";
			var filter = Builders<TmxText>.Filter.Text(text, new TextSearchOptions { CaseSensitive = false, DiacriticSensitive = false, });
			if (_logSearches)
				log.Debug($"START Exact search for {text} {sourceLanguage} ");
			var textTables = GetTextTables(sourceLanguage);
			var tasks = new List<Task>();
			var texts = new List<TmxText>();
			foreach (var tt in textTables)
			{
				var copy = tt;
				var task = Task.Run(async () => {
					var cursor = await copy.FindAsync(filter, new FindOptions<TmxText>() { Limit = MAX_RESULTS_PER_TASK });
					var taskTexts = new List<TmxText>();
					await cursor.ForEachAsync(t =>
					{
						var matches = !careForLocale || t.NormalizedLocale == sourceLocale;
						if (matches)
							taskTexts.Add(t);
					});
					lock (this)
						texts.AddRange(taskTexts);
				});
				tasks.Add(task);
			}
			await Task.WhenAll(tasks);

			// note: I won't filter out any texts (thus, no limiting to MAX_RESULTS), because the search doesn't include the Score
			// also, obviously, exact searches will return very few results
			if (_logSearches)
				log.Debug($"END Exact search for {text} {sourceLanguage} ");
			return texts;
		}

		public async Task<IReadOnlyList<TmxText>> FuzzySearch(string text, string sourceLanguage, string sourceLocale, bool careForLocale) {
			var filter = Builders<TmxText>.Filter.Text(text, new TextSearchOptions { CaseSensitive = false, DiacriticSensitive = false, });

			var projection = Builders<TmxText>.Projection.MetaTextScore("Score")
				.Include(p => p.LocaseText).Include(p => p.NormalizedLanguage).Include(p => p.NormalizedLocale).Include(p => p.TranslationUnitID).Include(p => p.FormattedText);
			var sort = Builders<TmxText>.Sort.MetaTextScore("Score");

			if (_logSearches)
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
						.Limit(MAX_RESULTS_PER_TASK)
						.Project(projection)
						.ToListAsync();
					var taskTexts = new List<TmxText>();
					foreach (var p in cursor)
					{
						var t = BsonSerializer.Deserialize<TmxText>(p);
						var matches = !careForLocale || t.NormalizedLocale == sourceLocale;
						if (matches)
							taskTexts.Add(t);
					}
					lock (this)
						texts.AddRange(taskTexts);
				});
				tasks.Add(task);
			}
			await Task.WhenAll(tasks);

			texts = texts.OrderByDescending(t => t.Score).Take(MAX_RESULTS).ToList();
			if (_logSearches)
				log.Debug($"END Fuzzy search for {text} {sourceLanguage} ");

			return texts;
		}

		public async Task<IReadOnlyList<TmxText>> ConcordanceSearch(string text, string sourceLanguage, string sourceLocale, bool careForLocale)
		{
			var filter = Builders<TmxText>.Filter.Text(text, new TextSearchOptions { CaseSensitive = false, DiacriticSensitive = false, });
			if (_logSearches)
				log.Debug($"START Concordance search for {text} {sourceLanguage} ");
			var textTables = GetTextTables(sourceLanguage);
			var tasks = new List<Task>();
			var texts = new List<TmxText>();
			foreach (var tt in textTables)
			{
				var copy = tt;
				var task = Task.Run(async () => {
					var cursor = await copy.FindAsync(filter, new FindOptions<TmxText>() { Limit = MAX_RESULTS_PER_TASK });
					var taskTexts = new List<TmxText>();
					await cursor.ForEachAsync(t =>
					{
						var matches = !careForLocale || t.NormalizedLocale == sourceLocale;
						if (matches)
							taskTexts.Add(t);
					});
					lock (this)
						texts.AddRange(taskTexts);
				});
				tasks.Add(task);
			}
			await Task.WhenAll(tasks);

			// note: I won't filter out any texts (thus, no limiting to MAX_RESULTS), because the search doesn't include the Score
			if (_logSearches)
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
						await table.InsertManyAsync(copy.Value, null, token);
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
