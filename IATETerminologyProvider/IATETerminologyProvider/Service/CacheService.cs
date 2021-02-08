using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class CacheService : ICacheService
	{
		private readonly DatabaseContextService _dbContext;

		public CacheService(DatabaseContextService dbContext)
		{
			_dbContext = dbContext;
		}

		public IEnumerable<SearchCache> GetAllCachedResults()
		{
			return _dbContext.SearchCaches.AsNoTracking();
		}

		public void AddSearchResults(SearchCache searchCache,List<ISearchResult>iateSearchResults)
		{
			if (iateSearchResults == null) return;
			var serializedSearchResult = SerializeSearchResult(iateSearchResults);
			if (string.IsNullOrEmpty(serializedSearchResult)) return;
			searchCache.SearchResultsString = serializedSearchResult;
			_dbContext.SearchCaches.Add(searchCache);
			_dbContext.SaveChanges();
		}

		public void ClearCachedResults()
		{
			var catchedData = GetAllCachedResults();
			_dbContext.SearchCaches.RemoveRange(catchedData);
			_dbContext.SaveChanges();
		}

		public List<ISearchResult> GetCachedResults(string sourceText, string targetLanguageName, string bodyModelString)
		{
			var cacheData = _dbContext.SearchCaches.FirstOrDefault(s =>
				s.SourceText.Equals(sourceText) && s.TargetLanguageName.Equals(targetLanguageName) &&
				s.QueryString.Equals(bodyModelString));

			return cacheData != null ? DeserializeSearchResult(cacheData.SearchResultsString) : null;
		}

		private string SerializeSearchResult(List<ISearchResult> iateSearchResults)
		{
			return JsonConvert.SerializeObject(iateSearchResults, Formatting.Indented, GetJsonSettings());
		}

		private List<ISearchResult> DeserializeSearchResult(string dbSavedResult)
		{
			return JsonConvert.DeserializeObject<List<ISearchResult>>(dbSavedResult, GetJsonSettings());
		}

		private JsonSerializerSettings GetJsonSettings()
		{
			return  new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			};
		}

		public void Dispose()
		{
			_dbContext?.Dispose();
		}
	}
}
