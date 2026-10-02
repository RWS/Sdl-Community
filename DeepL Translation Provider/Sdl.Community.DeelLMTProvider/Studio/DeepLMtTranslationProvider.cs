using Newtonsoft.Json;
using NLog;
using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Studio
{
    public class DeepLMtTranslationProvider : ITranslationProvider, ITranslationProviderExtension
    {
        public static readonly string ListTranslationProviderScheme = "deepltranslationprovider";
        private readonly Logger _logger = Log.GetLogger(nameof(DeepLTranslationProviderClient));

        public DeepLMtTranslationProvider(DeepLTranslationOptions options, DeepLTranslationProviderClient deepLTranslationProviderConnecter)
        {
            DeepLTranslationProviderConnecter = deepLTranslationProviderConnecter;
            Options = options;
        }

        public DeepLTranslationProviderClient DeepLTranslationProviderConnecter { get; }
        public bool IsReadOnly => true;
        public Dictionary<string, string> LanguagesSupported { get; set; } = new Dictionary<string, string>();
        public string Name => "DeepL Translator provider using DeepL Translator ";

        public DeepLTranslationOptions Options
        {
            get;
            set;
        }

        public ProviderStatusInfo StatusInfo => new ProviderStatusInfo(true, "Deepl");
        public bool SupportsConcordanceSearch => false;

        public bool SupportsSearchForTranslationUnits => true;
        public bool SupportsSourceConcordanceSearch => false;
        public bool SupportsTargetConcordanceSearch => false;
        public bool SupportsUpdate => false;
        public TranslationMethod TranslationMethod => TranslationMethod.MachineTranslation;
        public Uri Uri => Options.Uri;

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            return new DeepLMtTranslationProviderLanguageDirection(this, languageDirection, DeepLTranslationProviderConnecter);
        }

        public void LoadState(string translationProviderState)
        {
            Options = JsonConvert.DeserializeObject<DeepLTranslationOptions>(translationProviderState);
        }

        public void RefreshStatusInfo()
        { }

        public string SerializeState() => JsonConvert.SerializeObject(Options);

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            try
            {
                var (sourceLangCode, _, _) = LanguageValidationService.GetDeepLLanguageCode(languageDirection.SourceCulture, true);
                var (targetLangCode, _, _) = LanguageValidationService.GetDeepLLanguageCode(languageDirection.TargetCulture, false);
                return
                    Task.Run(() => LanguageClientV3
                            .IsLanguageSupportedAsync(sourceLangCode, "source", DeepLTranslationProviderClient.ApiKey))
                        .GetAwaiter().GetResult() &&
                    Task.Run(() => LanguageClientV3
                            .IsLanguageSupportedAsync(targetLangCode, "target", DeepLTranslationProviderClient.ApiKey))
                        .GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _logger.Error($"Error for following LP: source {languageDirection.SourceCultureName} and target {languageDirection.TargetCultureName}");
                _logger.Error(e);
            }

            return false;
        }
    }
}