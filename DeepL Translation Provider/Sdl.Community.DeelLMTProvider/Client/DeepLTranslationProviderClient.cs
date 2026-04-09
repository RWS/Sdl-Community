using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Service;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
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

        public (string Translation, string ErrorMessage) Translate(LanguagePair languageDirection, string sourceText, DeepLSettings deepLSettings)
        {
            var (sourceLanguage, _, _) = LanguageValidationService.GetDeepLLanguageCode(languageDirection.TargetCulture, true);
            var (targetLanguage, _,_) = LanguageValidationService.GetDeepLLanguageCode(languageDirection.SourceCulture, false);

            string errorMessage = null;
            try
            {
                var modelType = deepLSettings.ModelType == ModelType.Not_Supported
                    ? ModelType.Quality_Optimized.ToString().ToLowerInvariant()
                    : deepLSettings.ModelType.ToString().ToLowerInvariant();

                var tagHandling = deepLSettings.TagHandling == TagFormat.None
                    ? null
                    : deepLSettings.TagHandling.ToString().ToLowerInvariant();

                var formality = deepLSettings.Formality == Formality.Not_Supported
                    ? null
                    : deepLSettings.Formality.ToString().ToLowerInvariant();

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
                    StyleId = deepLSettings.StyleId,
                    TagHandlingVersion = "v2"
                };

                ApplyDeepLRestrictions(deeplRequestParameters);

                var response = Translate(deeplRequestParameters);

                var translationResponse = response.Content?.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                    return
                    (
                        null,
                        !string.IsNullOrWhiteSpace(translationResponse) ? translationResponse : response.ReasonPhrase
                    );

                var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(translationResponse);

                if (translatedObject != null && translatedObject.Translations.Any())
                    return (translatedObject.Translations[0].Text, null);
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aEx) ex = aEx.InnerException;
                errorMessage = ex?.Message;
            }

            return (null, errorMessage);
        }

        private static void ApplyDeepLRestrictions(DeeplRequestParameters deeplRequestParameters)
        {
            deeplRequestParameters.TagHandlingVersion =
                deeplRequestParameters.ModelType == "latency_optimized" ? "v1" : "v2";
        }

        private static HttpResponseMessage IsValidApiKey()
        {
            return AppInitializer.Client.GetAsync($"{ChosenBaseUrl}/usage").Result;
        }

        private static void OnApiKeyChanged()
        {
            AppInitializer.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DeepL-Auth-Key", ApiKey);
            IsApiKeyValidResponse = IsValidApiKey();
            LanguageClientV3.ClearCache();
        }

        private static HttpResponseMessage Translate(DeeplRequestParameters deeplRequestParameters)
        {
            var requestJson = JsonConvert.SerializeObject(
                deeplRequestParameters,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            var request = new HttpRequestMessage
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{ChosenBaseUrl}/translate")
            };

            var response = AppInitializer.Client.SendAsync(request).Result;
            return response;
        }
    }
}