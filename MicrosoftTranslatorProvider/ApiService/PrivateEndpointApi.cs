using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Model;
using Newtonsoft.Json;

namespace MicrosoftTranslatorProvider.ApiService
{
	class PrivateEndpointApi
    {
		private readonly string _uri;
		private readonly List<UrlMetadata> _headers;
		private readonly List<UrlMetadata> _parameters;

		public PrivateEndpointApi(string endpoint, List<UrlMetadata> headers, List<UrlMetadata> parameters)
		{
			_headers = headers;
			_parameters = parameters;
			_uri = BuildUri(endpoint);
		}

		private string BuildUri(string endpoint)
		{
			var url = endpoint.StartsWith("https://") ? endpoint : $"https://{endpoint}";
			url += url.EndsWith("?") ? string.Empty : "?";

			foreach (var parameter in _parameters)
			{
				if (parameter.Key.Equals("from") || parameter.Key.Equals("to"))
				{
					continue;
				}

				url += $"{parameter.Key}={parameter.Value}&";
			}

			return url.EndsWith("?") ? url : url.Substring(0, url.Length - 1);
		}

		public string Translate(string sourceLanguage, string targetLanguage, string textToTranslate)
		{
			try
			{
				sourceLanguage = ConvertLanguageCode(sourceLanguage);
				targetLanguage = ConvertLanguageCode(targetLanguage);
				return TryTranslate(sourceLanguage, targetLanguage, textToTranslate);
			}
			catch (WebException exception)
			{
				ErrorHandler.HandleError(exception);
				return null;
			}
		}

		private string TryTranslate(string sourceLanguage, string targetLanguage, string textToTranslate)
		{
			const string RegexPattern = @"(\<\w+[üäåëöøßşÿÄÅÆĞ]*[^\d\W\\/\\]+\>)";
			var words = new Regex(RegexPattern).Matches(textToTranslate); //search for words like this: <example> 
			if (words.Count > 0)
			{
				textToTranslate = textToTranslate.ReplaceCharacters(words);
			}

			return RequestTranslation(sourceLanguage, targetLanguage, textToTranslate);
		}

		private string RequestTranslation(string sourceLanguage, string targetLanguage, string textToTranslate)
		{
			try
			{
				return TryRequestTranslation(sourceLanguage, targetLanguage, textToTranslate);
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleError(ex);
				return null;
			}
		}

		private string TryRequestTranslation(string sourceLanguage, string targetLanguage, string textToTranslate)
		{
			var body = new object[] { new { Text = textToTranslate } };
			var requestBody = JsonConvert.SerializeObject(body);
			var httpRequest = new HttpRequestMessage()
			{
				Method = HttpMethod.Post,
				Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
				RequestUri = new Uri($"{_uri}from={sourceLanguage}&to={targetLanguage}")
			};

			foreach (var header in _headers)
			{
				httpRequest.Headers.Add(header.Key, header.Value);
			}

			var httpClient = new HttpClient();
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

		private string ConvertLanguageCode(string languageCode)
		{
			const string TraditionalChinese = "zh-TW zh-HK zh-MO zh-Hant zh-CHT";
			const string SimplifiedChinese = "zh-CN zh-SG zh-Hans-HK zh-Hans-MO zh-Hans zh-CHS";

			var cultureInfo = new CultureInfo(languageCode);
			var isSerbianCyrillic = languageCode.Contains("sr-Cyrl");
			var isSerbianLatin = languageCode.Contains("sr-Latn");
			var isTraditionalChinese = TraditionalChinese.Contains(cultureInfo.Name);
			var isSimplifiedChinese = SimplifiedChinese.Contains(cultureInfo.Name);

			if (isSerbianCyrillic)
			{
				return "sr-Cyrl";
			}
			else if (isSerbianLatin)
			{
				return "sr-Latn";
			}
			else if (isTraditionalChinese)
			{
				return "zh-Hant";
			}
			else if (isSimplifiedChinese)
			{
				return "zh-Hans";
			}

			return cultureInfo.TwoLetterISOLanguageName;
		}
	}
}