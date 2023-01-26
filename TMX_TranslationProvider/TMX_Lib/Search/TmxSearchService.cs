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
		private TmxMongoDb _db;
		private bool _hasImportBeenDoneBefore;
		private TmxImportReport _report = new TmxImportReport();

		Task _initTask;
		public TmxSearchService(string dbName)
		{
			// async call
			_initTask = SetOptionsAsync(dbName);
		}

		// report about the current import
		public TmxImportReport Report => _report;


		public bool IsImporting() => (_db?.IsImportInProgress() ?? false);
		public double ImportProgress() => _db?.ImportProgress() ?? 0d;
		public bool ImportComplete() => _hasImportBeenDoneBefore || (_db != null && _db.IsImportComplete());
		public bool HasImportBeenDoneBefore() => _hasImportBeenDoneBefore;

		// if non empty, an error happened while import
		public string ImportError() => _db?.ImportError() ?? "";

		public async Task InitAsync() => await _initTask;

		public IReadOnlyList<string> Languages => _search?.Languages ?? new List<string>();

		public bool SupportsLanguage(LanguagePair language)
		{
			return SupportsLanguage(language.SourceCultureName) && SupportsLanguage(language.TargetCultureName);
		}

		public bool SupportsLanguage(string language)
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
			return search.SupportsLanguage(language);
		}

		// if it will start an import, this will return once the import is complete
		private async Task SetOptionsAsync(string dbName)
		{
			log.Debug($"search service {dbName} ");
			TmxMongoDb db;
			lock (this)
			{
				if (_dbName == dbName)
					return; // same db

				_dbName = dbName;
				db = _db;
			}

			try
			{
				db = new TmxMongoDb(dbName);
				await db.InitAsync();

				var search = new TmxSearch(db);
				await search.LoadLanguagesAsync();
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public async Task<SearchResults> Search(SearchSettings settings, Segment segment, LanguagePair language)
		{
			TmxSearch search;
			lock (this)
				search = _search;
			if (search == null)
				return new SearchResults();

			return await search.Search(TmxSearchSettings.FromSearchSettings(settings), segment, language) ?? new SearchResults();
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
			using (var writer = new TmxWriter(fileName))
			{
				ulong writeBlock = 100;
				await writer.WriteAsync(_db, continueFunc, writeBlock);
			}
		}
	}
}
