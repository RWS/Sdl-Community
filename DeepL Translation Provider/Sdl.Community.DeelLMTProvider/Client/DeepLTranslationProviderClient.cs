using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Sdl.Community.DeepLMTProvider.Client
{
    public class DeepLTranslationProviderClient
    {
        private static readonly Logger Logger = Log.GetLogger(nameof(DeepLTranslationProviderClient));

        public DeepLTranslationProviderClient(string key)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            ApiKey = key;
        }

        public static event Action ApiKeyChanged;

        public static string ApiKey
        {
            get;
            set
            {
                field = value;
                OnApiKeyChanged();
            }
        }

        public static string ApiVersion { get; set; }

        public static HttpResponseMessage IsApiKeyValidResponse { get; private set; }

        private static Dictionary<string, List<string>> ChineseMappings { get; set; } = new()
        {
            ["ZH-HANS"] = ["ZH-CN", "ZH-SG", "ZH-HANS-HK", "ZH-HANS-MO"],
            ["ZH-HANT"] = ["ZH-TW", "ZH-HK", "ZH-MO"]
        };

        private static string ChosenBaseUrl => ApiVersion?.Contains("V1") ?? true ? Constants.BaseUrlV1 : Constants.BaseUrlV2;

        private static List<string> SupportedSourceLanguages { get; set; }
        private static Dictionary<string, bool> SupportedSourceLanguagesAndFormalities { get; set; }
        private static List<string> SupportedTargetLanguages { get; set; }

        private static Dictionary<string, bool> SupportedTargetLanguagesAndFormalities { get; set; }

        // Get the target language based on availability in DeepL; if we have a flavour use that, otherwise use general culture of that flavour (two letter iso) if available, otherwise return null
        // (e.g. for Portuguese, the leftLanguageTag (pt-PT or pt-BR) should be used, so the translations will correspond to the specific language flavor)
        public static string GetLanguage(CultureInfo culture, List<string> languageList, bool isTarget = false)
        {
            var ietfLanguageTag = culture.IetfLanguageTag.ToUpperInvariant();
            if (isTarget && ietfLanguageTag.Contains("ZH"))
                return GetChineseFlavour(ietfLanguageTag);

            if (languageList == null || !languageList.Any())
                return string.Empty;

            var twoLetterIso = culture.TwoLetterISOLanguageName.ToUpperInvariant();

            var selectedTargetLanguage = languageList.FirstOrDefault(tl => tl == ietfLanguageTag) ?? languageList.FirstOrDefault(tl => tl == twoLetterIso);
            return selectedTargetLanguage ?? (languageList.Any(tl => tl.Contains(twoLetterIso)) ? twoLetterIso : null);
        }

        public static Dictionary<string, bool> GetSupportedSourceLanguages(string apiKey)
        {
            var supportedLanguages = new Dictionary<string, bool>();
            try
            {
                var response = LanguageClient.GetSupportedLanguages("source", apiKey, ChosenBaseUrl);
                supportedLanguages =
                    response.ToDictionary(
                        item => item.Language.ToUpperInvariant(),
                        item => item.SupportsOptions);
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex}");
            }

            return supportedLanguages;
        }

        public static Dictionary<string, bool> GetSupportedTargetLanguages(string apiKey)
        {
            var supportedLanguages = new Dictionary<string, bool>();
            try
            {
                var response = LanguageClient.GetSupportedLanguages("target", apiKey, ChosenBaseUrl);
                supportedLanguages =
                    response.ToDictionary(
                        item => item.Language.ToUpperInvariant(),
                        item => item.SupportsOptions);
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex}");
            }

            return supportedLanguages;
        }

        public static bool SupportsAllModelTypes(LanguagePair languagePair)
        {
            var sourceLanguage = GetLanguage(languagePair.SourceCulture, SupportedSourceLanguages);
            var targetLanguage = GetLanguage(languagePair.TargetCulture, SupportedTargetLanguages, true);

            return SupportedSourceLanguagesAndFormalities.TryGetValue(
                sourceLanguage, out var supportsOptions) && SupportedTargetLanguagesAndFormalities.TryGetValue(
                targetLanguage, out supportsOptions) && supportsOptions;
        }

        public static bool SupportsFormality(CultureCode cultureCode)
        {
            var targetLanguage = GetLanguage(cultureCode, SupportedTargetLanguages, true);
            return SupportedTargetLanguagesAndFormalities.TryGetValue(
                targetLanguage, out var supportsOptions) && supportsOptions;
        }

        public bool IsLanguagePairSupported(CultureInfo sourceCulture, CultureInfo targetCulture)
        {
            var supportedSourceLanguage = GetLanguage(sourceCulture, SupportedSourceLanguages);
            // do not make a call again to the server if source languages are not supported, because the return condition requires both source and target languages to be supported
            var supportedTargetLanguage = !string.IsNullOrEmpty(supportedSourceLanguage)
                ? GetLanguage(targetCulture, SupportedTargetLanguages)
                : string.Empty;

            return !string.IsNullOrEmpty(supportedSourceLanguage) && !string.IsNullOrEmpty(supportedTargetLanguage);
        }

        public string Translate(LanguagePair languageDirection, string sourceText, DeepLSettings deepLSettings)
        {
            var targetLanguage = GetLanguage(languageDirection.TargetCulture, SupportedTargetLanguages, true);
            var sourceLanguage = GetLanguage(languageDirection.SourceCulture, SupportedSourceLanguages);
            var translatedText = string.Empty;

            try
            {
                var modelType = deepLSettings.ModelType == ModelType.Not_Supported
                    ? ModelType.Quality_Optimized.ToString().ToLower()
                    : deepLSettings.ModelType.ToString().ToLower();

                var tagHandling = deepLSettings.TagHandling == TagFormat.None
                    ? null
                    : deepLSettings.TagHandling.ToString().ToLower();

                var formality = deepLSettings.Formality == Formality.Not_Supported
                    ? null
                    : deepLSettings.Formality.ToString().ToLower();

                var deeplRequestParameters = new DeeplRequestParameters
                {
                    Text = [sourceText],
                    SourceLanguage = sourceLanguage,
                    TargetLanguage = targetLanguage,
                    Formality = formality,
                    GlossaryId = deepLSettings.GlossaryId,
                    PreserveFormatting = deepLSettings.PreserveFormatting,
                    TagHandling = tagHandling,
                    SplittingSentenceHandling = deepLSettings.SplitSentencesHandling.GetApiValue(),
                    IgnoreTags = deepLSettings.IgnoreTags,
                    ModelType = modelType,
                    StyleId = deepLSettings.StyleId
                };

                var response = Translate(deeplRequestParameters);
                response.EnsureSuccessStatusCode();

                var translationResponse = response.Content?.ReadAsStringAsync().Result;
                var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(translationResponse);

                if (translatedObject != null && translatedObject.Translations.Any())
                {
                    translatedText = translatedObject.Translations[0].Text;
                }
            }
            catch (AggregateException aEx)
            {
                foreach (var innerEx in aEx.InnerExceptions)
                {
                    Logger.Error(innerEx);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }

            return translatedText;
        }

        private static string GetChineseFlavour(string languageName)
        {
            return ChineseMappings.FirstOrDefault(m => m.Value.Contains(languageName)).Key;
        }

        private static HttpResponseMessage IsValidApiKey(string apiKey)
        {
            return AppInitializer.Client.GetAsync($"{ChosenBaseUrl}/usage?auth_key={apiKey}").Result;
        }

        private static void OnApiKeyChanged()
        {
            IsApiKeyValidResponse = IsValidApiKey(ApiKey);

            if (!IsApiKeyValidResponse.IsSuccessStatusCode)
                return;

            if (SupportedTargetLanguagesAndFormalities is { Count: not 0 })
            {
                ApiKeyChanged?.Invoke();
                return;
            }

            SupportedSourceLanguagesAndFormalities = GetSupportedSourceLanguages(ApiKey);
            SupportedSourceLanguages = SupportedSourceLanguagesAndFormalities.Keys.ToList();

            SupportedTargetLanguagesAndFormalities = GetSupportedTargetLanguages(ApiKey);
            SupportedTargetLanguages = SupportedTargetLanguagesAndFormalities.Keys.ToList();

            ApiKeyChanged?.Invoke();
        }

        private static HttpResponseMessage Translate(DeeplRequestParameters deeplRequestParameters)
        {
            deeplRequestParameters.TagHandlingVersion =
                deeplRequestParameters.ModelType == "latency_optimized" ? "v1" : "v2";
            var requestJson = JsonConvert.SerializeObject(
                deeplRequestParameters,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage
            {
                Content = content,
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{ChosenBaseUrl}/translate")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("DeepL-Auth-Key", ApiKey);

            var response = AppInitializer.Client.SendAsync(request).Result;
            return response;
        }
    }
}