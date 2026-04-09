using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Client
{
    /// <summary>
    /// Client for DeepL's Retrieve languages [BETA] endpoint (V3 API)
    /// Provides enhanced language detection and support information
    /// </summary>
    public class LanguageClientV3
    {
        private const string LanguagesEndpoint = "v3/languages";

        // Session-scoped cache keyed by (product, apiKey, baseUrl).
        // Static lifetime matches Studio's process lifetime, so results persist for the whole session.
        private static readonly Dictionary<(string product, string apiKey), List<LanguageV3Response>> _cache = new();

        private static string BaseUrl => Constants.BaseUrlV3;

        /// <summary>Clears the session language cache. Call after an API key change to release stale entries.</summary>
        public static void ClearCache() => _cache.Clear();

        /// <summary>Gets detailed language information with all features from V3 API.</summary>
        public static async Task<LanguageV3Response> GetLanguageV3InfoAsync(string languageCode, string product, string apiKey)
        {
            if (string.IsNullOrEmpty(languageCode))
                return null;

            try
            {
                var languages = await GetLanguagesByProductAsync(product, apiKey);
                return languages.Find(lang => string.Equals(lang.Lang, languageCode, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Checks if a specific language is supported as source or target for a given product.</summary>
        public static async Task<bool> IsLanguageSupportedAsync(string languageCode, string type, string apiKey, string product = "translate_text")
        {
            if (string.IsNullOrEmpty(languageCode))
                return false;

            try
            {
                var languages = await GetLanguagesByProductAsync(product, apiKey);
                return languages.Exists(lang =>
                    string.Equals(lang.Lang, languageCode, StringComparison.OrdinalIgnoreCase) &&
                    type.ToLowerInvariant() switch
                    {
                        "source" => lang.UsableAsSource,
                        "target" => lang.UsableAsTarget,
                        _ => false
                    });
            }
            catch
            {
                return false;
            }
        }

        private static async Task<List<LanguageV3Response>> FetchFromApiAsync(string product, string apiKey)
        {
            if (string.IsNullOrEmpty(product))
                throw new ArgumentException("Product cannot be null or empty", nameof(product));
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));

            var requestUri = new Uri($"{BaseUrl.TrimEnd('/')}/{LanguagesEndpoint}?product={product}");

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Authorization", $"DeepL-Auth-Key {apiKey}");
            request.Headers.Add("Accept", "application/json");

            try
            {
                var response = await AppInitializer.Client.SendAsync(request);

                System.Diagnostics.Debug.WriteLine($"[DeepL V3] Request: {requestUri}");
                System.Diagnostics.Debug.WriteLine($"[DeepL V3] Response Status: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[DeepL V3] Response Content: {content}");

                return JsonConvert.DeserializeObject<List<LanguageV3Response>>(content) ?? new List<LanguageV3Response>();
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DeepL V3] HTTP Error: {ex.Message}");
                throw new InvalidOperationException($"Failed to retrieve languages from DeepL API: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DeepL V3] JSON Error: {ex.Message}");
                throw new InvalidOperationException($"Failed to parse language response from DeepL API: {ex.Message}", ex);
            }
        }

        // Returns the cached list for the given product, fetching from the API on the first call.
        private static async Task<List<LanguageV3Response>> GetCachedLanguagesAsync(string product, string apiKey)
        {
            var key = (product, apiKey);
            if (_cache.TryGetValue(key, out var cached))
                return cached;

            var languages = await FetchFromApiAsync(product, apiKey);
            _cache[key] = languages;
            return languages;
        }

        private static Task<List<LanguageV3Response>> GetGlossaryLanguagesAsync(string apiKey)
            => GetCachedLanguagesAsync("glossary", apiKey);

        // Routes to the dedicated method for known products; falls back to the cache layer for any future product.
        private static Task<List<LanguageV3Response>> GetLanguagesByProductAsync(string product, string apiKey)
            => product switch
            {
                "translate_text" => GetTranslateTextLanguagesAsync(apiKey),
                "glossary" => GetGlossaryLanguagesAsync(apiKey),
                "style_rules" => GetStyleRulesLanguagesAsync(apiKey),
                _ => GetCachedLanguagesAsync(product, apiKey)
            };

        private static Task<List<LanguageV3Response>> GetStyleRulesLanguagesAsync(string apiKey)
            => GetCachedLanguagesAsync("style_rules", apiKey);

        private static Task<List<LanguageV3Response>> GetTranslateTextLanguagesAsync(string apiKey)
                                                    => GetCachedLanguagesAsync("translate_text", apiKey);
    }

    /// <summary>
    /// Response model for DeepL V3 languages endpoint
    /// </summary>
    public class LanguageV3Response
    {
        [JsonProperty("features")]
        public string[] Features { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("usable_as_source")]
        public bool UsableAsSource { get; set; }

        [JsonProperty("usable_as_target")]
        public bool UsableAsTarget { get; set; }
    }
}