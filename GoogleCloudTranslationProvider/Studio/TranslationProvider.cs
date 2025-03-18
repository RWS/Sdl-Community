using System;
using System.Collections.Generic;
using System.Linq;
using GoogleCloudTranslationProvider.Extensions;
using GoogleCloudTranslationProvider.GoogleAPI;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interface;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace GoogleCloudTranslationProvider.Studio
{
	public class TranslationProvider : ITranslationProvider, ITranslationProviderExtension
	{
		private readonly HtmlUtil _htmlUtil;

		private V2Connector _googleV2Api;
		private V3Connector _googleV3Api;

		public TranslationProvider(ITranslationOptions options)
		{
			Options = options;
			_htmlUtil = new HtmlUtil();
			LanguagesSupported = Options.LanguagesSupported.ToDictionary(lang => lang, lang => PluginResources.LanguageSupported_ShortName);
		}

		public ITranslationOptions Options { get; set; }

		public string Name => Options.ProviderName;

		public ProviderStatusInfo StatusInfo => new(true, Constants.GoogleNaming_FullName);
		
		public bool SupportsSearchForTranslationUnits => true;
		
		public bool SupportsTaggedInput => true;
		
		public bool SupportsTranslation => true;
		
		public bool SupportsPenalties => true;
		
		public bool IsReadOnly => true;
		
		public bool SupportsSourceConcordanceSearch => false;
		
		public bool SupportsTargetConcordanceSearch => false;
		
		public bool SupportsConcordanceSearch => false;
		
		public bool SupportsDocumentSearches => false;
		
		public bool SupportsStructureContext => false;
		
		public bool SupportsMultipleResults => false;
		
		public bool SupportsFuzzySearch => false;
		
		public bool SupportsPlaceables => false;
		
		public bool SupportsWordCounts => false;
		
		public bool SupportsFilters => false;
		
		public bool SupportsScoring => false;
		
		public bool SupportsUpdate => false;

		public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;

		public Uri Uri => Options.Uri;

		public Dictionary<string, string> LanguagesSupported { get; set; } = new Dictionary<string, string>();

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			DatabaseExtensions.CreateDatabase(Options);
			if (Options.SelectedGoogleVersion is not ApiVersion.V2)
			{
				_googleV3Api ??= new V3Connector(Options);
				return _googleV3Api.IsSupportedLanguage(languageDirection.SourceCulture, languageDirection.TargetCulture);
			}

			if (_googleV2Api is null)
			{
				_googleV2Api = new V2Connector(Options.ApiKey, _htmlUtil);
			}
			else
			{
				_googleV2Api.ApiKey = Options.ApiKey;
			}

			return _googleV2Api.IsSupportedLanguagePair(languageDirection.SourceCulture, languageDirection.TargetCulture);
		}

		public void LoadState(string translationProviderState)
		{
			Options = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);
		}

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new TranslationProviderLanguageDirection(this, languageDirection, _htmlUtil);
		}

		public string SerializeState()
		{
			return JsonConvert.SerializeObject(Options);
		}

		public void RefreshStatusInfo() { }
	}
}