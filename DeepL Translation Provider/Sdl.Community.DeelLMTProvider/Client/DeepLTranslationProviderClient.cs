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
        private const int MaxBatchSizeBytes = 128 * 1024;

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

        private static string ChosenBaseUrl => ApiVersion?.Contains("V1") ?? true ? Constants.BaseUrlV1 : Constants.BaseUrlV2;

        public (string Translation, string ErrorMessage) Translate(LanguagePair languageDirection, string sourceText, DeepLSettings deepLSettings)
        {
            var (sourceLanguage, _, _) = LanguageValidationService.GetDeepLLanguageCode(languageDirection.SourceCulture, true);
            var (targetLanguage, _, _) = LanguageValidationService.GetDeepLLanguageCode(languageDirection.TargetCulture, false);

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
                    TagHandlingVersion = "v2",
                    TranslationMemoryId = deepLSettings.TranslationMemoryId
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
                var inner = ex is AggregateException aEx ? aEx.InnerExceptions.FirstOrDefault() ?? ex : ex;
                errorMessage = inner.Message;
            }

            return (null, errorMessage);
        }

        public IReadOnlyList<(string Translation, string ErrorMessage)> TranslateBatch(
            LanguagePair languageDirection,
            IReadOnlyList<string> sourceTexts,
            DeepLSettings deepLSettings)
        {
            var (sourceLanguage, _, _) = LanguageValidationService.GetDeepLLanguageCode(languageDirection.SourceCulture, true);
            var (targetLanguage, _, _) = LanguageValidationService.GetDeepLLanguageCode(languageDirection.TargetCulture, false);

            var modelType = deepLSettings.ModelType == ModelType.Not_Supported
                ? ModelType.Quality_Optimized.ToString().ToLowerInvariant()
                : deepLSettings.ModelType.ToString().ToLowerInvariant();

            var tagHandling = deepLSettings.TagHandling == TagFormat.None
                ? null
                : deepLSettings.TagHandling.ToString().ToLowerInvariant();

            var formality = deepLSettings.Formality == Formality.Not_Supported
                ? null
                : deepLSettings.Formality.ToString().ToLowerInvariant();

            var results = new (string Translation, string ErrorMessage)[sourceTexts.Count];
            var batchStartIndex = 0;

            foreach (var batch in BuildBatches(sourceTexts))
            {
                var deeplRequestParameters = new DeeplRequestParameters
                {
                    Text = batch,
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
                    TagHandlingVersion = "v2",
                    TranslationMemoryId = deepLSettings.TranslationMemoryId
                };

                ApplyDeepLRestrictions(deeplRequestParameters);

                try
                {
                    var response = Translate(deeplRequestParameters);
                    var responseBody = response.Content?.ReadAsStringAsync().Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = !string.IsNullOrWhiteSpace(responseBody) ? responseBody : response.ReasonPhrase;
                        for (var i = 0; i < batch.Count; i++)
                            results[batchStartIndex + i] = (null, errorMessage);
                    }
                    else
                    {
                        var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(responseBody);
                        for (var i = 0; i < batch.Count; i++)
                        {
                            var translation = translatedObject?.Translations?.Count > i
                                ? translatedObject.Translations[i].Text
                                : null;
                            results[batchStartIndex + i] = (translation, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var inner = ex is AggregateException aEx ? aEx.InnerExceptions.FirstOrDefault() ?? ex : ex;
                    for (var i = 0; i < batch.Count; i++)
                        results[batchStartIndex + i] = (null, inner.Message);
                }

                batchStartIndex += batch.Count;
            }

            return results;
        }

        private static void ApplyDeepLRestrictions(DeeplRequestParameters deeplRequestParameters)
        {
            deeplRequestParameters.TagHandlingVersion =
                deeplRequestParameters.ModelType == "latency_optimized" ? "v1" : "v2";
        }

        private static IEnumerable<List<string>> BuildBatches(IReadOnlyList<string> sourceTexts)
        {
            var currentBatch = new List<string>();
            var currentBatchSize = 0;

            foreach (var text in sourceTexts)
            {
                var textSize = Encoding.UTF8.GetByteCount(text ?? string.Empty);

                if (currentBatch.Count > 0 && currentBatchSize + textSize > MaxBatchSizeBytes)
                {
                    yield return currentBatch;
                    currentBatch = new List<string>();
                    currentBatchSize = 0;
                }

                currentBatch.Add(text);
                currentBatchSize += textSize;
            }

            if (currentBatch.Count > 0)
                yield return currentBatch;
        }

        private static HttpResponseMessage IsValidApiKey() =>
            AppInitializer.Client.GetAsync($"{ChosenBaseUrl}/usage").Result;

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