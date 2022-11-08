using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_TranslationProvider.Search.Result;

namespace TMX_TranslationProvider.Search
{

	internal class GenericSegmentPairSearch
	{
		private ISegmentPairSearch _provider;

		private class SimpleResults
		{
			public SearchResults ToSearchResults(LanguagePair language)
			{
				var searchResults = new SearchResults();
				foreach (var result in Results)
					searchResults.Add(result.ToSearchResult(language.SourceCulture, language.TargetCulture));
				return searchResults;
			}
			public List<SimpleResult> Results = new List<SimpleResult>();
		}

		public GenericSegmentPairSearch(ISegmentPairSearch provider)
		{
			_provider = provider;
		}

		private bool HaveEnoughResults(SimpleResults results, SearchSettings settings)
		{
			if (results.Results.Count >= settings.MaxResults)
				return true;

			// note: ignoring upLIFT

			// care about penalties

			return false;
		}

		// remove any extraneous results, if needed
		private void CompleteSearch(SimpleResults results)
		{
			// care about SortSpecification

			// care about penalties - searchresult.ScoringResult.ApplyPenalty()

			// do i need to care about QuickInsertIds ???

			// option: show most recent translations first
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
				if (result != null && result.Score >= settings.MinScore)
				{
					results.Results.Add(result);
					if (HaveEnoughResults(results, settings))
						break;
				}
			}

			CompleteSearch(results);
			return results.ToSearchResults(language);
		}

	}
}
