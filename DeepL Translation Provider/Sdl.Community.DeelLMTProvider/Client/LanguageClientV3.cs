using Newtonsoft.Json;
using Sdl.Community.DeepLMTProvider.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Gets detailed language information with all features from V3 API
        /// </summary>
        /// <param name="languageCode">Language code to get information for</param>
        /// <param name="product">Product to get features for</param>
        /// <param name="apiKey">DeepL API key</param>
        /// <param name="baseUrl">Base URL for the DeepL API</param>
        /// <returns>LanguageV3Response object with detailed features, or null if not found</returns>
        public static async Task<LanguageV3Response> GetLanguageV3InfoAsync(string languageCode, string product, string apiKey, string baseUrl)
        {
            if (string.IsNullOrEmpty(languageCode))
                return null;

            try
            {
                var supportedLanguages = await GetSupportedLanguagesAsync(product, apiKey, baseUrl);
                return supportedLanguages.Find(lang => string.Equals(lang.Lang, languageCode, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if a specific language is supported for the given type
        /// </summary>
        /// <param name="languageCode">Language code to check (e.g., "EN", "DE")</param>
        /// <param name="type">Type of language support to check: "source" or "target"</param>
        /// <param name="apiKey">DeepL API key</param>
        /// <param name="baseUrl">Base URL for the DeepL API</param>
        /// <param name="product">Product to check support for (defaults to "translate_text")</param>
        /// <returns>True if the language is supported, false otherwise</returns>
        public static async Task<bool> IsLanguageSupportedAsync(string languageCode, string type, string apiKey, string baseUrl, string product = "translate_text")
        {
            if (string.IsNullOrEmpty(languageCode))
                return false;

            try
            {
                var supportedLanguages = await GetSupportedLanguagesAsync(type, product, apiKey, baseUrl);
                return supportedLanguages.Exists(lang => string.Equals(lang.Language, languageCode, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves supported languages and converts to legacy format for compatibility
        /// </summary>
        /// <param name="type">Type of languages to retrieve: "source" or "target"</param>
        /// <param name="product">Product to retrieve languages for: "translate_text", "translate_document", etc.</param>
        /// <param name="apiKey">DeepL API key</param>
        /// <param name="baseUrl">Base URL for the DeepL API</param>
        /// <returns>List of supported languages in legacy format</returns>
        private static async Task<List<SupportedLanguage>> GetSupportedLanguagesAsync(string type, string product, string apiKey, string baseUrl)
        {
            var v3Languages = await GetSupportedLanguagesAsync(product, apiKey, baseUrl);
            var result = new List<SupportedLanguage>();

            foreach (var lang in v3Languages)
            {
                bool includeLanguage = type.ToLower() switch
                {
                    "source" => lang.UsableAsSource,
                    "target" => lang.UsableAsTarget,
                    _ => throw new ArgumentException($"Invalid type: {type}. Must be 'source' or 'target'.")
                };

                if (includeLanguage)
                {
                    result.Add(new SupportedLanguage
                    {
                        Language = lang.Lang,
                        Name = lang.Name,
                        SupportsOptions = lang.Features?.Contains("formality") == true
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves supported languages from DeepL API V3 endpoint
        /// </summary>
        /// <param name="product">Product to retrieve languages for: "translate_text", "translate_document", "glossary", "voice", "write", "style_rules"</param>
        /// <param name="apiKey">DeepL API key</param>
        /// <param name="baseUrl">Base URL for the DeepL API</param>
        /// <returns>List of supported languages with enhanced metadata</returns>
        private static async Task<List<LanguageV3Response>> GetSupportedLanguagesAsync(string product, string apiKey, string baseUrl)
        {
            if (string.IsNullOrEmpty(product))
                throw new ArgumentException("Product cannot be null or empty", nameof(product));

            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));

            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Base URL cannot be null or empty", nameof(baseUrl));

            var requestUri = new Uri($"{baseUrl.TrimEnd('/')}/{LanguagesEndpoint}?product={product}");

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Authorization", $"DeepL-Auth-Key {apiKey}");
            request.Headers.Add("Accept", "application/json");

            try
            {
                var response = await AppInitializer.Client.SendAsync(request);

                // Log response details for debugging
                System.Diagnostics.Debug.WriteLine($"[DeepL V3] Request: {requestUri}");
                System.Diagnostics.Debug.WriteLine($"[DeepL V3] Response Status: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[DeepL V3] Response Content: {content}");

                var languages = JsonConvert.DeserializeObject<List<LanguageV3Response>>(content) ?? new List<LanguageV3Response>();

                return languages;
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