using System;
using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service;
using MicrosoftTranslatorProvider.Studio.TranslationProvider;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider
{
	public class Provider : ITranslationProvider
	{
		private readonly HtmlUtil _htmlUtil;

		private ProviderConnecter _providerConnecter;

		public Provider(ITranslationOptions options, RegionsProvider regionProvider, HtmlUtil htmlUtil)
		{
			_htmlUtil = htmlUtil;
			Options = options;
			RegionsProvider = regionProvider;
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

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			_providerConnecter ??= new ProviderConnecter(Options.ClientID, Options.Region, _htmlUtil);
			_providerConnecter.ResetCredentials(Options.ClientID, Options.Region);
			return _providerConnecter.IsSupportedLanguagePair(languageDirection.SourceCulture.Name, languageDirection.TargetCulture.Name);
		}

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new ProviderLanguageDirection(this, languageDirection, _htmlUtil);
		}

		public string SerializeState()
		{
			return JsonConvert.SerializeObject(Options);
		}

		public void LoadState(string translationProviderState)
		{
			Options = JsonConvert.DeserializeObject<MTETranslationOptions>(translationProviderState);
		}

		public void RefreshStatusInfo() { }
	}
}