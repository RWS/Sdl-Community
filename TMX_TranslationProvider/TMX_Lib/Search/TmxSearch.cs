using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.ProjectAutomation.Settings;
using TMX_Lib.Concordance;
using TMX_Lib.Db;
using TMX_Lib.TokenizeUtil;
using TMX_Lib.Utils;

namespace TMX_Lib.Search
{
	public class TmxSearch
	{

		private readonly TmxMongoDb _db;
		private LanguageArray _supportedLanguages = new LanguageArray();
		private CultureDictionary _cultures = new CultureDictionary();


		private class SimpleResults
		{
			private TokenizeText _tokenizeText = new TokenizeText();
			public List<SimpleResult> Results = new List<SimpleResult>();


			private Segment CreateTokenizedSegment(string text, CultureInfo language) => _tokenizeText.CreateTokenizedSegment(text, language);
			private Segment CreateSimpleSegment(string text, CultureInfo language)
			{
				var segment = new Segment(language);
				segment.Add(text);
				return segment;
			}

			private Segment CreateSourceSegment(string text, LanguagePair pair, SearchMode mode)
			{
				var language = mode == SearchMode.TargetConcordanceSearch ? pair.TargetCulture : pair.SourceCulture;
				var isConcordance = mode == SearchMode.ConcordanceSearch || mode == SearchMode.TargetConcordanceSearch;
				var segment = isConcordance ? CreateSimpleSegment(text, language) : CreateTokenizedSegment(text, language);
				return segment;
			}

			public SearchResults ToSearchResults(string text, TmxSearchSettings settings, LanguagePair language)
			{
				var searchResults = new SearchResults
				{
					SourceSegment = CreateSourceSegment(text, language, settings.Mode),
				};
				foreach (var result in Results)
				{
					if (settings.IsConcordanceSearch)
						searchResults.Add(new ConcordanceTokenizer(result, settings, language.SourceCulture, language.TargetCulture, text).Result);
					else 
						searchResults.Add(result.ToSearchResult(text, settings, language.SourceCulture, language.TargetCulture));

				}
				return searchResults;
			}
		}

		public TmxSearch(TmxMongoDb db)
		{
			_db = db;
		}

		public IReadOnlyList<string> SupportedLanguages() => _supportedLanguages?.Languages ?? new List<string>();

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
			return order;
		}

