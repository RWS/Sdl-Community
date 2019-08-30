using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
		#endregion

		#region Public Methods

		// To be implemented: the method from where the search begins
		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments, bool[] mask)
		{			
			var results = new List<SearchResult>();
			var searchService = new SearchService(SourceLanguage, TargetLanguage);
			for (int i = 0; i < segments.Length; i++)
			{
				var words = searchService.GetResults(_providerSettings.IndexPath, segments[i].ToPlain());
				foreach (var word in words)
				{
					var searchRes = searchService.CreateSearchResult(word, segments[i].ToPlain());
					results.Add(searchRes);
				}
			}
			// process all the results returned from each segment + each word returned per segment:eg: 2 segments, 3 words per each segment => total of search results=2*3=6
			var searchResults = new SearchResults[results.Count];
			for (int i = 0; i < results.Count; i++)
			{
				searchResults[i] = new SearchResults();
				searchResults[i].Add(results[i]);
			}
			return searchResults;
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
		
		#region Methods which doesn't need to be implemented in this app
		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings) => null;
		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask) => null;
		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings) => null;
		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings) => null;
		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask) => null;
		public SearchResults SearchSegment(SearchSettings settings, Segment segment) => null;
		public SearchResults SearchText(SearchSettings settings, string segment) => null;
		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit) => null;
		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits) => null;
		#endregion

		#endregion
	}
}
