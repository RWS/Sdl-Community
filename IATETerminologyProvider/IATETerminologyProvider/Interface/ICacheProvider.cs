using System;
using System.Collections.Generic;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.ProjectAutomation.Core;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Interface
{
	public interface ICacheProvider : IDisposable
	{
		bool Connect(IProject project);
		
		List<ISearchResult> GetCachedResults(string sourceText, string targetLanguage, string queryString);
		
		IEnumerable<SearchCache> GetAllCachedResults();

		void AddSearchResults(SearchCache searchCache, List<ISearchResult> searchResult);

		void ClearCachedResults();

		bool IsDbConnected();
	}
}
