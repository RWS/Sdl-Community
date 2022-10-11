using System;
using MTEnhancedMicrosoftProvider.Connect;
using MTEnhancedMicrosoftProvider.Interfaces;
using MTEnhancedMicrosoftProvider.Model;
using MTEnhancedMicrosoftProvider.Service;
using MTEnhancedMicrosoftProvider.Studio.TranslationProvider;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MTEnhancedMicrosoftProvider
{
	public class MTEMicrosoftProvider : ITranslationProvider
    {
		public static readonly string ListTranslationProviderScheme = "mtenhancedprovider";
		private readonly HtmlUtil _htmlUtil;
		private ApiConnecter _mstConnect;


		public MTEMicrosoftProvider(ITranslationOptions options, RegionsProvider regionProvider, HtmlUtil htmlUtil)
		{
			_htmlUtil = htmlUtil;
			Options = options;
			RegionsProvider = regionProvider;
		}


		public string Name => PluginResources.Plugin_Name;

		public RegionsProvider RegionsProvider { get; }

		public ITranslationOptions Options { get; set; }

		public Uri Uri => Options.Uri;

		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, PluginResources.Microsoft_NiceName);

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
			_mstConnect = _mstConnect is null ? new ApiConnecter(Options.ClientId, Options.Region, _htmlUtil)
											  : _mstConnect;
			_mstConnect.ResetCredentials(Options.ClientId, Options.Region);
			return _mstConnect.IsSupportedLangPair(languageDirection.SourceCulture.Name, languageDirection.TargetCulture.Name);
		}

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new MTEMicrosoftProviderLanguageDirection(this, languageDirection, _htmlUtil);
		}

		public string SerializeState()
		{
			return JsonConvert.SerializeObject(Options);
		}

		public void LoadState(string translationProviderState)
		{
			Options = JsonConvert.DeserializeObject<MTEMicrosoftTranslationOptions>(translationProviderState);
		}

		public void RefreshStatusInfo() { }
	}
}