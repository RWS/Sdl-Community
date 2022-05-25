using System.Collections.Generic;
using Newtonsoft.Json;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.ProjectAutomation.Core;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class CacheProvider : ICacheProvider
	{
		private readonly SqliteDatabaseProvider _databaseProvider;

		public CacheProvider(SqliteDatabaseProvider databaseProvider)
		{
			_databaseProvider = databaseProvider;
		}

		public bool Connect(IProject project)
		{
			 _databaseProvider?.Connect(project);
			 return IsDbConnected();
		}

		public IEnumerable<SearchCache> GetAllCachedResults()
		{
			if (IsDbConnected())
			{
				_databaseProvider?.Get();
			}

			return null;
		}

		public void AddSearchResults(SearchCache searchCache, List<ISearchResult> searchResults)
		{
			if (searchResults == null)
			{
				return;
			}

			var serializedSearchResult = SerializeSearchResult(searchResults);
			if (string.IsNullOrEmpty(serializedSearchResult))
			{
				return;
			}

			searchCache.SearchResultsString = serializedSearchResult;

			_databaseProvider?.Insert(searchCache);
		}

		public void ClearCachedResults()
		{
			_databaseProvider?.RemoveAll();
		}

		public bool IsDbConnected()
		{
			return _databaseProvider?.IsConnected() ?? false;
		}

		public List<ISearchResult> GetCachedResults(string sourceText, string targetLanguageName, string queryString)
		{
			var searchCache = _databaseProvider?.Get(sourceText, targetLanguageName, queryString);

			return searchCache != null
				? DeserializeSearchResult(searchCache.SearchResultsString)
				: null;
		}

		private string SerializeSearchResult(List<ISearchResult> searchResults)
		{
			return JsonConvert.SerializeObject(searchResults, Formatting.Indented, GetJsonSettings());
		}

		private List<ISearchResult> DeserializeSearchResult(string searchResultsString)
		{
			return JsonConvert.DeserializeObject<List<ISearchResult>>(searchResultsString, GetJsonSettings());
		}

		private JsonSerializerSettings GetJsonSettings()
		{
			return new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			};
		}

		public void Dispose()
		{
			_databaseProvider?.CloseConnection();
		}
	}
}
