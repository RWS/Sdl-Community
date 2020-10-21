using System;
using Newtonsoft.Json;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DeepLMTProvider
{
	public class DeepLMtTranslationProvider : ITranslationProvider
	{
		public DeepLTranslationProviderConnecter DeepLTranslationProviderConnecter { get; }
		public static readonly string ListTranslationProviderScheme = "deepltranslationprovider";
		
		public DeepLMtTranslationProvider(DeepLTranslationOptions options, DeepLTranslationProviderConnecter deepLTranslationProviderConnecter, LanguagePair[] languagePairs = null)
		{
			DeepLTranslationProviderConnecter = deepLTranslationProviderConnecter;
			Options = options;

			if (languagePairs != null)
			{
				GetSupportedTargetLanguages(languagePairs);
			}
		}

		public bool IsReadOnly => true;

		public string Name => "DeepL Translator provider using DeepL Translator ";

		public DeepLTranslationOptions Options
		{
			get;
			set;
		}

		private void GetSupportedTargetLanguages(LanguagePair[] languagePairs)
		{
			foreach (var languagePair in languagePairs)
			{
				var targetLanguage = languagePair.TargetCulture.TwoLetterISOLanguageName.ToUpper();
				if (DeepLTranslationProviderConnecter.IsLanguagePairSupported(languagePair.SourceCulture, languagePair.TargetCulture) && !Options.LanguagesSupported.ContainsKey(targetLanguage))
				{
					if (!Options.LanguagesSupported.ContainsKey(languagePair.TargetCultureName))
					{
						Options.LanguagesSupported.Add(languagePair.TargetCultureName, "DeepLTranslator");
					}
				}
			}
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

		// Check if LanguageDirection is supported (if true, the provider is added in Studio)
		// 'IsInvalidServerMessage' is used to check if any server error was returned in the first call, if yes, then is no need to make the second call to 
		// the server to verify if language pairs are supported. (2 calls of SupportsLanguageDirection() are made because of twice instantiation of DeepLTranslationProviderConnecter. 
		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			
			if (!Helpers.IsInvalidServerMessage)
			{
				return DeepLTranslationProviderConnecter.IsLanguagePairSupported(languageDirection.SourceCulture, languageDirection.TargetCulture);
			}

			return false;
		}
	}
}