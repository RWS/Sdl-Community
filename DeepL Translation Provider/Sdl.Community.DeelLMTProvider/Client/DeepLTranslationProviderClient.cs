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

        public static HttpResponseMessage IsApiKeyValidResponse { get; private set; }

        private static string BaseUrl => $"{Constants.BaseUrl}/v2";

        public IReadOnlyList<(string Translation, string ErrorMessage)> TranslateBatch(
            LanguagePair languageDirection,
            IReadOnlyList<string> sourceTexts,
            DeepLSettings deepLSettings,
            bool useLocalCache = true)
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

            // Caching is gated per call, not via ILocalCache.IsEnabled: language
            // directions with different settings run concurrently and would race
            // on the shared instance's flag.
            var cacheKeys = new string[sourceTexts.Count];
            var pendingIndices = new List<int>(sourceTexts.Count);
            for (var i = 0; i < sourceTexts.Count; i++)
            {
                if (useLocalCache)
                {
                    cacheKeys[i] = BuildCacheKey(sourceLanguage, targetLanguage, modelType, tagHandling, formality, deepLSettings, sourceTexts[i]);
                    if (DeepLTranslationCache.Instance.TryGet(cacheKeys[i], out var cachedTranslation))
                    {
                        results[i] = (cachedTranslation, null);
                        continue;
                    }
                }

                pendingIndices.Add(i);
            }

            var pendingTexts = pendingIndices.Select(index => sourceTexts[index]).ToList();
            var batchStartIndex = 0;

            foreach (var batch in BuildBatches(pendingTexts))
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
                            results[pendingIndices[batchStartIndex + i]] = (null, errorMessage);
                    }
                    else
                    {
                        var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(responseBody);
                        for (var i = 0; i < batch.Count; i++)
                        {
                            var translation = translatedObject?.Translations?.Count > i
                                ? translatedObject.Translations[i].Text
                                : null;

                            var sourceIndex = pendingIndices[batchStartIndex + i];
                            results[sourceIndex] = (translation, null);

                            if (useLocalCache && !string.IsNullOrEmpty(translation))
                                DeepLTranslationCache.Instance.Set(cacheKeys[sourceIndex], translation);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var inner = ex is AggregateException aEx ? aEx.InnerExceptions.FirstOrDefault() ?? ex : ex;
                    for (var i = 0; i < batch.Count; i++)
                        results[pendingIndices[batchStartIndex + i]] = (null, inner.Message);
                }

                batchStartIndex += batch.Count;
            }

            return results;
        }

        private static string BuildCacheKey(
            string sourceLanguage,
            string targetLanguage,
            string modelType,
            string tagHandling,
            string formality,
            DeepLSettings deepLSettings,
            string sourceText)
        {
            return new Trados.LocalCache.CacheKeyBuilder()
                .Add("provider", "deepl")
                .Add("src", sourceLanguage)
                .Add("tgt", targetLanguage)
                .Add("model", modelType)
                .Add("tagHandling", tagHandling)
                .Add("formality", formality)
                .Add("glossary", deepLSettings.GlossaryId)
                .Add("style", deepLSettings.StyleId)
                .Add("tm", deepLSettings.TranslationMemoryId)
                .Add("preserveFormatting", deepLSettings.PreserveFormatting)
                .Add("splitSentences", deepLSettings.SplitSentencesHandling.GetApiValue())
                .AddValues("ignoreTags", deepLSettings.IgnoreTags)
                .Add("content", sourceText)
                .Build();
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
            AppInitializer.Client.GetAsync($"{BaseUrl}/usage").Result;

        public static HttpResponseMessage ValidateApiKey()
        {
            try
            {
                IsApiKeyValidResponse = IsValidApiKey();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "API key validation request failed; DeepL may be unreachable. Continuing with validity unknown.");
                IsApiKeyValidResponse = null;
            }

            return IsApiKeyValidResponse;
        }

        private static void OnApiKeyChanged()
        {
            AppInitializer.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DeepL-Auth-Key", ApiKey);
            IsApiKeyValidResponse = null;
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
                RequestUri = new Uri($"{BaseUrl}/translate")
            };

            var response = AppInitializer.Client.SendAsync(request).Result;
            return response;
        }
    }
}