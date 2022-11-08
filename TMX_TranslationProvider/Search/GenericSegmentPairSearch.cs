using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.ProjectApi.Settings;
using TMX_TranslationProvider.Search.Result;

namespace TMX_TranslationProvider.Search
{

	internal class GenericSegmentPairSearch
	{
		private ISegmentPairSearch _provider;

		private class SimpleResults
		{
			public SearchResults ToSearchResults(SearchSettings settings, LanguagePair language)
			{
				var searchResults = new SearchResults();
				foreach (var result in Results)
					searchResults.Add(result.ToSearchResult(settings, language.SourceCulture, language.TargetCulture));
				return searchResults;
			}
			public List<SimpleResult> Results = new List<SimpleResult>();
		}

		public TranslationMemorySettings TMSettings { get; set; }

		public bool ShowMostRecentTranslationsFirst => TMSettings?.OrderSearchResultsInReverseChronologicalOrder ?? false;
		private int MaxResults(SearchSettings settings) => settings.IsConcordanceSearch 
			? (TMSettings?.ConcordanceMaximumResults ?? settings.MaxResults) : settings.MaxResults;

		private int MinScore(SearchSettings settings) => settings.IsConcordanceSearch
			? (TMSettings?.ConcordanceMinimumMatchValue ?? settings.MinScore)
			: settings.MinScore;

		private bool LookupMtEvenIfTmHasMatch => TMSettings?.LookupMtEvenIfTmHasMatch?.Value ?? false;

		public GenericSegmentPairSearch(ISegmentPairSearch provider)
		{
			_provider = provider;
		}

		private bool HaveEnoughResults(SimpleResults results, SearchSettings settings)
		{
			if (LookupMtEvenIfTmHasMatch)
				// in this case, look through all the file
				return false;

			if (results.Results.Count >= MaxResults(settings))
				return true;

			// note: ignoring upLIFT
			return false;
		}

		// remove any extraneous results, if needed
		private void CompleteSearch(SearchSettings settings, SimpleResults results)
		{
			// note: don't care about QuickInsertIds for now
			SortSearchResults(settings, results);

			// can happen if LookupMtEvenIfTmHasMatch is true
			if (results.Results.Count >= MaxResults(settings))
				results.Results = results.Results.Take(MaxResults(settings)).ToList();
		}

		private int SortSimpleResult(SimpleResult a, SimpleResult b, SortCriterium criteria)
		{
			var order = 0;
			switch (criteria.FieldName)
			{
				case "sco": // "sco" = score
					order = a.Score - b.Score;
					break;
				case "chd": // "chd" = chronological
					order = (a.TranslateTime.Ticks < b.TranslateTime.Ticks) ? -1 : (a.TranslateTime.Ticks > b.TranslateTime.Ticks ? 1 : 0);
					break;
				case "usc": // "usc" - usage counter
					// we don't record that
					order = 0;
					break;

			}

			if (criteria.Direction == SortDirection.Descending)
				order = -order;
			return 0;
		}

		private int SortSimpleResult(SimpleResult a, SimpleResult b, IReadOnlyList<SortCriterium> criteria)
		{
			foreach (var c in criteria)
			{
				var order = SortSimpleResult(a, b, c);
				if (order != 0)
					return order;
			}

			return 0;
		}

		private void SortSearchResults(SearchSettings settings, SimpleResults results)
		{
			// more about SortSpecification: TranslationMemorySettings.cs:596

			results.Results.Sort((a,b) => SortSimpleResult(a,b,settings.SortSpecification.Criteria));
		}

		public SearchResults Search(SearchSettings settings, Segment segment, LanguagePair language)
		{
			if (!_provider.SupportsSourceLanguage(language.SourceCulture) || !_provider.SupportsTargetLanguage(language.TargetCulture))
				return new SearchResults();

			var results = new SimpleResults();
			var count = _provider.SegmentPairCount();
			for (int i = 0; i < count; ++i)
			{
				var result = _provider.TryTranslate(segment.ToPlain(), i, language.SourceCulture, language.TargetCulture, settings.Mode);
				if (result != null && result.Score >= MinScore(settings))
				{
					results.Results.Add(result);
					if (HaveEnoughResults(results, settings))
						break;
				}
			}

			CompleteSearch(settings, results);
			return results.ToSearchResults(settings, language);
		}

	}
}
