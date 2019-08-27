using System;
using Sdl.Community.DtSearch4Studio.Provider.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DtSearch4Studio.Provider.Studio
{
	class DtSearch4StudioProvider : ITranslationProvider
	{
		#region Private fields
		#endregion
		

		#region Constructors
		public DtSearch4StudioProvider(ProviderSettings providerSettings)
		{
			UpdateSettings(providerSettings);
		}
		#endregion

		#region Public properties
		public bool IsReadOnly => true;
		public string Name => "DtSearch4StudioTranslationProvider";
		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, "SDL DtSearch4Studio");
		public bool SupportsConcordanceSearch => true;
		public bool SupportsDocumentSearches => true;
		public bool SupportsFilters => true;
		public bool SupportsFuzzySearch => true;
		public bool SupportsLanguageDirection(LanguagePair languageDirection) => true;
		public bool SupportsMultipleResults => true;
		public bool SupportsPenalties => true;
		public bool SupportsPlaceables => true;
		public bool SupportsScoring => true;
		public bool SupportsSearchForTranslationUnits => true;
		public bool SupportsSourceConcordanceSearch => true;
		public bool SupportsStructureContext => true;
		public bool SupportsTaggedInput => true;
		public bool SupportsTargetConcordanceSearch => true;
		public bool SupportsTranslation => true;
		public bool SupportsUpdate => true;
		public bool SupportsWordCounts => true;
		public TranslationMethod TranslationMethod => TranslationMethod.Other;
		public Uri Uri => new Uri("dtsearch://dtSearch4StudioTranslationProvider");

		public ProviderSettings ProviderSettings { get; set; }
		#endregion

		#region Public methods
		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new DtSearch4StudioLanguageDirection(this, languageDirection);
		}		

        public void LoadState(string translationProviderState)
		{
		}

		public void RefreshStatusInfo()
		{			
		}

		public string SerializeState()
		{
			return null;
		}
		
		public void UpdateSettings(ProviderSettings providerSettings)
		{
			ProviderSettings = providerSettings;
		}
		#endregion
	}
}