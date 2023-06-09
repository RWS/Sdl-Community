﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Model;
using MicrosoftTranslatorProvider.Service;
using Newtonsoft.Json;
using NLog;
using RestSharp;

namespace MicrosoftTranslatorProvider.Studio.TranslationProvider
{
	public class MicrosoftApi
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly HtmlUtil _htmlUtil;

		private string _subscriptionKey;
		private string _region;
		private string _authToken;
		private List<string> _supportedLanguages;

		public MicrosoftApi(string subscriptionKey, string region, HtmlUtil htmlUtil)
		{
			_subscriptionKey = subscriptionKey;
			_region = region;
			_htmlUtil = htmlUtil;
			_authToken ??= GetAuthToken();
			_supportedLanguages ??= GetSupportedLanguages();
		}

		public void RefreshAuthToken()
		{
			_authToken = string.Empty;
			_authToken = GetAuthToken();
		}

		public void ResetCredentials(string subscriptionKey, string region)
		{
			if (subscriptionKey == _subscriptionKey
			 && region == _region)
			{
				return;
			}

			_subscriptionKey = subscriptionKey;
			_region = region;
			_authToken = GetAuthToken();
			_supportedLanguages = GetSupportedLanguages();
		}

		public bool IsSupportedLanguagePair(string sourceLanguage, string tarrgetLanguage)
		{
			sourceLanguage = ConvertLanguageCode(sourceLanguage);
			tarrgetLanguage = ConvertLanguageCode(tarrgetLanguage);
			var (sourceSupported, targetSupported) = (false, false);
			foreach (var language in _supportedLanguages)
			{
				sourceSupported = sourceSupported ? sourceSupported : language.Equals(sourceLanguage);
				targetSupported = targetSupported ? targetSupported : language.Equals(tarrgetLanguage);
				if (sourceSupported && targetSupported)
				{
					return true;
				}
			}

			return false;
		}

		public string Translate(string sourceLanguage, string targetLanguage, string textToTranslate, string categoryId)
		{
			_authToken ??= GetAuthToken();
			if (_authToken is null)
			{
				throw new Exception("Invalid credentials");
			}

			try
			{
				sourceLanguage = ConvertLanguageCode(sourceLanguage);
				targetLanguage = ConvertLanguageCode(targetLanguage);
				categoryId = categoryId == "" ? "general" : categoryId;
				return TryTranslate(sourceLanguage, targetLanguage, textToTranslate, categoryId);
			}
			catch (WebException exception)
			{
				ErrorHandler.HandleError(exception);
				return null;
			}
		}

		private string TryTranslate(string sourceLanguage, string targetLanguage, string textToTranslate, string categoryID)
		{
			const string RegexPattern = @"(\<\w+[üäåëöøßşÿÄÅÆĞ]*[^\d\W\\/\\]+\>)";
			var words = new Regex(RegexPattern).Matches(textToTranslate); //search for words like this: <example> 
			if (words.Count > 0)
			{
				textToTranslate = textToTranslate.ReplaceCharacters(words);
			}

			return RequestTranslation(sourceLanguage, targetLanguage, textToTranslate, categoryID);
		}

		private string RequestTranslation(string sourceLanguage, string targetLanguage, string textToTranslate, string categoryID)
		{
			var body = new object[] { new { Text = textToTranslate } };
			var requestBody = JsonConvert.SerializeObject(body);
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
				RequestUri = new Uri(BuildTranslationUri(sourceLanguage, targetLanguage, categoryID))
			};
			httpRequest.Headers.Add("Authorization", _authToken);

			var httpClient = new HttpClient();
			var response = httpClient.SendAsync(httpRequest).Result;
			var responseBody = response.Content.ReadAsStringAsync().Result;
			if (!response.IsSuccessStatusCode)
			{
				var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(responseBody);
				throw new Exception(responseMessage.Error.Message);
			}

			var responseTranslation = JsonConvert.DeserializeObject<List<TranslationResponse>>(responseBody);
			return _htmlUtil.HtmlDecode(responseTranslation[0]?.Translations[0]?.Text);
		}

		private string BuildTranslationUri(string sourceLanguage, string targetLanguage, string category)
		{
			const string path = "/translate?api-version=3.0";
			const string uri = $@"https://{Constants.MicrosoftProviderUriBase}";
			var languageParams = $"&from={sourceLanguage}&to={targetLanguage}&textType=html&category={category}";

			return string.Concat(uri, path, languageParams);
		}

		private List<string> GetSupportedLanguages()
		{
			try
			{
				_authToken ??= GetAuthToken();
				return TryGetSupportedLanguages();
			}
			catch (WebException exception)
			{
				ErrorHandler.HandleError(exception);
				return null;
			}
		}

		private List<string> TryGetSupportedLanguages()
		{
			var uri = new Uri("https://" + Constants.MicrosoftProviderUriBase);
			var client = new RestClient(uri);

			var request = new RestRequest("languages", Method.Get);
			request.AddParameter("api-version", "3.0");
			request.AddParameter("scope", "translation");

			var languageResponse = client.ExecuteAsync(request).Result;
			var languages = JsonConvert.DeserializeObject<LanguageResponse>(languageResponse.Content);

			var languageCodeList = new List<string>();
			foreach (var language in languages?.Translation)
			{
				languageCodeList.Add(language.Key);
			}

			return languageCodeList;
		}

		private string GetAuthToken()
		{
			string accessToken = null;
			var task = Task.Run(async () =>
			{
				accessToken = await GetAccessTokenAsync();
			});

			while (!task.IsCompleted)
			{
				System.Threading.Thread.Yield();
			}

			if (task.IsFaulted && task.Exception != null)
			{
				throw new Exception(task.Exception.InnerException?.Message);
			}

			if (task.IsCanceled)
			{
				throw new Exception("Timeout obtaining access token.");
			}

			return accessToken;
		}

		private async Task<string> GetAccessTokenAsync()
		{
			if (!string.IsNullOrWhiteSpace(_authToken))
			{
				return _authToken;
			}

			if (string.IsNullOrEmpty(_subscriptionKey))
			{
				return string.Empty;
			}

			var region = string.IsNullOrEmpty(_region) ? "" : _region + ".";
			var uriString = $"https://{region}{Constants.MicrosoftProviderServiceUriBase}/sts/v1.0/issueToken";
			var uri = new Uri(uriString);
			try
			{
				using var client = new HttpClient();
				using var request = new HttpRequestMessage();
				request.Method = HttpMethod.Post;
				request.RequestUri = uri;
				request.Headers.TryAddWithoutValidation(Constants.OcpApimSubscriptionKeyHeader, _subscriptionKey);

				var response = await client.SendAsync(request);
				response.EnsureSuccessStatusCode();
				var tokenString = await response.Content.ReadAsStringAsync();
				var token = ReadToken(tokenString);
				_authToken = "Bearer " + tokenString;
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}\n{ex.Message}\n {ex.StackTrace}");
				throw ex;
			}

			return _authToken;
		}

		private JwtSecurityToken ReadToken(string token)
		{
			if (string.IsNullOrEmpty(token))
			{
				return null;
			}

			try
			{
				var jwtHandler = new JwtSecurityTokenHandler();
				var readableToken = jwtHandler.CanReadToken(token);
				return readableToken ? jwtHandler.ReadJwtToken(token)
									 : null;
			}
			catch { }

			return null;
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