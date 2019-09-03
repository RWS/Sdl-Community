using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Community.DtSearch4Studio.Provider.Helpers;
using Sdl.Community.DtSearch4Studio.Provider.Model;
using Sdl.Community.DtSearch4Studio.Provider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DtSearch4Studio.Provider.Studio
{
	class DtSearch4StudioLanguageDirection : ITranslationProviderLanguageDirection
	{
		#region Private Members
		private DtSearch4StudioProvider _dtSearch4StudioProvider;
		private LanguagePair _languagePair;
		private ProviderSettings _providerSettings;
		#endregion

		#region Constructors
		public DtSearch4StudioLanguageDirection(DtSearch4StudioProvider dtSearch4StudioProvider, LanguagePair languageDirection)
		{
			_dtSearch4StudioProvider = dtSearch4StudioProvider;
			_languagePair = languageDirection;
			_providerSettings = dtSearch4StudioProvider?.ProviderSettings;
		}
		#endregion

		#region Public Properties
		public CultureInfo SourceLanguage => _languagePair?.SourceCulture;
		public CultureInfo TargetLanguage => _languagePair?.TargetCulture;
		public bool CanReverseLanguageDirection { get; }
		public ITranslationProvider TranslationProvider => _dtSearch4StudioProvider;
		public static readonly Log Log = Log.Instance;
		#endregion

		#region Public Methods

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			var results = new SearchResults[segments.Length];
			try
			{
				var searchService = new SearchService(SourceLanguage, TargetLanguage);
				for (int i = 0; i < segments.Length; i++)
				{
					if (mask != null && !mask[i])
					{
						results[i] = null;
						continue;
					}

					var wordResults = new List<SearchResult>();
					var words = searchService.GetResults(_providerSettings.IndexPath, segments[i].ToPlain());
					foreach (var word in words)
					{
						var searchRes = searchService.CreateSearchResult(word, segments[i].ToPlain());
						wordResults.Add(searchRes);
					}
					results[i] = new SearchResults
					{
						SourceSegment = segments[i].Duplicate()
					};
					results[i].Results.AddRange(wordResults);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.SearchSegments}: {ex.Message}\n {ex.StackTrace}");
			}
			return results;
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			// Need this vs having mask parameter default to null as inheritence doesn't allow default values to
			// count as the same thing as having no parameter at all. IE, you can't have
			// public string foo(string s = null) override public string foo().
			return SearchSegments(settings, segments, null);
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			return SearchSegment(settings, translationUnit.SourceSegment);
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			return SearchSegments(settings, translationUnits.Select(tu => tu.SourceSegment).ToArray());
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			if (translationUnits == null)
			{
				throw new ArgumentNullException("translationUnits", "TranslationUnits in SearchSegmentsMasked");
			}
			if (mask == null || mask.Length != translationUnits.Length)
			{
				throw new ArgumentException("Mask in SearchSegmentsMasked");
			}
			var translationUnit = translationUnits.Where((seg, i) => mask == null || mask[i]).ToArray();
			return SearchSegments(settings, translationUnits.Select(tu => tu?.SourceSegment).ToArray(), mask);
		}

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			if (segments == null)
			{
				throw new ArgumentNullException("segments", "Segments in SearchSegmentsMasked");
			}
			if (mask == null || mask.Length != segments.Length)
			{
				throw new ArgumentException("Mask in SearchSegmentsMasked");
			}
			return SearchSegments(settings, segments, mask);
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			var searchResults = new SearchResults();
			try
			{
				var searchService = new SearchService(SourceLanguage, TargetLanguage);
				var wordResults = new List<SearchResult>();
				var words = searchService.GetResults(_providerSettings.IndexPath, segment.ToPlain());
				foreach (var word in words)
				{
					var searchRes = searchService.CreateSearchResult(word, segment.ToPlain());
					wordResults.Add(searchRes);
				}
				searchResults.SourceSegment = segment.Duplicate();
				searchResults.Results.AddRange(wordResults);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.SearchSegment}: {ex.Message}\n {ex.StackTrace}");
			}
			return searchResults;
		}

		#region Methods which doesn't need to be implemented in this app
		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings) => null;
		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask) => null;
		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings) => null;
		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings) => null;
		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask) => null;
		public SearchResults SearchText(SearchSettings settings, string segment) => null;
		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit) => null;
		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits) => null;
		#endregion

		#endregion
	}
}
