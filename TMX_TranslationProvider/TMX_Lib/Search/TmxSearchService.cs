using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Db;

namespace TMX_Lib.Search
{
	public class TmxSearchService
	{
		private ISearchServiceParameters _parameters;
		private TmxSearch _search;
		private TmxMongoDb _db;
		private bool _paramsOk = false;

		public TmxSearchService(ISearchServiceParameters parameters)
		{
			_parameters = parameters;
		}

		public ISearchServiceParameters Parameters => _parameters;
		public bool ParametersOk => _paramsOk;

		private static string ConnectionStr(ISearchServiceParameters parameters) => parameters.DbConnectionNoPassword.Replace("<password>", parameters.Password);
		private static string DbName(ISearchServiceParameters parameters) 
			=> parameters == null ? "" : (parameters.DbName != "" ? parameters.DbName : Path.GetFileNameWithoutExtension(parameters.FileName));

		public bool IsImporting() => _paramsOk && (_db?.IsImportInProgress() ?? false);
		public double ImportProgress() => _db?.ImportProgress() ?? 0d;

		public async Task SetParametersAsync(ISearchServiceParameters newParameters)
		{
			Debug.Assert(newParameters != null);

			var isSameDb = DbName(_parameters) == DbName(newParameters);
			if (!isSameDb)
			{
				_db?.CancelImport();
				_db = null;
			}
			if (!(await TryParametersAsync(newParameters)).ok)
			{
				_parameters = newParameters;
				_paramsOk = false;
				return;
			}

			_paramsOk = true;
			var needsReimport = _parameters == null 
			                    || (_parameters.FileName != newParameters.FileName) 
			                    || (newParameters.DbName == "")
			                    || !isSameDb;
			_parameters = newParameters;
			if (!isSameDb)
			{
				_db = new TmxMongoDb(ConnectionStr(_parameters), DbName(_parameters));
				await _db.InitAsync();
				if (!await _db.HasImportBeenDoneBeforeAsync())
					// this is a fresh db
					needsReimport = true;
			}

			_search = new TmxSearch(_db);
			await _search.LoadLanguagesAsync();
			if (needsReimport)
				// IMPORTANT: just start the import, this can take a lot of time, for big databases
				Task.Run(async () =>
				{
					var search = _search;
					await _db.ImportToDbAsync(_parameters.FileName);
					await search.LoadLanguagesAsync();
				});

		}

		public SearchResults Search(SearchSettings settings, Segment segment, LanguagePair language)
		{
			return _search.Search(settings, segment, language) ?? new SearchResults();
		}

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
