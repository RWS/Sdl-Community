using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLog;
using Sdl.Community.DeepLMTProvider.Model;
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
        private static string _apiKey;
        private List<string> _supportedSourceLanguages;

        public DeepLTranslationProviderClient(string key)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            ApiKey = key;
        }

        public static event Action ApiKeyChanged;

        public static string ApiKey
        {
            get => _apiKey;
            set
            {
                _apiKey = value;
                OnApiKeyChanged();
            }
        }

        public static ApiVersion ApiVersion { get; set; }
        public static HttpResponseMessage IsApiKeyValidResponse { get; private set; }
        private static string ChosenBaseUrl => ApiVersion == ApiVersion.V1 ? Constants.BaseUrlV1 : Constants.BaseUrlV2;
        private static List<string> SupportedTargetLanguages { get; set; }
        private static Dictionary<string, bool> SupportedTargetLanguagesAndFormalities { get; set; }

        private List<string> SupportedSourceLanguages =>
                    _supportedSourceLanguages ??= GetSupportedSourceLanguages(ApiKey);

        public static List<string> GetSupportedSourceLanguages(string apiKey)
        {
            var supportedLanguages = new List<string>();
            try
            {
                var response = GetSupportedLanguages("source", apiKey);
                supportedLanguages = JArray.Parse(response)
                    .Select(item => item["language"].ToString().ToUpperInvariant()).ToList();
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
                var response = GetSupportedLanguages("target", apiKey);
                supportedLanguages =
                    JArray.Parse(response).ToDictionary(
                        item => item["language"].ToString().ToUpperInvariant(),
                        item => bool.Parse(item["supports_formality"].ToString()));
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex}");
            }

            return supportedLanguages;
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
            deepLSettings.Formality = GetFormality(languageDirection, deepLSettings.Formality);

            var targetLanguage = GetLanguage(languageDirection.TargetCulture, SupportedTargetLanguages);
            var sourceLanguage = GetLanguage(languageDirection.SourceCulture, SupportedSourceLanguages);
            var translatedText = string.Empty;

            try
            {
                var deeplRequestParameters = new DeeplRequestParameters
                {
                    Text = [sourceText],
                    SourceLanguage = sourceLanguage,
                    TargetLanguage = targetLanguage,
                    Formality = deepLSettings.Formality.ToString().ToLower(),
                    GlossaryId = deepLSettings.GlossaryId,
                    PreserveFormatting = deepLSettings.PreserveFormatting,
                    TagHandling = deepLSettings.TagHandling == TagFormat.None ? null : deepLSettings.TagHandling.ToString().ToLower()
                };

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

        private static string GetSupportedLanguages(string type, string apiKey)
        {
            var content = new StringContent($"type={type}" + $"&auth_key={apiKey}", Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var response = AppInitializer.Client.PostAsync($"{ChosenBaseUrl}/languages", content).Result;
            response.EnsureSuccessStatusCode();

            return response.Content?.ReadAsStringAsync().Result;
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

            SupportedTargetLanguagesAndFormalities = GetSupportedTargetLanguages(ApiKey);
            SupportedTargetLanguages = SupportedTargetLanguagesAndFormalities.Keys.ToList();

            ApiKeyChanged?.Invoke();
        }

        private Formality GetFormality(LanguagePair languageDirection, Formality formality)
        {
            if (!SupportedTargetLanguagesAndFormalities.TryGetValue(
                            languageDirection.TargetCulture.RegionNeutralName.ToUpper(), out var supportsFormality))
            {
                SupportedTargetLanguagesAndFormalities.TryGetValue(languageDirection.TargetCulture.ToString().ToUpper(),
                    out supportsFormality);
            }

            return supportsFormality
                ? formality
                : Formality.Default;
        }

        // Get the target language based on availability in DeepL; if we have a flavour use that, otherwise use general culture of that flavour (two letter iso) if available, otherwise return null
        // (e.g. for Portuguese, the leftLanguageTag (pt-PT or pt-BR) should be used, so the translations will correspond to the specific language flavor)
        private string GetLanguage(CultureInfo culture, List<string> languageList)
        {
            if (languageList != null && languageList.Any())
            {
                var leftLangTag = culture.IetfLanguageTag.ToUpperInvariant();
                var twoLetterIso = culture.TwoLetterISOLanguageName.ToUpperInvariant();

                var selectedTargetLanguage = languageList.FirstOrDefault(tl => tl == leftLangTag) ?? languageList.FirstOrDefault(tl => tl == twoLetterIso);

                return selectedTargetLanguage ?? (languageList.Any(tl => tl.Contains(twoLetterIso)) ? twoLetterIso : null);
            }

            return string.Empty;
        }
    }
}