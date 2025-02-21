using System;
using System.Collections.Generic;
using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider
{
	public class TranslationProvider : ITranslationProvider
	{

		public TranslationProvider(ITranslationOptions translationOptions)
		{
			TranslationOptions = translationOptions;
		}

		public string Name => TranslationOptions.ProviderName;

        public List<UrlMetadata> PrivateHeaders { get; set; }

		public Uri Uri => TranslationOptions.Uri;

		public ITranslationOptions TranslationOptions { get; set; }

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

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			if (TranslationOptions.AuthenticationType == AuthenticationType.PrivateEndpoint)
			{
				return true;
			}

			foreach (var pairMapped in TranslationOptions.PairModels)
			{
				if (pairMapped.TradosLanguagePair.SourceCultureName == languageDirection.SourceCultureName
				 && pairMapped.TradosLanguagePair.TargetCultureName == languageDirection.TargetCultureName)
				{
					return pairMapped.IsSupported;
				}
			}

			return false;
		}

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new TranslationProviderLanguageDirection(this, TranslationOptions, languageDirection);
		}

		public string SerializeState()
		{
			return JsonConvert.SerializeObject(TranslationOptions);
		}

		public void LoadState(string translationProviderState)
		{
			TranslationOptions = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
		}

		public void RefreshStatusInfo() { }
	}
}