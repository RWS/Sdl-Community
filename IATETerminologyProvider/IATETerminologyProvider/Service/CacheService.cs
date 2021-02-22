using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IATETerminologyProvider.Interface;
using IATETerminologyProvider.Model;
using Newtonsoft.Json;
using NLog;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider.Service
{
	public class CacheService : ICacheService
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly DatabaseContextService _dbContext;

		public CacheService(string activeProjectName)
		{
			if (IsDbConnected()) return;
			_logger.Info($"--> Trying to create database context for following project name: {activeProjectName}");
			_dbContext = new DatabaseContextService(activeProjectName);
		}

		public IEnumerable<SearchCache> GetAllCachedResults()
		{
			return _dbContext?.SearchCaches;
		}

		public async Task AddSearchResults(SearchCache searchCache, List<ISearchResult> iateSearchResults)
		{
			if (iateSearchResults == null) return;

			var serializedSearchResult = SerializeSearchResult(iateSearchResults);
			if (string.IsNullOrEmpty(serializedSearchResult)) return;

			searchCache.SearchResultsString = serializedSearchResult;
			_dbContext?.SearchCaches.Add(searchCache);
			if (_dbContext != null) await _dbContext.SaveChangesAsync();
		}

		public async Task ClearCachedResults()
		{
			var cachedData = GetAllCachedResults();
			_dbContext?.SearchCaches.RemoveRange(cachedData);
			if (_dbContext != null) await _dbContext.SaveChangesAsync();
		}

		public bool IsDbConnected()
		{
			if (_dbContext?.Database == null || !_dbContext.Database.Exists()) return false;
			return _dbContext.Database.Connection.State == ConnectionState.Open;
		}

		public async Task<List<ISearchResult>> GetCachedResults(string sourceText, string targetLanguageName, string bodyModelString)
		{
			if (_dbContext?.SearchCaches == null) return null;

			var cacheData = await _dbContext?.SearchCaches?.FirstOrDefaultAsync(s =>
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
			return new JsonSerializerSettings
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
