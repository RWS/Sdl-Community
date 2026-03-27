using Newtonsoft.Json;
using Sdl.Community.DeepLMTProvider.Model;
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

        /// <summary>
        /// Retrieves supported languages from DeepL API V3 endpoint
        /// </summary>
        /// <param name="type">Type of languages to retrieve: "source" or "target"</param>
        /// <param name="apiKey">DeepL API key</param>
        /// <param name="baseUrl">Base URL for the DeepL API</param>
        /// <returns>List of supported languages with enhanced metadata</returns>
        public static async Task<List<SupportedLanguage>> GetSupportedLanguagesAsync(string type, string apiKey, string baseUrl)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentException("Language type cannot be null or empty", nameof(type));

            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));

            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("Base URL cannot be null or empty", nameof(baseUrl));

            var requestUri = new Uri($"{baseUrl.TrimEnd('/')}/{LanguagesEndpoint}?type={type}");

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Authorization", $"DeepL-Auth-Key {apiKey}");
            request.Headers.Add("Accept", "application/json");

            try
            {
                var response = await AppInitializer.Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var languages = JsonConvert.DeserializeObject<List<SupportedLanguage>>(content) ?? new List<SupportedLanguage>();

                return languages;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve languages from DeepL API: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse language response from DeepL API: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Retrieves supported languages from DeepL API V3 endpoint (synchronous version)
        /// </summary>
        /// <param name="type">Type of languages to retrieve: "source" or "target"</param>
        /// <param name="apiKey">DeepL API key</param>
        /// <param name="baseUrl">Base URL for the DeepL API</param>
        /// <returns>List of supported languages with enhanced metadata</returns>
        public static List<SupportedLanguage> GetSupportedLanguages(string type, string apiKey, string baseUrl)
        {
            return GetSupportedLanguagesAsync(type, apiKey, baseUrl).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Retrieves all source languages from DeepL API V3
        /// </summary>
        /// <param name="apiKey">DeepL API key</param>
        /// <param name="baseUrl">Base URL for the DeepL API</param>
        /// <returns>List of supported source languages</returns>
        public static async Task<List<SupportedLanguage>> GetSourceLanguagesAsync(string apiKey, string baseUrl)
        {
            return await GetSupportedLanguagesAsync("source", apiKey, baseUrl);
        }

        /// <summary>
        /// Retrieves all target languages from DeepL API V3
        /// </summary>
        /// <param name="apiKey">DeepL API key</param>
        /// <param name="baseUrl">Base URL for the DeepL API</param>
        /// <returns>List of supported target languages</returns>
        public static async Task<List<SupportedLanguage>> GetTargetLanguagesAsync(string apiKey, string baseUrl)
        {
            return await GetSupportedLanguagesAsync("target", apiKey, baseUrl);
        }

        /// <summary>
        /// Checks if a specific language is supported for the given type
        /// </summary>
        /// <param name="languageCode">Language code to check (e.g., "EN", "DE")</param>
        /// <param name="type">Type of language support to check: "source" or "target"</param>
        /// <param name="apiKey">DeepL API key</param>
        /// <param name="baseUrl">Base URL for the DeepL API</param>
        /// <returns>True if the language is supported, false otherwise</returns>
        public static async Task<bool> IsLanguageSupportedAsync(string languageCode, string type, string apiKey, string baseUrl)
        {
            if (string.IsNullOrEmpty(languageCode))
                return false;

            try
            {
                var supportedLanguages = await GetSupportedLanguagesAsync(type, apiKey, baseUrl);
                return supportedLanguages.Exists(lang => string.Equals(lang.Language, languageCode, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets language metadata including formality support
        /// </summary>
        /// <param name="languageCode">Language code to get metadata for</param>
        /// <param name="type">Type of language: "source" or "target"</param>
        /// <param name="apiKey">DeepL API key</param>
        /// <param name="baseUrl">Base URL for the DeepL API</param>
        /// <returns>SupportedLanguage object with metadata, or null if not found</returns>
        public static async Task<SupportedLanguage> GetLanguageMetadataAsync(string languageCode, string type, string apiKey, string baseUrl)
        {
            if (string.IsNullOrEmpty(languageCode))
                return null;

            try
            {
                var supportedLanguages = await GetSupportedLanguagesAsync(type, apiKey, baseUrl);
                return supportedLanguages.Find(lang => string.Equals(lang.Language, languageCode, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return null;
            }
        }
    }
}