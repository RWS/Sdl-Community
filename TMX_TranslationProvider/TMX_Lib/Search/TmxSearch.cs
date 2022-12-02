using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.ProjectAutomation.Settings;
using TMX_Lib.Db;
using TMX_Lib.Utils;

namespace TMX_Lib.Search
{
	public class TmxSearch
	{

		private readonly TmxMongoDb _db;
		private IReadOnlyList<string> _supportedLanguages = new List<string>();
		private CultureDictionary _cultures = new CultureDictionary();

		private class SimpleResults
		{
			public SearchResults ToSearchResults(string text, TmxSearchSettings settings, LanguagePair language)
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

		public TmxSearch(TmxMongoDb db)
		{
			_db = db;
		}


		private bool LookupMtEvenIfTmHasMatch => true;

		// remove any extraneous results, if needed
		private void CompleteSearch(TmxSearchSettings settings, SimpleResults results)
		{
			// note: don't care about QuickInsertIds for now
			SortSearchResults(settings, results);

			// can happen if LookupMtEvenIfTmHasMatch is true
			if (results.Results.Count >= settings.MaxResults)
				results.Results = results.Results.Take(settings.MaxResults).ToList();
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
		private void SortSearchResults(TmxSearchSettings settings, SimpleResults results)
		{
			// more about SortSpecification: TranslationMemorySettings.cs:596
			results.Results.Sort((a, b) => SortSimpleResult(a, b, settings.SortSpecification.Criteria));
		}

		private bool HaveEnoughResults(SimpleResults results, TmxSearchSettings settings)
		{
			if (LookupMtEvenIfTmHasMatch)
				// in this case, look through all the file
				return false;

			if (results.Results.Count >= settings.MaxResults)
				return true;

			// note: ignoring upLIFT
			return false;
		}

		// searchedText - the text I'm searching for
		// result - the text I have internally
		private void ApplyPenalties(SimpleResult result, string searchedText)
		{
			// FIXME
		}

		private async Task SearchExact(TmxSearchSettings settings, string text, LanguagePair language, SimpleResults results)
		{
			if (HaveEnoughResults(results, settings))
				return;

			var dbResults = await _db.ExactSearch(text, language.SourceCultureName, language.TargetCultureName);
			foreach (var dbResult in dbResults)
			{
				var score = StringIntCompare(text, dbResult.SourceText);
				if (score >= settings.MinScore )
				{
					var result = new SimpleResult(dbResult) { Score = score };
					ApplyPenalties(result, text);
					results.Results.Add(result);
				}
			}
		}

		// note: it's sorted by score
		private IReadOnlyList<TmxSegment> FilterFuzzySearch(IReadOnlyList<TmxSegment> results)
		{
			if (results.Count < 2)
				return results;

			var diff = results.First().Score - results.Last().Score;
			Debug.Assert(diff >= 0);
			if (diff < 1)
				return results;

			// here, diff is > 1, note: I'm using the score from the mongodb
			// and do a simple filtering: if score not in the top-third of the results, ignore it
			var min = results.First().Score - diff / 3;
			return results.Where(r => r.Score >= min).ToList();
		}

		private static int StringIntCompare(string a, string b)
		{
			return Math.Min((int)(SlowCompareTexts.Compare(a, b) * 100 + .5), 100);
		}


		private async Task SearchFuzzy(TmxSearchSettings settings, string text, LanguagePair language, SimpleResults results)
		{
			if (HaveEnoughResults(results, settings))
				return;

			var dbResults = FilterFuzzySearch( await _db.FuzzySearch(text, language.SourceCultureName, language.TargetCultureName));
			foreach (var dbResult in dbResults)
			{
				var score = StringIntCompare(text, dbResult.SourceText);
				if (score >= settings.MinScore)
				{
					var result = new SimpleResult(dbResult) { Score = score };
					ApplyPenalties(result, text);
					results.Results.Add(result);
				}
			}
		}

		private async Task SearchConcordance(TmxSearchSettings settings, string text, LanguagePair language, SimpleResults results, bool sourceConcorance = true)
		{
			if (HaveEnoughResults(results, settings))
				return;

		}

		public async Task LoadLanguagesAsync()
		{
			_supportedLanguages = (await _db.GetAllLanguagesAsync());
		}

		public bool SupportsLanguage(string language)
		{
			return _supportedLanguages.Any(l => l.Equals(language, StringComparison.OrdinalIgnoreCase));
		}

		public async Task<SearchResults> Search(TmxSearchSettings settings, Segment segment, LanguagePair language)
		{
			if (_db.IsImportInProgress() && !_db.IsImportComplete())
				// while importing, don't do any searches
				return new SearchResults();

			var hasLanguages = SupportsLanguage(language.SourceCultureName) && SupportsLanguage(language.TargetCultureName);
			if (!hasLanguages)
				return new SearchResults();

			var text = segment.ToPlain();
			var results = new SimpleResults();
			switch (settings.Mode)
			{
				// Performs only an exact search, without fuzzy search.
				case SearchMode.ExactSearch:
					await SearchExact(settings, text, language, results);
					break;

				// Performs a normal search, i.e. a combined exact/fuzzy search. Fuzzy search is only triggered
				// if no exact matches are found.
				case SearchMode.NormalSearch:
					await SearchExact(settings, text, language, results);
					var anyExactMatches = results.Results.Any(r => r.IsExactMatch);
					if (!anyExactMatches)
						await SearchFuzzy(settings, text, language, results);
					break;

				// Performs a full search, i.e. a combined exact/fuzzy search. In contrast to NormalSearch, 
				// fuzzy search is always triggered, even if exact matches are found.
				case SearchMode.FullSearch:
					await SearchExact(settings, text, language, results);
					await SearchFuzzy(settings, text, language, results);
					break;

				// Performs a concordance search on the source segments, using the source character-based index if it exists or
				// the default word-based index otherwise.
				case SearchMode.ConcordanceSearch:
					await SearchConcordance(settings, text, language, results, sourceConcorance: true);
					break;

				// Performs a concordance search on the target segments, if the target character-based index exists.
				case SearchMode.TargetConcordanceSearch:
					await SearchConcordance(settings, text, language, results, sourceConcorance: false);
					break;

				// Performs only a fuzzy search. 
				case SearchMode.FuzzySearch:
					await SearchFuzzy(settings, text, language, results);
					break;

				// Performs a search on the source and target hashes for duplicate search during import (only used internally).
				case SearchMode.DuplicateSearch:
					throw new ArgumentOutOfRangeException(nameof(SearchMode));
				default:
					throw new ArgumentOutOfRangeException();
			}

			CompleteSearch(settings, results);
			return results.ToSearchResults(text, settings, language);
		}

	}
}
