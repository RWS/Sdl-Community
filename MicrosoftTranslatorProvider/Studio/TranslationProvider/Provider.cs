using System;
using System.Collections.Generic;
using System.Linq;
using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Studio.TranslationProvider;
using MicrosoftTranslatorProvider.Interface;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider
{
    public class Provider : ITranslationProvider, ITranslationProviderExtension
	{
		private MicrosoftApi _providerConnector;

		public Provider(ITranslationOptions options)
		{
			Options = options;
            LanguagesSupported = Options.LanguagesSupported.ToDictionary(lang => lang, lang => PluginResources.Microsoft_ShortName);
        }

        public string Name
		{
			get
			{
				var customName = Options.CustomProviderName;
				var useCustomName = Options.UseCustomProviderName;
				var providerName = customName.SetProviderName(useCustomName);
				return providerName;
			}
		}

		public List<UrlMetadata> PrivateHeaders { get; set; }

		public RegionsProvider RegionsProvider { get; }

		public ITranslationOptions Options { get; set; }

		public Uri Uri => Options.Uri;

		public ProviderStatusInfo StatusInfo => new(true, PluginResources.Microsoft_NiceName);

		public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

		public bool IsReadOnly => true;

		public bool SupportsConcordanceSearch => false;

		public bool SupportsDocumentSearches => false;

		public bool SupportsFilters => false;

		public bool SupportsFuzzySearch => false;

		public bool SupportsMultipleResults => false;

		public bool SupportsPenalties => true;

		public bool SupportsPlaceables => false;

		public bool SupportsScoring => false;

		public bool SupportsSearchForTranslationUnits => true;

		public bool SupportsSourceConcordanceSearch => false;

		public bool SupportsStructureContext => false;

		public bool SupportsTaggedInput => true;

		public bool SupportsTargetConcordanceSearch => false;

		public bool SupportsTranslation => true;

		public bool SupportsUpdate => false;

		public bool SupportsWordCounts => false;

        public Dictionary<string, string> LanguagesSupported { get; set; } = new Dictionary<string, string>();

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			if (Options.UsePrivateEndpoint)
			{
				return true;
			}

			_providerConnector ??= new MicrosoftApi(Options.ApiKey, Options.Region);
			_providerConnector.ResetCredentials(Options.ApiKey, Options.Region);
			return _providerConnector.IsSupportedLanguagePair(languageDirection.SourceCulture.Name, languageDirection.TargetCulture.Name);
		}

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new ProviderLanguageDirection(this, languageDirection);
		}

		public string SerializeState()
		{
			return JsonConvert.SerializeObject(Options);
		}

		public void LoadState(string translationProviderState)
		{
			Options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
		}

		public void RefreshStatusInfo() { }
	}
}