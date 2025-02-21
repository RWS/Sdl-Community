using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Db;
using TMX_Lib.Writer;
using LogManager = NLog.LogManager;

namespace TMX_Lib.Search
{
	public class TmxSearchService
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private string _dbName;

		private TmxSearch _search;
		
		private IReadOnlyList<TmxMongoDb> _databases;

		private bool _hasImportBeenDoneBefore;
		private TmxImportReport _report = new TmxImportReport();

		Task _initTask;
		public TmxSearchService(IReadOnlyList<TmxMongoDb> databases)
		{
			// async call
			_databases = databases;
			_initTask = SetOptionsAsync(databases);
		}

		// report about the current import
		public TmxImportReport Report => _report;


		public bool IsImporting() => _databases.Any(db => db.IsImportInProgress());


		public async Task InitAsync() => await _initTask;

		public IReadOnlyList<string> Languages => _search?.SupportedLanguages() ?? new List<string>();

		public bool SupportsLanguage(LanguagePair language, bool careForeLocale)
		{
			return SupportsLanguage(language.SourceCultureName, careForeLocale) && SupportsLanguage(language.TargetCultureName, careForeLocale);
		}

		public bool SupportsLanguage(string language, bool careForeLocale)
		{
			// if not fully loaded, return true
			TmxSearch search;
			lock (this)
			{
				if (IsImporting())
					// during import, we don't know what languages we support
					return true;

				search = _search;
			}

			if (search == null)
				// we haven't fully connected to the db yet
				return true;
			return search.SupportsLanguage(language, careForeLocale);
		}

		// if it will start an import, this will return once the import is complete
		private async Task SetOptionsAsync(IReadOnlyList<TmxMongoDb> databases)
		{
			try
			{
				foreach (var db in databases)
					await db.InitAsync();

				var search = new TmxSearch(databases);
				await search.LoadLanguagesAsync();
				lock(this)
					_search = search;
			}
			catch (Exception e)
			{
				// note: we can't throw here, it would end the application
				log.Fatal($"can't initialize the search service {e.Message}");
			}
		}

		public async Task<SearchResults> Search(SearchSettings settings, Segment segment, LanguagePair language, bool careForeLocale)
		{
			TmxSearch search;
			lock (this)
				search = _search;
			if (search == null)
				return new SearchResults();

			return await search.Search(TmxSearchSettings.FromSearchSettings(settings), segment, language, careForeLocale) ?? new SearchResults();
		}

		public async Task UpdateAsync(TranslationUnit tu, LanguagePair languagePair)
		{
			TmxSearch search;
			lock (this)
				search = _search;
			if (search == null)
				return ;
			await search.UpdateAsync(tu, languagePair);
		}
		public async Task AddAsync(TranslationUnit tu, LanguagePair languagePair)
		{
			TmxSearch search;
			lock (this)
				search = _search;
			if (search == null)
				return;
			await search.AddAsync(tu, languagePair);
		}


		public async Task ExportToFileAsync(string fileName, Func<double, bool> continueFunc)
		{
			if (_databases.Count != 1)
				throw new TmxException("Export can only be done from a single database");

			using (var writer = new TmxWriter(fileName))
			{
				ulong writeBlock = 100;
				await writer.WriteAsync(_databases[0], continueFunc, writeBlock);
			}
		}
	}
}
