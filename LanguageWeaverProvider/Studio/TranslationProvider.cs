using System;
using System.Linq;
using LanguageWeaverProvider.LanguageMappingProvider;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider
{
	internal class TranslationProvider : ITranslationProvider
	{
		private readonly ITranslationProviderCredentialStore _credentialStore;
		public TranslationProvider(ITranslationOptions translationOptions, ITranslationProviderCredentialStore credentialStore = null)
		{
			_credentialStore = credentialStore;
			TranslationOptions = translationOptions;
			_ = DatabaseControl.InitializeDatabase(translationOptions.Version);
		}

		public string Name => Constants.PluginName;

		public ITranslationOptions TranslationOptions { get; set; }

		public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

		public bool SupportsSearchForTranslationUnits => true;
		
		public bool SupportsTaggedInput => true;
		
		public bool SupportsTranslation => true;
		
		public bool SupportsPenalties => true;
		
		public bool SupportsScoring => true;
		
		public bool IsReadOnly => true;
		
		public bool SupportsConcordanceSearch => false;
		
		public bool SupportsDocumentSearches => false;
		
		public bool SupportsSourceConcordanceSearch => false;
		
		public bool SupportsTargetConcordanceSearch => false;
		
		public bool SupportsStructureContext => false;
		
		public bool SupportsMultipleResults => false;
		
		public bool SupportsFuzzySearch => false;
		
		public bool SupportsPlaceables => false;
		
		public bool SupportsWordCounts => false;
		
		public bool SupportsFilters => false;
		
		public bool SupportsUpdate => false;

		public ProviderStatusInfo StatusInfo => new(true, Constants.PluginName);

		public Uri Uri => TranslationOptions.Uri;

		public void LoadState(string translationProviderState)
		{
			var translationOptions = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
			TranslationOptions = translationOptions;
		}
		public string SerializeState()
		{
			var translationProviderState = JsonConvert.SerializeObject(TranslationOptions);
			return translationProviderState;
		}

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			var currentPair = TranslationOptions.PairMappings.FirstOrDefault(pair => pair.LanguagePair.SourceCulture.Name == languageDirection.SourceCulture.Name
																				  && pair.LanguagePair.TargetCulture.Name == languageDirection.TargetCulture.Name);
			try
			{
				return !string.IsNullOrEmpty(currentPair.SourceCode)
					&& !string.IsNullOrEmpty(currentPair.TargetCode)
					&& !currentPair.SelectedModel.DisplayName.Equals(PluginResources.PairModel_Model_Unavailable);
			}
			catch
			{
				return false;
			}
		}

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new TranslationProviderLanguageDirection(this, TranslationOptions, languageDirection, _credentialStore);
		}

		public void RefreshStatusInfo() { }
	}
}