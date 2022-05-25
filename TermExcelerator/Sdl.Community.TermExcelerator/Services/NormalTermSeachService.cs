﻿using System.Collections.Generic;
using System.Linq;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Community.TermExcelerator.Services.Interfaces;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator.Services
{
	public class NormalTermSeachService : ITermSearchService
	{
		private readonly ProviderSettings _providerSettings;
		public NormalTermSeachService(ProviderSettings providerSettings)
		{
			_providerSettings = providerSettings;
		}

		public IList<ISearchResult> Search(string text, List<ExcelEntry> entries, int maxResultsCount)
		{
			var lowerSearchText = text.ToLower();

			var results = (from entry in entries
						   let matchedSearch = Contains(lowerSearchText, entry.SearchText)
						   where !string.IsNullOrEmpty(matchedSearch)
						   select new SearchResult
						   {
							   Id = entry.Id,
							   Language = entry.Languages.Cast<ExcelEntryLanguage>().First(lang => lang.IsSource),
							   Score = 100,
							   Text = matchedSearch
						   }).ToList();

			return results
				.Take(maxResultsCount)
				.Cast<ISearchResult>()
				.ToList();
		}

		public string Contains(string searchedText, string sourceText)
		{
			var splittedSearchText = sourceText.ToLower().Split(_providerSettings.Separator);

			foreach (var searchText in splittedSearchText.Where(searchedText.Contains))
			{
				return searchText;
			}
			return string.Empty;
		}
	}
}