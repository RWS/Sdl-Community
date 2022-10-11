using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MTEnhancedMicrosoftProvider.Extensions;
using MTEnhancedMicrosoftProvider.Model;
using MTEnhancedMicrosoftProvider.Service;
using Newtonsoft.Json;
using NLog;
using RestSharp;

namespace MTEnhancedMicrosoftProvider.Connect
{
	internal class ApiConnecter
	{
		private const string BaseUri = @"api.cognitive.microsofttranslator.com";
		private const string ServiceBaseUri = @"api.cognitive.microsoft.com";
		private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private string _subscriptionKey;
		private string _region;
		private HtmlUtil _htmlUtil;
		private string _authToken;
		private List<string> _supportedLangs;

		/// <summary>
		/// This class allows connection to the Microsoft Translation API
		/// </summary>
		/// <param name="subscriptionKey">Microsoft API key</param>
		/// <param name="region">Region</param>
		internal ApiConnecter(string subscriptionKey, string region, HtmlUtil htmlUtil)
		{
			_subscriptionKey = subscriptionKey;
			_region = region;
			_htmlUtil = htmlUtil;

			_authToken = _authToken is null ? GetAuthToken() : _authToken;
			_supportedLangs = _supportedLangs is null ? GetSupportedLanguages() : _supportedLangs;
		}

		internal void ResetCredentials(string subscriptionKey, string region)
		{
			if (subscriptionKey != _subscriptionKey
				|| region != _region)
			{
				_subscriptionKey = subscriptionKey;
				_region = region;
				_authToken = GetAuthToken();
				_supportedLangs = GetSupportedLanguages();
			}
		}

		internal string Translate(string sourceLang, string targetLang, string textToTranslate, string categoryId)
		{
			if (_authToken is null)
			{
				_authToken = GetAuthToken();
				if (_authToken is null)
				{
					throw new Exception("Authorization token not valid!");
				}
			}

			const string host = "https://api.cognitive.microsofttranslator.com";
			const string path = "/translate?api-version=3.0";
			var translatedText = string.Empty;
			try
			{
				var expression = new Regex("(\\<\\w+[üäåëöøßşÿÄÅÆĞ]*[^\\d\\W\\\\/\\\\]+\\>)");
				var matches = expression.Matches(textToTranslate);
				if (matches.Count > 0)
				{
					textToTranslate = ReplaceCharacters(textToTranslate, matches);
				}

				var body = new object[] { new { Text = textToTranslate } };
				var requestBody = JsonConvert.SerializeObject(body);
				using (var httpClient = new HttpClient())
				{
					using (var httpRequest = new HttpRequestMessage())
					{
						var convertedSource = ConvertLangCode(sourceLang);
						var convertedTarget = ConvertLangCode(targetLang);
						var category = string.IsNullOrEmpty(categoryId) ? "general" : categoryId;
						var languageParams = $"&from={convertedSource}&to={convertedTarget}&textType=html&category={category}";
						var uri = string.Concat(host, path, languageParams);

						httpRequest.Method = HttpMethod.Post;
						httpRequest.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
						httpRequest.RequestUri = new Uri(uri);
						httpRequest.Headers.Add("Authorization", _authToken);

						var response = httpClient.SendAsync(httpRequest).Result;
						var responseBody = response.Content.ReadAsStringAsync().Result;
						if (response.IsSuccessStatusCode)
						{
							var responseTranslation = JsonConvert.DeserializeObject<List<TranslationResponse>>(responseBody);
							translatedText = _htmlUtil.HtmlDecode(responseTranslation[0]?.Translations[0]?.Text);
						}
						else
						{
							var responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(responseBody);
							throw new Exception(responseMessage.Error.Message);
						}
					}
				}
			}
			catch (WebException exception)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}\n {exception.Message}\n {exception.StackTrace}");

				var message = ProcessWebException(exception, PluginResources.MsApiFailedGetLanguagesMessage);
				throw new Exception(message);
			}

