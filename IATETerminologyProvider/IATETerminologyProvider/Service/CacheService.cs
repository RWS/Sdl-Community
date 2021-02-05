using System.Collections.Generic;
using System.Linq;
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

		public void AddSearchResults(SearchCache searchCache)
		{
			if (searchCache.SearchResults == null) return;
			_dbContext.SearchCaches.Add(searchCache);
			_dbContext.SaveChanges();
		}

		public void ClearCachedResults()
		{
			var catchedData = GetAllCachedResults();
			_dbContext.SearchCaches.RemoveRange(catchedData);
			_dbContext.SaveChanges();
		}

		public IList<SearchResultModel> GetCachedResults(string sourceText, string targetLanguageName, string bodyModelString)
		{
			var cacheData = _dbContext.SearchCaches.FirstOrDefault(s =>
				s.SourceText.Equals(sourceText) && s.TargetLanguageName.Equals(targetLanguageName) &&
				s.QueryString.Equals(bodyModelString));
			//var test = cacheData.SearchResults.ToList() as List<ISearchResult>;
			return cacheData?.SearchResults.ToList();
		}

		public void Dispose()
		{
			_dbContext?.Dispose();
		}
	}
}
