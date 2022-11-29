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
using LogManager = NLog.LogManager;

namespace TMX_Lib.Search
{
	public class TmxSearchService
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private ISearchServiceParameters _options;
		private TmxSearch _search;
		private TmxMongoDb _db;
		private bool _paramsOk = true;
		private bool _hasImportBeenDoneBefore;

		public TmxSearchService(ISearchServiceParameters options)
		{
			// async call
			SetOptionsAsync(options);
		}

		public ISearchServiceParameters Options => _options;
		public bool OptionsOk => _paramsOk;

		private static string ConnectionStr(ISearchServiceParameters parameters) => parameters.DbConnectionNoPassword.Replace("<password>", parameters.Password);
		private static string DbName(ISearchServiceParameters parameters) 
			=> parameters == null ? "" : (parameters.DbName != "" ? parameters.DbName : Path.GetFileNameWithoutExtension(parameters.FileName));

		public bool IsImporting() => _paramsOk && (_db?.IsImportInProgress() ?? false);
		public double ImportProgress() => _db?.ImportProgress() ?? 0d;
		public bool ImportComplete() => _hasImportBeenDoneBefore || (_db != null && _db.IsImportComplete());

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
				if (!_paramsOk)
					return false;

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
		public async Task SetOptionsAsync(ISearchServiceParameters newOptions)
		{
			Debug.Assert(newOptions != null);
			log.Debug($"new options {newOptions.DbConnectionNoPassword} {newOptions.FileName} db={newOptions.DbName}");
			ISearchServiceParameters oldOptions;
			TmxMongoDb db;
			lock (this)
			{
				if (newOptions.Equals(_options))
					return;

				oldOptions = _options;
				_options = newOptions;
				db = _db;
			}

			var isSameDb = DbName(oldOptions) == DbName(newOptions);
			if (!isSameDb)
				db?.CancelImport();

			try
			{
				if (!(await TryParametersAsync(newOptions)).ok)
				{
					lock (this)
						_paramsOk = false;
					return;
				}

				lock (this)
					_paramsOk = true;
				var needsReimport = oldOptions == null
				                    || (oldOptions.FileName != newOptions.FileName)
				                    || (newOptions.DbName == "")
				                    || !isSameDb;
				if (!isSameDb)
				{
					db = new TmxMongoDb(ConnectionStr(newOptions), DbName(newOptions));
					await db.InitAsync();
					_hasImportBeenDoneBefore = await db.HasImportBeenDoneBeforeAsync();
					if (_hasImportBeenDoneBefore)
						needsReimport = Path.GetFileName(newOptions.FileName) != await db.ImportedFileNameAsync();
					else 
						// this is a fresh db
						needsReimport = true;
				}

				if (db == null)
				{
					// no db connection yet
					lock (this)
						_paramsOk = false;
					return;
				}

				var search = new TmxSearch(db);
				await search.LoadLanguagesAsync();
				bool continueImport;
				lock (this)
				{
					// the idea - while we're initializing this asynchronously, we can have another call to change the params
					continueImport = _options.Equals(newOptions);
					if (continueImport)
					{
						_db = db;
						_search = search;
					}
				}

				if (continueImport && needsReimport)
					// IMPORTANT: just start the import, this can take a lot of time, for big databases
					await Task.Run(async () =>
					{
						await db.ImportToDbAsync(_options.FileName);
						await search.LoadLanguagesAsync();
					});
			}
			catch (Exception e)
			{
				lock (this)
					_paramsOk = false;
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

			return await search.Search(settings, segment, language) ?? new SearchResults();
		}

		// returns true if the given parameters are valid
		public static async Task<(bool ok,string error)> TryParametersAsync(ISearchServiceParameters parameters)
		{
			string error = "";
			try
			{
				if (!File.Exists(parameters.FileName))
				{
					error = $"File not found: {parameters.FileName}";
					return (false, error);
				}

				if (ConnectionStr(parameters) == "")
				{
					error = $"Please set the connection.";
					return (false, error);
				}

				var db = new TmxMongoDb(ConnectionStr(parameters), DbName(parameters));
				// this throws on connection failure
				await db.InitAsync();

				return (true, "");
			}
			catch (Exception e)
			{
				error = e.Message;
				return (false, error);
			}
		}
	}
}
