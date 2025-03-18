using System;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Studio
{
    public class DeepLMtTranslationProvider : ITranslationProvider, ITranslationProviderExtension
    {
        public static readonly string ListTranslationProviderScheme = "deepltranslationprovider";
        private readonly Logger _logger = Log.GetLogger(nameof(Client.DeepLTranslationProviderClient));

        public DeepLMtTranslationProvider(DeepLTranslationOptions options, DeepLTranslationProviderClient deepLTranslationProviderConnecter, LanguagePair[] languagePairs = null)
        {
            DeepLTranslationProviderConnecter = deepLTranslationProviderConnecter;
            Options = options;

            if (languagePairs != null)
            {
                GetSupportedTargetLanguages(languagePairs);
            }

            LanguagesSupported = Options.LanguagesSupported;
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
        public DeepLTranslationProviderClient DeepLTranslationProviderConnecter { get; }
        public Dictionary<string, string> LanguagesSupported { get; set; } = new Dictionary<string, string>();

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            return new DeepLMtTranslationProviderLanguageDirection(this, languageDirection, DeepLTranslationProviderConnecter);
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
            try
            {
                return DeepLTranslationProviderConnecter.IsLanguagePairSupported(languageDirection.SourceCulture, languageDirection.TargetCulture);
            }
            catch (Exception e)
            {
                _logger.Error($"Error for following LP: source {languageDirection.SourceCultureName} and target {languageDirection.TargetCultureName}");
                _logger.Error(e);
            }

            return false;
        }

        private void GetSupportedTargetLanguages(LanguagePair[] languagePairs)
        {
            foreach (var languagePair in languagePairs)
            {
                var targetLanguage = languagePair.TargetCulture.RegionNeutralName.ToUpper();
                if (DeepLTranslationProviderConnecter.IsLanguagePairSupported(languagePair.SourceCulture, languagePair.TargetCulture) && !Options.LanguagesSupported.ContainsKey(targetLanguage))
                {
                    if (!Options.LanguagesSupported.ContainsKey(languagePair.TargetCultureName))
                    {
                        Options.LanguagesSupported.Add(languagePair.TargetCultureName, "DeepLTranslator");
                    }
                }
            }
        }
    }
}