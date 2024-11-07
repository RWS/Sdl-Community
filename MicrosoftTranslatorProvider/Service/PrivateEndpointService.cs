using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using TradosProxySettings;
using TradosProxySettings.Model;

namespace MicrosoftTranslatorProvider.Service
{
    public static class PrivateEndpointService
    {
        public static string Translate(PrivateEndpoint privateEndpoint, ProxySettings proxySettings, PairModel pairMapping, string textToTranslate)
        {
            try
            {
                return TryTranslate(privateEndpoint, pairMapping, textToTranslate);
            }
            catch (WebException exception)
            {
                ErrorHandler.HandleError(exception);
                return null;
            }
        }

        private static string BuildUri(PrivateEndpoint privateEndpoint)
        {
            var endpoint = privateEndpoint.Endpoint;
            var parameters = privateEndpoint.Parameters;

            var url = endpoint.StartsWith("https://") ? endpoint : $"https://{endpoint}";
            url += url.EndsWith("?") ? string.Empty : "?";

            foreach (var parameter in parameters)
            {
                if (parameter.Key.Equals("from") || parameter.Key.Equals("to"))
                {
                    continue;
                }

                url += $"{parameter.Key}={parameter.Value}&";
            }

            return url.EndsWith("?") ? url : url.Substring(0, url.Length - 1);
        }

        private static string RequestTranslation(PrivateEndpoint privateEndpoint, PairModel pairMapping, string textToTranslate)
        {
            try
            {
                return TryRequestTranslation(privateEndpoint, pairMapping, textToTranslate);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex);
                return null;
            }
        }

        private static string TryRequestTranslation(PrivateEndpoint privateEndpoint, PairModel pairMapping, string textToTranslate)
        {
            var body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);
            var httpRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
                RequestUri = new Uri($"{BuildUri(privateEndpoint)}from={pairMapping.SourceLanguageCode}&to={pairMapping.TargetLanguageCode}")
            };

            foreach (var header in privateEndpoint.Headers)
            {
                httpRequest.Headers.Add(header.Key, header.Value);
            }

            var httpClientHandler = ProxyHelper.GetHttpClientHandler(CredentialsManager.GetProxySettings(), true);
            using var httpClient = new HttpClient(httpClientHandler);
            var response = httpClient.SendAsync(httpRequest).Result;
            var responseBody = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(responseBody);
                throw new Exception(responseMessage.Error.Message);
            }

            var responseTranslation = JsonConvert.DeserializeObject<List<TranslationResponse>>(responseBody);
            return new HtmlUtil().HtmlDecode(responseTranslation[0]?.Translations[0]?.Text);
        }

        private static string TryTranslate(PrivateEndpoint privateEndpoint, PairModel pairMapping, string textToTranslate)
        {
            const string RegexPattern = @"(\<\w+[üäåëöøßşÿÄÅÆĞ]*[^\d\W\\/\\]+\>)";
            var words = new Regex(RegexPattern).Matches(textToTranslate); //search for words like this: <example>
            if (words.Count > 0)
            {
                textToTranslate = textToTranslate.ReplaceCharacters(words);
            }

            return RequestTranslation(privateEndpoint, pairMapping, textToTranslate);
        }

        public static bool ValidateSettings(PrivateEndpoint privateEndpoint)
        {
            try
            {
                TestTranslate(privateEndpoint);
                return true;
            }
            catch (Exception e)
            {
                ErrorHandler.HandleError(e);
                return false;
            }
        }

        private static void TestTranslate(PrivateEndpoint privateEndpoint)
        {
            var body = new object[] { new { Text = "test" } };
            var requestBody = JsonConvert.SerializeObject(body);
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
                RequestUri = new Uri($"{BuildUri(privateEndpoint)}to=de")
            };

            foreach (var header in privateEndpoint.Headers)
                httpRequest.Headers.Add(header.Key, header.Value);

            var httpClientHandler = ProxyHelper.GetHttpClientHandler(CredentialsManager.GetProxySettings(), true);
            using var httpClient = new HttpClient(httpClientHandler);
            var response = httpClient.SendAsync(httpRequest).Result;
            var responseBody = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
                return;

            var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(responseBody);
            throw new Exception(responseMessage.Error.Message);
        }
    }
}