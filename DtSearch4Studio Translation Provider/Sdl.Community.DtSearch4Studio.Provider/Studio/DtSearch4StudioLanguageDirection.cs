using System.Globalization;
using Sdl.Community.DtSearch4Studio.Provider.Model;
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
		public CultureInfo SourceLanguage { get; }
		public CultureInfo TargetLanguage { get; }
		public bool CanReverseLanguageDirection { get; }
		public ITranslationProvider TranslationProvider => _dtSearch4StudioProvider;
		#endregion

		#region Public Methods

		// To be implemented
		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			return null;
		}

		// To be implemented
		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			return SearchSegment(settings, translationUnit.SourceSegment);
		}

		// To be implemented
		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			return null;
		}

		// To be implemented
		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			return null;
		}

		// To be implemented
		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			return null;
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
