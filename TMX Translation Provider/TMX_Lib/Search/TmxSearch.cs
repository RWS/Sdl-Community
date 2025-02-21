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

		private readonly IReadOnlyList< TmxMongoDb> _dbs;
		private List<LanguageArray> _supportedLanguages = new List<LanguageArray>();
		private CultureDictionary _cultures = new CultureDictionary();

		private enum SearchType { 
			Exact, Fuzzy, Concordance, TargetConcordance,
		}

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

		public TmxSearch(IReadOnlyList<TmxMongoDb> dbs) {
			_dbs = dbs;
		}

		public TmxSearch(TmxMongoDb db)
		{
			_dbs = new List<TmxMongoDb> { db };
		}

		public IReadOnlyList<string> SupportedLanguages() {
			var supported = new HashSet<string>();
			foreach (var sl in _supportedLanguages)
				foreach (var l in sl.LanguageAndLocaleArray())
				supported.Add(l);
			return supported.ToList();
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

		private string SourceLanguage(LanguagePair pair) => pair.SourceCultureName;
		private string TargetLanguage(LanguagePair pair) => pair.TargetCultureName;

		// returns the databases that match this language pair
		private IEnumerable<TmxMongoDb> MatchingDbs(LanguagePair language, bool careForLocale) {
			for (var i = 0; i < _dbs.Count; i++) {
				var la = _supportedLanguages[i];
				if (la.SupportsLanguage(language.SourceCultureName, careForLocale) && la.SupportsLanguage(language.TargetCultureName, careForLocale))
					yield return _dbs[i];
			}
		}

		private async Task<IReadOnlyList<TmxSegment>> ParallelSearch(SearchType searchType, string text, LanguagePair language, bool careForLocale) {
			var dbs = MatchingDbs(language, careForLocale).ToList();
			List<TmxSegment> segments = new List<TmxSegment>();
			var tasks = dbs.Select(db => {
				var dbCopy = db;
				return Task.Run(async () => {
					switch (searchType)
					{
						case SearchType.Exact:{
								var dbResults = await dbCopy.ExactSearch(text, SourceLanguage(language), TargetLanguage(language), careForLocale);
								lock (this)
									segments.AddRange(dbResults);
							}
							break;
						case SearchType.Fuzzy:{
								var dbResults = await dbCopy.FuzzySearch(text, SourceLanguage(language), TargetLanguage(language), careForLocale);
								lock (this)
									segments.AddRange(dbResults);
							}
							break;
						case SearchType.Concordance:{
								var dbResults = await dbCopy.ConcordanceSearch(text, SourceLanguage(language), TargetLanguage(language), careForLocale);
								lock (this)
									segments.AddRange(dbResults);
							}
							break;
						case SearchType.TargetConcordance: {
								var dbResults = await dbCopy.ConcordanceSearch(text, TargetLanguage(language), SourceLanguage(language), careForLocale);
								lock (this)
									segments.AddRange(dbResults);
							}
							break;
					}
				});
			}).ToList();

			if (tasks.Count > 0)
				await Task.WhenAll(tasks);

			var needsResort = searchType == SearchType.Fuzzy;
			if (needsResort)
				segments = segments.OrderByDescending(s => s.Score).ToList();
			return segments;
		}

		private async Task SearchExact(TmxSearchSettings settings, string text, LanguagePair language, SimpleResults results, bool careForLocale)
		{
			if (HaveEnoughResults(results, settings))
				return;

			var dbResults = await ParallelSearch(SearchType.Exact, text, language, careForLocale);
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

		private async Task SearchFuzzy(TmxSearchSettings settings, string text, LanguagePair language, SimpleResults results, bool careForLocale)
		{
			if (HaveEnoughResults(results, settings))
				return;

			var dbResults = FilterFuzzySearch(await ParallelSearch(SearchType.Fuzzy, text, language, careForLocale));
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

		private async Task SearchConcordance(TmxSearchSettings settings, string text, LanguagePair language, SimpleResults results, bool sourceConcordance , bool careForLocale)
		{
			if (HaveEnoughResults(results, settings))
				return;

			var dbResults = await ParallelSearch(sourceConcordance ? SearchType.Concordance : SearchType.TargetConcordance, text, language, careForLocale);
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
			foreach (var db in _dbs) {
				await db.InitAsync();
				var la = new LanguageArray();
				la.LoadLanguages(await db.GetAllLanguagesAsync());
				_supportedLanguages.Add(la);
			}
		}

		public bool SupportsLanguage(string language, bool careForLocale)
		{
			return _supportedLanguages.Any(sl => sl.SupportsLanguage(language, careForLocale));
		}

		private async Task OptimizedNormalSearch(TmxSearchSettings settings, string text, LanguagePair language, SimpleResults results, bool careForLocale) {
			// the idea - run exact and fuzzy in parallel
			// if we get exact match, I don't care about fuzzy
			SimpleResults fuzzyResults = new SimpleResults();
			var exact = Task.Run(async() => await SearchExact(settings, text, language, results, careForLocale));
			var fuzzy = Task.Run(async () => await SearchFuzzy(settings, text, language, fuzzyResults, careForLocale));
			await exact;
			var anyExactMatches = results.Results.Any(r => r.IsExactMatch);
			if (!anyExactMatches) {
				await fuzzy;
				results.Results = fuzzyResults.Results;
			}
		}


		public async Task<SearchResults> Search(TmxSearchSettings settings, Segment segment, LanguagePair language, bool careForLocale = false)
		{
			if (_dbs.Any(db => db.IsImportInProgress()) && _dbs.Any(db => !db.IsImportComplete()))
				// while importing, don't do any searches
				return new SearchResults();

			var supportsLanguages = SupportsLanguage(language.SourceCultureName, careForLocale) && SupportsLanguage(language.TargetCultureName, careForLocale);
			if (!supportsLanguages)
				return new SearchResults();

			var text = segment.ToPlain();
			var results = new SimpleResults();

			try
			{
				switch (settings.Mode)
				{
					// Performs only an exact search, without fuzzy search.
					case SearchMode.ExactSearch:
						await SearchExact(settings, text, language, results, careForLocale);
						break;

					// Performs a normal search, i.e. a combined exact/fuzzy search. Fuzzy search is only triggered
					// if no exact matches are found.
					case SearchMode.NormalSearch:
						await OptimizedNormalSearch(settings, text, language, results, careForLocale);
						break;

					// Performs a full search, i.e. a combined exact/fuzzy search. In contrast to NormalSearch, 
					// fuzzy search is always triggered, even if exact matches are found.
					case SearchMode.FullSearch:
						await SearchExact(settings, text, language, results, careForLocale);
						await SearchFuzzy(settings, text, language, results, careForLocale);
						break;

					// Performs a concordance search on the source segments, using the source character-based index if it exists or
					// the default word-based index otherwise.
					case SearchMode.ConcordanceSearch:
						await SearchConcordance(settings, text, language, results, sourceConcordance: true, careForLocale);
						break;

					// Performs a concordance search on the target segments, if the target character-based index exists.
					case SearchMode.TargetConcordanceSearch:
						await SearchConcordance(settings, text, language, results, sourceConcordance: false, careForLocale);
						break;

					// Performs only a fuzzy search. 
					case SearchMode.FuzzySearch:
						await SearchFuzzy(settings, text, language, results, careForLocale);
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
					Util.ToLocaseLanguage(languagePair.SourceCultureName),
					Util.ToLocaseLanguage(languagePair.TargetCultureName),
				},
			};
			return tmx;
		}


		private TmxText ToDb(Segment segment, ulong tuID, CultureInfo fullLanguageName)
		{
			var (language, locale) = Util.NormalizeLanguage(fullLanguageName.IsoLanguageName());
			return new TmxText
			{
				TranslationUnitID = tuID, 
				NormalizedLanguage = language,
				NormalizedLocale = locale,
				LocaseText = Util.TextToDbText(segment.ToPlain(), fullLanguageName),
				FormattedText = segment.ToString(), 
			};
		}

		// the idea: when we have several databases, we'll need to guess the best one when we need to add or update a TU
		private TmxMongoDb GuessBestTranslationUnitDb(LanguagePair languagePair) {
			if (_dbs.Count == 1)
				return _dbs[0];

			for (int i = 0; i < _dbs.Count; ++i) {
				var sl = _supportedLanguages[i];
				if (sl.SupportsLanguage(languagePair.SourceCultureName, careForLocale: true) && sl.SupportsLanguage(languagePair.TargetCultureName, careForLocale: true))
					return _dbs[i];
			}
			for (int i = 0; i < _dbs.Count; ++i)
			{
				var sl = _supportedLanguages[i];
				if (sl.SupportsLanguage(languagePair.SourceCultureName, careForLocale: false) && sl.SupportsLanguage(languagePair.TargetCultureName, careForLocale: false))
					return _dbs[i];
			}

			for (int i = 0; i < _dbs.Count; ++i)
			{
				var sl = _supportedLanguages[i];
				if (sl.SupportsLanguage(languagePair.SourceCultureName, careForLocale: true) )
					return _dbs[i];
			}
			for (int i = 0; i < _dbs.Count; ++i)
			{
				var sl = _supportedLanguages[i];
				if (sl.SupportsLanguage(languagePair.SourceCultureName, careForLocale: false))
					return _dbs[i];
			}

			return _dbs[0];
		}

		public async Task UpdateAsync(TranslationUnit tu, LanguagePair languagePair)
		{
			// find in db, see which languages i have
			var id = TranslationUnitDbID(tu);
			var db = GuessBestTranslationUnitDb(languagePair);
			var dbTU = await db.FindTranslationUnitAsync(id);

			var sourceLanguage = Util.ToLocaseLanguage(languagePair.SourceCultureName);
			var targetLanguage = Util.ToLocaseLanguage(languagePair.TargetCultureName);
			var hasSource = dbTU.NormalizedLanguages.Contains(sourceLanguage);
			var hasTarget = dbTU.NormalizedLanguages.Contains(targetLanguage);

			if (!hasSource)
				dbTU.NormalizedLanguages.Add(sourceLanguage);
			if (!hasTarget)
				dbTU.NormalizedLanguages.Add(targetLanguage);
			if (!hasSource || !hasTarget)
				await db.UpdateTranslationUnitAsync(dbTU);

			var source = ToDb(tu.SourceSegment, dbTU.TranslationUnitID, languagePair.SourceCulture);
			var target = ToDb(tu.TargetSegment, dbTU.TranslationUnitID, languagePair.TargetCulture);

			if (!hasSource || !hasTarget)
			{
				var texts = new List<TmxText>();
				if (!hasSource)
					texts.Add(source);
				if (!hasTarget)
					texts.Add(target);
				await db.AddTextsAsync(texts);
			}

			if (hasSource)
				await db.UpdateTextAsync(source);
			if (hasTarget)
				await db.UpdateTextAsync(target);
		}

		// returns the ID of the newly added translation unit
		public async Task<ulong> AddAsync(TranslationUnit tu, LanguagePair languagePair)
		{
			var tmx = ToDb(tu, languagePair);
			var db = GuessBestTranslationUnitDb(languagePair);
			await db.AddSingleTranslationUnitAsync(tmx);
			// here, I know the translation unit ID, so I can create the texts

			var source = ToDb(tu.SourceSegment, tmx.TranslationUnitID, languagePair.SourceCulture);
			var target = ToDb(tu.TargetSegment, tmx.TranslationUnitID, languagePair.TargetCulture);
			await db.AddTextsAsync(new[] { source, target });
			return tmx.TranslationUnitID;
		}


	}
}
