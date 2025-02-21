using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TradosProxySettings;

namespace MicrosoftTranslatorProvider.Service
{
    public static class MicrosoftService
    {
        public static async Task<bool> AuthenticateUser(MicrosoftCredentials credentials)
        {
            try
            {
                await TestTranslate(credentials);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex);
                return false;
            }
        }
        public static async Task<HashSet<string>> GetSupportedLanguageCodes()
        {
            var supportedLanguages = await GetSupportedLanguages();
            var supportedCodes = new HashSet<string>(supportedLanguages.Select(x => x.LanguageCode));
            return supportedCodes;
        }

        public static async Task<List<TranslationLanguage>> GetSupportedLanguages()
        {
            var httpClientHandler = ProxyHelper.GetHttpClientHandler(CredentialsManager.GetProxySettings(), true);
            var httpClient = new HttpClient(httpClientHandler);

            var url = "https://api.cognitive.microsofttranslator.com/languages?api-version=3.0";

            var response = await httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error: {response.StatusCode}, {responseBody}");

            var languagesResponse = JsonConvert.DeserializeObject<LanguagesResponse>(responseBody);

            var output = languagesResponse.Translation.Select(x => new TranslationLanguage()
            {
                LanguageCode = x.Key,
                LanguageName = x.Value.Name,
            }).ToList();

            return output;
        }

        public static async Task<string> TranslateAsync(PairModel pairModel, string textToTranslate, MicrosoftCredentials microsoftCredentials)
        {
            var sourceLanguage = pairModel.SourceLanguageCode;
            var targetLanguage = pairModel.TargetLanguageCode;
            var categoryId = string.IsNullOrEmpty(pairModel.Model) ? "general" : pairModel.Model;

            var words = Regex.Matches(textToTranslate, @"(\<\w+[üäåëöøßşÿÄÅÆĞ]*[^\d\W\\/\\]+\>)");
            if (words.Count > 0)
            {
                textToTranslate = ReplaceCharacters(textToTranslate, words);
            }

            var requestBody = JsonConvert.SerializeObject(new[] { new { Text = textToTranslate } });
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
                RequestUri = new Uri(BuildTranslationUri(sourceLanguage, targetLanguage, categoryId))
            };

            httpRequest.Headers.Add("Ocp-Apim-Subscription-Key", microsoftCredentials.APIKey);
            httpRequest.Headers.Add("Ocp-Apim-Subscription-Region", microsoftCredentials.Region);

            var httpClientHandler = ProxyHelper.GetHttpClientHandler(CredentialsManager.GetProxySettings(), true);
            using var httpClient = new HttpClient(httpClientHandler);
            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseTranslation = JsonConvert.DeserializeObject<List<TranslationResponse>>(responseBody);
            var translation = new HtmlUtil().HtmlDecode(responseTranslation[0]?.Translations[0]?.Text);
            return translation;
        }

        private static string BuildTranslationUri(string sourceLanguage, string targetLanguage, string category)
        {
            const string uri = $@"https://{Constants.MicrosoftProviderUriBase}";
            const string path = "/translate?api-version=3.0";
            var languageParams = $"&from={sourceLanguage}&to={targetLanguage}&textType=html&category={category}";

            return string.Concat(uri, path, languageParams);
        }

        private static string ReplaceCharacters(string text, MatchCollection words)
        {
            foreach (Match match in words)
            {
                text = text.Replace(match.Value, "");
            }

            return text;
        }

        private static async Task TestTranslate(MicrosoftCredentials credentials)
        {
            var sourceLanguage = "en";
            var targetLanguage = "de";

            var requestBody = JsonConvert.SerializeObject(new[] { new { Text = "test" } });
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
                RequestUri = new Uri(BuildTranslationUri(sourceLanguage, targetLanguage, "general"))
            };

            httpRequest.Headers.Add("Ocp-Apim-Subscription-Key", credentials.APIKey);
            httpRequest.Headers.Add("Ocp-Apim-Subscription-Region", credentials.Region);

            var httpClientHandler = ProxyHelper.GetHttpClientHandler(CredentialsManager.GetProxySettings(), true);
            using var httpClient = new HttpClient(httpClientHandler);
            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
        }
    }
}