			return translatedText;
		}

		private string ReplaceCharacters(string textToTranslate, MatchCollection matches)
		{
			var indexes = new List<int>();
			foreach (Match match in matches)
			{
				if (match.Index.Equals(0))
				{
					indexes.Add(match.Length);
					continue;
				}

				indexes.Add(match.Index);
				var remainingText = textToTranslate.Substring(match.Index + match.Length);
				if (!string.IsNullOrEmpty(remainingText))
				{
					indexes.Add(match.Index + match.Length);
				}
			}

			var splitText = textToTranslate.SplitAt(indexes.ToArray()).ToList();
			var positions = new List<int>();
			for (var i = 0; i < splitText.Count; i++)
			{
				if (splitText[i].Contains("tg"))
				{
					continue;
				}

				positions.Add(i);
			}

			foreach (var position in positions)
			{
				var originalString = splitText[position];
				var start = Regex.Replace(originalString, "<", "&lt;");
				var finalString = Regex.Replace(start, ">", "&gt;");
				splitText[position] = finalString;
			}

			var finalText = string.Empty;
			foreach (var text in splitText)
			{
				finalText += text;
			}

			return finalText;
		}

		internal bool IsSupportedLangPair(string sourceLang, string targetLang)
		{
			var convertedSource = ConvertLangCode(sourceLang);
			var convertedTarget = ConvertLangCode(targetLang);

			var sourceSupported = false;
			var targetSupported = false;
			foreach (var lang in _supportedLangs)
			{
				if (lang.Equals(convertedSource))
				{
					sourceSupported = true;
				}

				if (lang.Equals(convertedTarget))
				{
					targetSupported = true;
				}
			}

			return sourceSupported && targetSupported;
		}

		private List<string> GetSupportedLanguages()
		{
			_authToken = _authToken is null ? GetAuthToken() : _authToken;

			var languageCodeList = new List<string>();
			try
			{
				var request = new RestRequest("languages", Method.Get);
				request.AddParameter("api-version", "3.0");
				request.AddParameter("scope", "translation");

				var client = new RestClient(new Uri("https://" + BaseUri));
				var languageResponse = client.ExecuteAsync(request).Result;
				var languages = JsonConvert.DeserializeObject<LanguageResponse>(languageResponse.Content);
				languageCodeList.AddRange(languages?.Translation.Select(language => language.Key));
			}
			catch (WebException exception)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}\n{exception.Message}\n {exception.StackTrace}");

				var message = ProcessWebException(exception, PluginResources.MsApiFailedGetLanguagesMessage);
				throw new Exception(message);
			}

			return languageCodeList;
		}

		private string ProcessWebException(WebException e, string message)
		{
			_logger.Error($"{MethodBase.GetCurrentMethod().Name}\n{e.Response}\n {message}");

			string strResponse;
			using (var response = (HttpWebResponse)e.Response)
			{
				using (var responseStream = response.GetResponseStream())
				{
					using (var sr = new StreamReader(responseStream, Encoding.ASCII))
					{
						strResponse = sr.ReadToEnd();
					}
				}
			}

			return $"Http status code={e.Status}, error message={strResponse}";
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

			if (task.IsFaulted
				&& task.Exception != null)
			{
				throw new Exception(task.Exception.InnerException?.Message);
			}

			if (task.IsCanceled)
			{
				throw new Exception("Timeout obtaining access token.");
			}

			return accessToken;
		}

		public void RefreshAuthToken()
		{
			_authToken = string.Empty;
			_authToken = GetAuthToken();
		}

		public async Task<string> GetAccessTokenAsync()
		{
			if (!string.IsNullOrWhiteSpace(_authToken))
			{
				return _authToken;
			}

			if (string.IsNullOrEmpty(_subscriptionKey))
			{
				return string.Empty;
			}

			try
			{
				using (var client = new HttpClient())
				using (var request = new HttpRequestMessage())
				{
					request.Method = HttpMethod.Post;
					request.Headers.TryAddWithoutValidation(OcpApimSubscriptionKeyHeader, _subscriptionKey);
					request.RequestUri = new Uri(
						$"https://{(string.IsNullOrEmpty(_region) ? "" : _region + ".")}{ServiceBaseUri}/sts/v1.0/issueToken");
					var response = await client.SendAsync(request);
					response.EnsureSuccessStatusCode();
					var tokenString = await response.Content.ReadAsStringAsync();
					_authToken = "Bearer " + tokenString;
					var token = ReadToken(tokenString);
				}
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
				return jwtHandler.CanReadToken(token) ? jwtHandler.ReadJwtToken(token)
													  : null;
			}
			catch { }

			return null;
		}

		private string ConvertLangCode(string languageCode)
		{
			const string traditionalChineseCodes = "zh-TW zh-HK zh-MO zh-Hant zh-CHT";
			const string simplifiedChineseCodes = "zh-CN zh-SG zh-Hans-HK zh-Hans-MO zh-Hans zh-CHS";

			var cultureInfo = new CultureInfo(languageCode);
			var isLatin = languageCode.Contains("sr-Latn");
			var isCyrillic = languageCode.Contains("sr-Cyrl");
			var isTraditionalChinese = traditionalChineseCodes.Contains(cultureInfo.Name);
			var isSimplifiedChinese = simplifiedChineseCodes.Contains(cultureInfo.Name);

			if (isCyrillic)
			{
				return "sr-Cyrl";
			}
			else if (isLatin)
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