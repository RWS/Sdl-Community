using System;
using Newtonsoft.Json;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DeepLMTProvider
{
	public class DeepLMtTranslationProvider : ITranslationProvider
	{
		public static readonly string ListTranslationProviderScheme = "deepltranslationprovider";

		public DeepLMtTranslationProvider(DeepLTranslationOptions options)
		{
			Options = options;
		}

		public bool IsReadOnly => true;

		public string Name => "DeepL Translator provider using DeepL Translator ";

		public DeepLTranslationOptions Options
		{
			get;
			set;
		}

		public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, "Deepl");

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
		public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;
		public Uri Uri => Options.Uri;

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new DeepLMtTranslationProviderLanguageDirection(this, languageDirection);
		}

		public void LoadState(string translationProviderState)
		{
			Options = JsonConvert.DeserializeObject<DeepLTranslationOptions>(translationProviderState);
		}

		public void RefreshStatusInfo()
		{
		}

		public string SerializeState()
		{
			return JsonConvert.SerializeObject(Options);
		}

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			return Helpers.IsSupportedLanguagePair(languageDirection.SourceCulture.TwoLetterISOLanguageName.ToUpper(),
				languageDirection.TargetCulture.TwoLetterISOLanguageName.ToUpper());
		}
	}
}