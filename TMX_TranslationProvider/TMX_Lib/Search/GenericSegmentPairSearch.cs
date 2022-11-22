using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.ProjectAutomation.Settings;
using TMX_Lib.Search.Result;
using TMX_Lib.Search.SearchSegment;

namespace TMX_Lib.Search
{

	public class GenericSegmentPairSearch
	{
		private ISegmentPairSearch _provider;

		private class SimpleResults
		{
			public SearchResults ToSearchResults(string text, SearchSettings settings, LanguagePair language)
			{
				var source = new Segment();
				source.Add(text);
				var searchResults = new SearchResults
				{
					SourceSegment = source
				};
				foreach (var result in Results)
					searchResults.Add(result.ToSearchResult(settings, language.SourceCulture, language.TargetCulture));
				return searchResults;
			}
			public List<SimpleResult> Results = new List<SimpleResult>();
		}

		public TranslationMemorySettings TMSettings { get; set; }

		private int MaxResults(SearchSettings settings) => settings.IsConcordanceSearch 
			? (TMSettings?.ConcordanceMaximumResults ?? settings.MaxResults) : settings.MaxResults;

		private int MinScore(SearchSettings settings) => settings.IsConcordanceSearch
			? (TMSettings?.ConcordanceMinimumMatchValue ?? settings.MinScore)
			: settings.MinScore;

		private bool LookupMtEvenIfTmHasMatch => TMSettings.GetSetting<bool>("LookupMtEvenIfTmHasMatch");

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

		private void SearchExact(SearchSettings settings, TextSegment text, LanguagePair language, SimpleResults results)
		{
			if (HaveEnoughResults(results, settings))
				return;

			var count = _provider.SegmentPairCount();
			var minScore = MinScore(settings);
			for (int i = 0; i < count; ++i)
			{
				var result = _provider.TryTranslateExact(text, i, language.SourceCulture, language.TargetCulture, minScore);
				if (result != null && result.Score >= minScore)
				{
					results.Results.Add(result);
					if (HaveEnoughResults(results, settings))
						break;
				}
			}
		}

		private void SearchFuzzy(SearchSettings settings, TextSegment text, LanguagePair language, SimpleResults results)
		{
			if (HaveEnoughResults(results, settings))
				return;

			var count = _provider.SegmentPairCount();
			var minScore = MinScore(settings);
			for (int i = 0; i < count; ++i)
			{
				var result = _provider.TryTranslateFuzzy(text, i, language.SourceCulture, language.TargetCulture, minScore);
				if (result != null && result.Score >= minScore)
				{
					results.Results.Add(result);
					if (HaveEnoughResults(results, settings))
						break;
				}
			}
		}
		private void SearchConcordance(SearchSettings settings, TextSegment text, LanguagePair language, SimpleResults results, bool sourceConcorance = true)
		{
			if (HaveEnoughResults(results, settings))
				return;

			var count = _provider.SegmentPairCount();
			var minScore = MinScore(settings);
			for (int i = 0; i < count; ++i)
			{
				var result = _provider.TryTranslateConcordance(text, i, language.SourceCulture, language.TargetCulture, sourceConcorance, minScore);
				if (result != null && result.Score >= minScore)
				{
					results.Results.Add(result);
					if (HaveEnoughResults(results, settings))
						break;
				}
			}
		}

		public SearchResults Search(SearchSettings settings, Segment segment, LanguagePair language)
		{
			if (!_provider.SupportsSourceLanguage(language.SourceCulture) || !_provider.SupportsTargetLanguage(language.TargetCulture))
				return new SearchResults();

			var text = new TextSegment(segment.ToPlain());
			var results = new SimpleResults();
			switch (settings.Mode)
			{
				// Performs only an exact search, without fuzzy search.
				case SearchMode.ExactSearch:
					SearchExact(settings, text, language, results);
					break;

				// Performs a normal search, i.e. a combined exact/fuzzy search. Fuzzy search is only triggered
				// if no exact matches are found.
				case SearchMode.NormalSearch:
					SearchExact(settings, text, language, results);
					var anyExactMatches = results.Results.Any(r => r.IsExactMatch);
					if (!anyExactMatches)
						SearchFuzzy(settings, text, language, results);
					break;

				// Performs a full search, i.e. a combined exact/fuzzy search. In contrast to NormalSearch, 
				// fuzzy search is always triggered, even if exact matches are found.
				case SearchMode.FullSearch:
					SearchExact(settings, text, language, results);
					SearchFuzzy(settings, text, language, results);
					break;

				// Performs a concordance search on the source segments, using the source character-based index if it exists or
				// the default word-based index otherwise.
				case SearchMode.ConcordanceSearch:
					SearchConcordance(settings, text, language, results, sourceConcorance: true);
					break;

				// Performs a concordance search on the target segments, if the target character-based index exists.
				case SearchMode.TargetConcordanceSearch:
					SearchConcordance(settings, text, language, results, sourceConcorance: false);
					break;

				// Performs only a fuzzy search. 
				case SearchMode.FuzzySearch:
					SearchFuzzy(settings, text, language, results);
					break;

				// Performs a search on the source and target hashes for duplicate search during import (only used internally).
				case SearchMode.DuplicateSearch:
					throw new ArgumentOutOfRangeException(nameof(SearchMode));
				default:
					throw new ArgumentOutOfRangeException();
			}

			CompleteSearch(settings, results);
			return results.ToSearchResults(text.OriginalText, settings, language);
		}

	}
}
