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
using System.Threading;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Client
{
    public class DeepLTranslationProviderClient
    {
        private const int MaxBatchSizeBytes = 128 * 1024;

        // Measured (DET-830): DeepL used the context only in requests shaped like
        // Studio's 5-segment batches: the request's own segments plus the one before.
        // Larger context, or context reaching past the request, was ignored.
        private const int ContextSliceUnits = 5;
        private const int ContextUnitsBefore = 1;
        private const int ContextUnitsAfter = 0;

        private const int MaxSendAttempts = 3;
        private const int MaxRequestsInFlight = 8;
        private static readonly Random Jitter = new();

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
            bool useLocalCache = true,
            IReadOnlyList<int> unitPositions = null,
            IReadOnlyList<string> contextUnits = null,
            string customContext = null)
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
            // Slices are cut before the cache lookup: a slice's context is part of
            // its texts' cache keys, so slicing must not depend on what is cached.
            // The cache is only touched on this thread: lookups before the parallel
            // sends, writes after them.
            var cacheKeys = new string[sourceTexts.Count];
            var requests = new List<(List<int> Indices, DeeplRequestParameters Parameters)>();
            foreach (var (textIndices, context) in BuildBatches(sourceTexts, unitPositions, contextUnits, customContext))
            {
                var pendingIndices = new List<int>(textIndices.Count);
                foreach (var i in textIndices)
                {
                    if (useLocalCache)
                    {
                        cacheKeys[i] = BuildCacheKey(sourceLanguage, targetLanguage, modelType, tagHandling, formality, deepLSettings, sourceTexts[i], context);
                        if (DeepLTranslationCache.Instance.TryGet(cacheKeys[i], out var cachedTranslation))
                        {
                            results[i] = (cachedTranslation, null);
                            continue;
                        }
                    }

                    pendingIndices.Add(i);
                }

                if (pendingIndices.Count == 0) continue;

                var deeplRequestParameters = new DeeplRequestParameters
                {
                    Context = context,
                    Text = pendingIndices.Select(index => sourceTexts[index]).ToList(),
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
                requests.Add((pendingIndices, deeplRequestParameters));
            }

            // Task.Run: the sends must not resume on a caller's synchronization context.
            var outcomes = Task.Run(() => SendAllAsync(requests, r => TranslateAsync(r.Parameters), MaxRequestsInFlight))
                .GetAwaiter().GetResult();

            for (var r = 0; r < requests.Count; r++)
            {
                var (pendingIndices, _) = requests[r];
                var (translations, errorMessage) = outcomes[r];
                for (var i = 0; i < pendingIndices.Count; i++)
                {
                    var sourceIndex = pendingIndices[i];
                    if (errorMessage is not null)
                    {
                        results[sourceIndex] = (null, errorMessage);
                        continue;
                    }

                    var translation = translations?.Count > i ? translations[i].Text : null;
                    results[sourceIndex] = (translation, null);

                    if (useLocalCache && !string.IsNullOrEmpty(translation))
                        DeepLTranslationCache.Instance.Set(cacheKeys[sourceIndex], translation);
                }
            }

            return results;
        }

        public static string BuildCacheKey(
            string sourceLanguage,
            string targetLanguage,
            string modelType,
            string tagHandling,
            string formality,
            DeepLSettings deepLSettings,
            string sourceText,
            string context = null)
        {
            var keyBuilder = new Trados.LocalCache.CacheKeyBuilder()
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
                .Add("content", sourceText);

            // Added only when present, so context-free keys stay as before and
            // existing cache entries keep hitting.
            if (context != null)
                keyBuilder.Add("context", context);

            return keyBuilder.Build();
        }

        private static void ApplyDeepLRestrictions(DeeplRequestParameters deeplRequestParameters)
        {
            deeplRequestParameters.TagHandlingVersion =
                deeplRequestParameters.ModelType == "latency_optimized" ? "v1" : "v2";
        }

        public static IEnumerable<(List<int> TextIndices, string Context)> BuildBatches(
            IReadOnlyList<string> sourceTexts,
            IReadOnlyList<int> unitPositions = null,
            IReadOnlyList<string> contextUnits = null,
            string customContext = null)
        {
            // Walks the units in document order and cuts a request when the next unit
            // would push text + context over the limit, so each request carries the
            // context of its own slice. Without context, every text is its own unit
            // and only text bytes count.
            var unitCount = contextUnits?.Count ?? sourceTexts.Count;
            var textAtUnit = Enumerable.Repeat(-1, unitCount).ToArray();
            for (var i = 0; i < sourceTexts.Count; i++)
                textAtUnit[contextUnits == null ? i : unitPositions[i]] = i;

            // Prefix sums, so the size of a slice [from, to) including its padded
            // context is O(1): text bytes of the slice + context bytes of the padded range.
            var unitsBefore = contextUnits == null ? 0 : ContextUnitsBefore;
            var unitsAfter = contextUnits == null ? 0 : ContextUnitsAfter;
            // The custom context goes into every request's context, so it counts toward every request.
            customContext = string.IsNullOrWhiteSpace(customContext) ? null : customContext.Trim();
            var customContextSize = customContext == null ? 0 : Encoding.UTF8.GetByteCount(customContext) + 1;
            var textPrefix = new int[unitCount + 1];
            var contextPrefix = new int[unitCount + 1];
            for (var unit = 0; unit < unitCount; unit++)
            {
                var textIndex = textAtUnit[unit];
                textPrefix[unit + 1] = textPrefix[unit]
                    + (textIndex < 0 ? 0 : Encoding.UTF8.GetByteCount(sourceTexts[textIndex] ?? string.Empty));
                contextPrefix[unit + 1] = contextPrefix[unit]
                    + (contextUnits == null ? 0 : Encoding.UTF8.GetByteCount(contextUnits[unit]) + 1);
            }

            var textIndices = new List<int>();
            var firstUnit = 0;

            for (var unit = 0; unit < unitCount; unit++)
            {
                if (unit > firstUnit && (Size(firstUnit, unit + 1) > MaxBatchSizeBytes
                    || contextUnits != null && unit - firstUnit >= ContextSliceUnits))
                {
                    if (textIndices.Count > 0)
                        yield return (textIndices, JoinContext(firstUnit, unit));

                    textIndices = new List<int>();
                    firstUnit = unit;
                }

                if (textAtUnit[unit] >= 0) textIndices.Add(textAtUnit[unit]);
            }

            if (textIndices.Count > 0)
                yield return (textIndices, JoinContext(firstUnit, unitCount));

            int PaddedFrom(int from) => Math.Max(0, from - unitsBefore);
            int PaddedTo(int to) => Math.Min(unitCount, to + unitsAfter);

            int Size(int from, int to) =>
                textPrefix[to] - textPrefix[from] + contextPrefix[PaddedTo(to)] - contextPrefix[PaddedFrom(from)] + customContextSize;

            // A slice over the limit is a single unit too big to also carry its context:
            // send it without, as it would be sent with the option off.
            string JoinContext(int from, int to)
            {
                if (contextUnits == null && customContext == null || Size(from, to) > MaxBatchSizeBytes) return null;
                if (contextUnits == null) return customContext;
                return (customContext == null ? string.Empty : customContext + "\n")
                    + string.Join("\n", contextUnits.Skip(PaddedFrom(from)).Take(PaddedTo(to) - PaddedFrom(from)));
            }
        }

        // Runs all sends concurrently, at most maxInFlight at a time; results keep request order.
        public static async Task<TResult[]> SendAllAsync<TRequest, TResult>(
            IReadOnlyList<TRequest> requests,
            Func<TRequest, Task<TResult>> send,
            int maxInFlight)
        {
            using var gate = new SemaphoreSlim(maxInFlight);
            return await Task.WhenAll(requests.Select(async request =>
            {
                await gate.WaitAsync().ConfigureAwait(false);
                try { return await send(request).ConfigureAwait(false); }
                finally { gate.Release(); }
            })).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> SendWithRetryAsync(
            Func<Task<HttpResponseMessage>> send,
            Func<TimeSpan, Task> delay)
        {
            // DeepL: retry 429/529 and 5xx with exponential backoff and jitter; never 456 (quota).
            for (var attempt = 1; ; attempt++)
            {
                var response = await send().ConfigureAwait(false);
                var status = (int)response.StatusCode;
                if (attempt >= MaxSendAttempts || status != 429 && status < 500) return response;

                var retryAfter = response.Headers.RetryAfter;
                response.Dispose();

                int jitterMs;
                lock (Jitter) jitterMs = Jitter.Next(250);
                var wait = retryAfter?.Delta
                    ?? (retryAfter?.Date is { } until ? until - DateTimeOffset.UtcNow : (TimeSpan?)null)
                    ?? TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)) + TimeSpan.FromMilliseconds(jitterMs);
                await delay(wait > TimeSpan.Zero ? wait : TimeSpan.Zero).ConfigureAwait(false);
            }
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

        // One request per slice; errors are returned, not thrown, so one failed slice
        // only fails its own segments.
        private static async Task<(List<TranslationDetails> Translations, string ErrorMessage)> TranslateAsync(
            DeeplRequestParameters deeplRequestParameters)
        {
            var requestJson = JsonConvert.SerializeObject(
                deeplRequestParameters,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            var requestUri = new Uri($"{BaseUrl}/translate");

            // .NET Framework allows 2 connections per host by default, which would
            // serialize the parallel requests; raise it for DeepL's host only.
            var servicePoint = ServicePointManager.FindServicePoint(requestUri);
            if (servicePoint.ConnectionLimit < MaxRequestsInFlight)
                servicePoint.ConnectionLimit = MaxRequestsInFlight;

            try
            {
                // A request message can only be sent once, so each attempt builds its own.
                using var response = await SendWithRetryAsync(
                    () => AppInitializer.Client.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
                    {
                        Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
                    }),
                    Task.Delay).ConfigureAwait(false);

                var responseBody = response.Content is null ? null : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                    return (null, !string.IsNullOrWhiteSpace(responseBody) ? responseBody : response.ReasonPhrase);

                return (JsonConvert.DeserializeObject<TranslationResponse>(responseBody)?.Translations, null);
            }
            catch (Exception ex)
            {
                var inner = ex is AggregateException aEx ? aEx.InnerExceptions.FirstOrDefault() ?? ex : ex;
                return (null, inner.Message);
            }
        }
    }
}