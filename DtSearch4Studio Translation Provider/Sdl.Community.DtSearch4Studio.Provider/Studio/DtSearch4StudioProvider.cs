using System;
using Sdl.Community.DtSearch4Studio.Provider.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DtSearch4Studio.Provider.Studio
{
	class DtSearch4StudioProvider : ITranslationProvider
	{
		#region Private fields
		private ProviderSettings _providerSettings;
		#endregion

		#region Constructors
		public DtSearch4StudioProvider(ProviderSettings providerSettings)
		{
			UpdateSettings(providerSettings);
		}
		#endregion

		// To be implemented all the methods /properties bellow. 
		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new DtSearch4StudioLanguageDirection(this, languageDirection);
		}

		public bool IsReadOnly => true;
		public string Name => "DtSearch4Studio Translation Provider";
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
		public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;
		public Uri Uri => new Uri("https://DtSearch4Studio.com");

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


		#region Private Methods
		public void UpdateSettings(ProviderSettings providerSettings)
		{
			_providerSettings = providerSettings;
		}
		#endregion
	}
}