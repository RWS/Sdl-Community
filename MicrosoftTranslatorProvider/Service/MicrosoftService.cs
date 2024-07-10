using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service.Model;
using Newtonsoft.Json;
using TradosProxySettings;

namespace MicrosoftTranslatorProvider.Service
{
    public static class MicrosoftService
    {
        public static async Task<bool> AuthenticateUser(MicrosoftCredentials credentials)
        {
            var region = credentials.Region;
            if (!string.IsNullOrEmpty(region))
            {
                region += ".";
            }

            var uriString = $"https://{region}{Constants.MicrosoftProviderServiceUriBase}/sts/v1.0/issueToken";
            var uri = new Uri(uriString);
            try
            {
                var httpClientHandler = ProxyHelper.GetHttpClientHandler(CredentialsManager.GetProxySettings(), true);
                using var httpClient = new HttpClient(httpClientHandler);
                using var request = new HttpRequestMessage();
                request.Method = HttpMethod.Post;
                request.RequestUri = uri;
                request.Headers.TryAddWithoutValidation(Constants.OcpApimSubscriptionKeyHeader, credentials.APIKey);

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var tokenString = await response.Content.ReadAsStringAsync();
                var token = ReadToken(tokenString);
                credentials.AccessToken = "Bearer " + tokenString;
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex);
                return false;
            }
        }

        public async static Task<List<TranslationLanguage>> GetSupportedLanguages()
        {
            var httpClientHandler = ProxyHelper.GetHttpClientHandler(CredentialsManager.GetProxySettings(), true);
            var httpClient = new HttpClient(httpClientHandler);

            var url = "https://api.cognitive.microsofttranslator.com/languages?api-version=3.0";

            var response = await httpClient.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();
            var languagesResponse = JsonConvert.DeserializeObject<LanguagesResponse>(responseBody);

            var output = languagesResponse.Translation.Select(x => new TranslationLanguage()
            {
                LanguageCode = x.Key,
                LanguageName = x.Value.Name,
            }).ToList();

            return output;
        }

        public async static Task<HashSet<string>> GetSupportedLanguageCodes()
        {
            var supportedLanguages = await GetSupportedLanguages();
            var supportedCodes = new HashSet<string>(supportedLanguages.Select(x => x.LanguageCode));
            return supportedCodes;
        }

        private static JwtSecurityToken ReadToken(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var readableToken = jwtHandler.CanReadToken(token);
            return readableToken ? jwtHandler.ReadJwtToken(token)
                                 : null;
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
            httpRequest.Headers.Add("Authorization", microsoftCredentials.AccessToken);

            var httpClientHandler = ProxyHelper.GetHttpClientHandler(CredentialsManager.GetProxySettings(), true);
            using var httpClient = new HttpClient(httpClientHandler);
            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseTranslation = JsonConvert.DeserializeObject<List<TranslationResponse>>(responseBody);
            var translation = new HtmlUtil().HtmlDecode(responseTranslation[0]?.Translations[0]?.Text);
            return translation;
        }

        private static string ReplaceCharacters(string text, MatchCollection words)
        {
            foreach (Match match in words)
            {
                text = text.Replace(match.Value, "");
            }

            return text;
        }

        private static string BuildTranslationUri(string sourceLanguage, string targetLanguage, string category)
        {
            const string uri = $@"https://{Constants.MicrosoftProviderUriBase}";
            const string path = "/translate?api-version=3.0";
            var languageParams = $"&from={sourceLanguage}&to={targetLanguage}&textType=html&category={category}";

            return string.Concat(uri, path, languageParams);
        }
    }
}