		private int SortSimpleResult(SimpleResult a, SimpleResult b, IReadOnlyList<SortCriterium> criteria)
		{
			foreach (var c in criteria)
			{
				var order = SortSimpleResult(a, b, c);
				if (order != 0)
					return order;
			}

			if (criteria.Count == 0)
				return b.Score - a.Score; // by default, by score, descending

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

		public IReadOnlyList<string> Languages => _supportedLanguages.Languages;

		private string SourceLanguage(LanguagePair pair) => _supportedLanguages.TryGetEquivalentLanguage(pair.SourceCultureName);
		private string TargetLanguage(LanguagePair pair) => _supportedLanguages.TryGetEquivalentLanguage(pair.TargetCultureName);

		private async Task SearchExact(TmxSearchSettings settings, string text, LanguagePair language, SimpleResults results)
		{
			if (HaveEnoughResults(results, settings))
				return;

			var dbResults = await _db.ExactSearch(text, SourceLanguage(language), TargetLanguage(language));
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

		private static int StringIntConcordanceCompare(string searchFor, string foundText)
		{
			return Math.Min((int)(SlowConcordanceCompareTexts.Compare(searchFor, foundText) * 100 + .5), 100);
		}

		private async Task SearchFuzzy(TmxSearchSettings settings, string text, LanguagePair language, SimpleResults results)
		{
			if (HaveEnoughResults(results, settings))
				return;

			var dbResults = FilterFuzzySearch( await _db.FuzzySearch(text, SourceLanguage(language), TargetLanguage(language)));
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

		private async Task SearchConcordance(TmxSearchSettings settings, string text, LanguagePair language, SimpleResults results, bool sourceConcordance = true)
		{
			if (HaveEnoughResults(results, settings))
				return;

			var sourceLanguage = sourceConcordance ? SourceLanguage(language) : TargetLanguage(language);
			var targetLanguage = sourceConcordance ? TargetLanguage(language) : SourceLanguage(language);
			var dbResults = await _db.ConcordanceSearch(text, sourceLanguage, targetLanguage);
			foreach (var dbResult in dbResults)
			{
				var score = StringIntConcordanceCompare(text, dbResult.SourceText);
				if (score >= settings.MinScore)
				{
					var result = new SimpleResult(dbResult) { Score = score };
					ApplyPenalties(result, text);
					results.Results.Add(result);
				}
			}
		}

		public async Task LoadLanguagesAsync()
		{
			_supportedLanguages.Languages = (await _db.GetAllLanguagesAsync());
		}

		public bool SupportsLanguage(string language)
		{
			return _supportedLanguages.TryGetEquivalentLanguage(language) != null;
		}

		private async Task OptimizedNormalSearch(TmxSearchSettings settings, string text, LanguagePair language, SimpleResults results) {
			// the idea - run exact and fuzzy in parallel
			// if we get exact match, I don't care about fuzzy
			SimpleResults fuzzyResults = new SimpleResults();
			var exact = Task.Run(async() => await SearchExact(settings, text, language, results));
			var fuzzy = Task.Run(async () => await SearchFuzzy(settings, text, language, fuzzyResults));
			await exact;
			var anyExactMatches = results.Results.Any(r => r.IsExactMatch);
			if (!anyExactMatches) {
				await fuzzy;
				results.Results = fuzzyResults.Results;
			}
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

			try
			{
				switch (settings.Mode)
				{
					// Performs only an exact search, without fuzzy search.
					case SearchMode.ExactSearch:
						await SearchExact(settings, text, language, results);
						break;

					// Performs a normal search, i.e. a combined exact/fuzzy search. Fuzzy search is only triggered
					// if no exact matches are found.
					case SearchMode.NormalSearch:
						/* OLD code
						await SearchExact(settings, text, language, results);
						var anyExactMatches = results.Results.Any(r => r.IsExactMatch);
						if (!anyExactMatches)
							await SearchFuzzy(settings, text, language, results);
						 */
						await OptimizedNormalSearch(settings, text, language, results);
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
						await SearchConcordance(settings, text, language, results, sourceConcordance: true);
						break;

					// Performs a concordance search on the target segments, if the target character-based index exists.
					case SearchMode.TargetConcordanceSearch:
						await SearchConcordance(settings, text, language, results, sourceConcordance: false);
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
			}
			catch (Exception e)
			{
				throw new TmxException($"Search for '{segment.ToPlain()}' failed: {e.Message}", e);
			}

			CompleteSearch(settings, results);
			return results.ToSearchResults(text, settings, language);
		}

		// for now, I use the Id - unfortunately, this is just an 'int' 
		private static ulong TranslationUnitDbID(TranslationUnit tu)
		{
			var id = tu.ResourceId.Id;
			if (id <= 0)
				throw new TmxException($"Invalid Translation Unit ID {id}");

			return (ulong)id;
		}

		private TmxTranslationUnit ToDb(TranslationUnit tu, LanguagePair languagePair)
		{
			var author = tu.FieldValues.FirstOrDefault(f => f.Name == "last_modified_by")?.GetValueString() ?? "unknown";
			var tmx = new TmxTranslationUnit
			{
				CreationDate = DateTime.Now,
				CreationAuthor = author,
				ChangeDate = DateTime.Now,
				ChangeAuthor = author,
				XmlProperties = "",
				TuAttributes = "",
				NormalizedLanguages = new List<string>
				{
					Util.NormalizeLanguage(languagePair.SourceCultureName),
					Util.NormalizeLanguage(languagePair.TargetCultureName),
				},
			};
			return tmx;
		}

		private TmxText ToDb(Segment segment, ulong tuID, CultureInfo language)
		{
			return new TmxText
			{
				TranslationUnitID = tuID, 
				NormalizedLanguage = Util.NormalizeLanguage(language.IsoLanguageName()),
				LocaseText = Util.TextToDbText(segment.ToPlain(), language),
				FormattedText = segment.ToString(), 
			};
		}

		public async Task UpdateAsync(TranslationUnit tu, LanguagePair languagePair)
		{
			// find in db, see which languages i have
			var id = TranslationUnitDbID(tu);
			var dbTU = await _db.FindTranslationUnitAsync(id);

			var sourceLanguage = Util.NormalizeLanguage(languagePair.SourceCultureName);
			var targetLanguage = Util.NormalizeLanguage(languagePair.TargetCultureName);
			var hasSource = dbTU.NormalizedLanguages.Contains(sourceLanguage);
			var hasTarget = dbTU.NormalizedLanguages.Contains(targetLanguage);

			if (!hasSource)
				dbTU.NormalizedLanguages.Add(sourceLanguage);
			if (!hasTarget)
				dbTU.NormalizedLanguages.Add(targetLanguage);
			if (!hasSource || !hasTarget)
				await _db.UpdateTranslationUnitAsync(dbTU);

			var source = ToDb(tu.SourceSegment, dbTU.TranslationUnitID, languagePair.SourceCulture);
			var target = ToDb(tu.TargetSegment, dbTU.TranslationUnitID, languagePair.TargetCulture);

			if (!hasSource || !hasTarget)
			{
				var texts = new List<TmxText>();
				if (!hasSource)
					texts.Add(source);
				if (!hasTarget)
					texts.Add(target);
				await _db.AddTextsAsync(texts);
			}

			if (hasSource)
				await _db.UpdateTextAsync(source);
			if (hasTarget)
				await _db.UpdateTextAsync(target);
		}

		// returns the ID of the newly added translation unit
		public async Task<ulong> AddAsync(TranslationUnit tu, LanguagePair languagePair)
		{
			var tmx = ToDb(tu, languagePair);
			await _db.AddSingleTranslationUnitAsync(tmx);
			// here, I know the translation unit ID, so I can create the texts

			var source = ToDb(tu.SourceSegment, tmx.TranslationUnitID, languagePair.SourceCulture);
			var target = ToDb(tu.TargetSegment, tmx.TranslationUnitID, languagePair.TargetCulture);
			await _db.AddTextsAsync(new[] { source, target });
			return tmx.TranslationUnitID;
		}


	}
}
