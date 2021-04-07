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
using Newtonsoft.Json;
using NLog;
using RestSharp;
using Sdl.Community.MtEnhancedProvider.Model;

namespace Sdl.Community.MtEnhancedProvider.MstConnect
{
	internal class ApiConnecter
	{
		private const string BaseUri = @"api.cognitive.microsofttranslator.com";
		private const string ServiceBaseUri = @"api.cognitive.microsoft.com";
		private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

		private List<string> _supportedLangs;
		private string _authToken;

		//TODO PACH (06/04/2021): identify if we can enhance the service using this value.
		private DateTime _tokenExpiresAt;

		private string _subscriptionKey;
		private string _region;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// This class allows connection to the Microsoft Translation API
		/// </summary>
		/// <param name="subscriptionKey">Microsoft API key</param>
		/// <param name="region">Region</param>
		internal ApiConnecter(string subscriptionKey, string region)
		{
			_subscriptionKey = subscriptionKey;
			_region = region;

			if (_authToken == null)
			{
				_authToken = GetAuthToken(); //if the class variable has not been set
			}
			if (_supportedLangs == null)
			{
				_supportedLangs = GetSupportedLanguages(); //if the class variable has not been set
			}
		}

		/// <summary>
		/// Allows static credentials to be updated by the calling program
		/// </summary>
		/// <param name="subscriptionKey">the client Id obtained from Microsoft</param>
		/// <param name="region">Region</param>
		internal void ResetCrd(string subscriptionKey, string region)
		{
			if (subscriptionKey != _subscriptionKey || region != _region)
			{
				_subscriptionKey = subscriptionKey;
				_region = region;
				_authToken = GetAuthToken();
				_supportedLangs = GetSupportedLanguages();
			}
		}

		/// <summary>
		/// translates the text input
		/// </summary>
		internal string Translate(string sourceLang, string targetLang, string textToTranslate, string categoryId)
		{
			//convert our language codes
			var sourceLc = ConvertLangCode(sourceLang);
			var targetLc = ConvertLangCode(targetLang);

			//check to see if token is null
			if (_authToken == null) _authToken = GetAuthToken();

			var translatedText = string.Empty;
			try
			{
				//search for words like this <word> 
				var rgx = new Regex("(\\<\\w+[üäåëöøßşÿÄÅÆĞ]*[^\\d\\W\\\\/\\\\]+\\>)");
				var words = rgx.Matches(textToTranslate);
				if (words.Count > 0)
				{
					textToTranslate = ReplaceCharacters(textToTranslate, words);
				}

				const string host = "https://api.cognitive.microsofttranslator.com";
				const string path = "/translate?api-version=3.0";
				var category = categoryId == "" ? "general" : categoryId;
				var languageParams = $"&from={sourceLc}&to={targetLc}&textType=html&category={category}";

				var uri = string.Concat(host, path, languageParams);
				var body = new object[]
				{
					new
					{
						Text =textToTranslate
					}
				};
				var requestBody = JsonConvert.SerializeObject(body);
				using (var httpClient = new HttpClient())
				{
					using (var httpRequest = new HttpRequestMessage())
					{
						httpRequest.Method = HttpMethod.Post;
						httpRequest.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
						httpRequest.RequestUri = new Uri(uri);
						httpRequest.Headers.Add("Authorization", _authToken);

						var response = httpClient.SendAsync(httpRequest).Result;
						var responseBody = response.Content.ReadAsStringAsync().Result;
						if (response.IsSuccessStatusCode)
						{
							var responseTranslation = JsonConvert.DeserializeObject<List<TranslationResponse>>(responseBody);
							translatedText = responseTranslation[0]?.Translations[0]?.Text;
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
				var mesg = ProcessWebException(exception, PluginResources.MsApiFailedGetLanguagesMessage);
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}\n {exception.Message}\n { exception.StackTrace}");
				throw new Exception(mesg);
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
				}
				else
				{
					//check if there is any text after PI
					var remainingText = textToTranslate.Substring(match.Index + match.Length);
					if (!string.IsNullOrEmpty(remainingText))
					{
						//get the position where PI starts to split before
						indexes.Add(match.Index);
						//split after PI
						indexes.Add(match.Index + match.Length);
					}
					else
					{
						indexes.Add(match.Index);
					}
				}
			}
			var splitText = textToTranslate.SplitAt(indexes.ToArray()).ToList();
			var positions = new List<int>();
			for (var i = 0; i < splitText.Count; i++)
			{
				if (!splitText[i].Contains("tg"))
				{
					positions.Add(i);
				}
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

		/// <summary>
		/// Checks of lang pair is supported by MS
		/// </summary>
		internal bool IsSupportedLangPair(string sourceLang, string targetLang)
		{
			//convert our language codes
			var source = ConvertLangCode(sourceLang);
			var target = ConvertLangCode(targetLang);

			var sourceSupported = false;
			var targetSupported = false;

			//check to see if both the source and target languages are supported
			foreach (var lang in _supportedLangs)
			{
				if (lang.Equals(source)) sourceSupported = true;
				if (lang.Equals(target)) targetSupported = true;
			}

			if (sourceSupported && targetSupported) return true; //if both are supported return true

			//otherwise return false
			return false;
		}

		private List<string> GetSupportedLanguages()
		{
			//check to see if token is null
			if (_authToken == null) _authToken = GetAuthToken();

			var languageCodeList = new List<string>();
			try
			{
				var uri = new Uri("https://" + BaseUri);
				var client = new RestClient(uri);

				var request = new RestRequest("languages", Method.GET);
				request.AddParameter("api-version", "3.0");
				request.AddParameter("scope", "translation");

				var languageResponse = client.Execute(request).Content;
				var languages = JsonConvert.DeserializeObject<LanguageResponse>(languageResponse);
				if (languages != null)
				{
					foreach (var language in languages.Translation)
					{
						languageCodeList.Add(language.Key);
					}
				}
			}
			catch (WebException exception)
			{
				var mesg = ProcessWebException(exception, PluginResources.MsApiFailedGetLanguagesMessage);
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}\n{exception.Message}\n { exception.StackTrace}");
				throw new Exception(mesg);
			}
			return languageCodeList;
		}

		private string ProcessWebException(WebException e, string message)
		{
			_logger.Error($"{MethodBase.GetCurrentMethod().Name}\n{e.Response}\n {message}");

			// Obtain detailed error information
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
			if (task.IsFaulted)
			{
				if (task.Exception != null) throw new Exception(task.Exception.InnerException?.Message);
			}
			if (task.IsCanceled)
			{
				throw new Exception("Timeout obtaining access token.");
			}
			return accessToken;
		}

		public void RefreshAuthToken()
		{
			//Clear the existing token because the api key has changed
			_authToken = string.Empty;

			//try to get the token for the new api key
			_authToken = GetAuthToken();
		}

		public async Task<string> GetAccessTokenAsync()
		{
			if (!string.IsNullOrWhiteSpace(_authToken)) return _authToken;
			if (string.IsNullOrEmpty(_subscriptionKey)) return string.Empty;


			var uri = new Uri("https://"
							  + (string.IsNullOrEmpty(_region) ? "" : _region + ".")
							  + ServiceBaseUri + "/sts/v1.0/issueToken");

			try
			{
				using (var client = new HttpClient())
				using (var request = new HttpRequestMessage())
				{
					request.Method = HttpMethod.Post;
					request.RequestUri = uri;
					request.Headers.TryAddWithoutValidation(OcpApimSubscriptionKeyHeader, _subscriptionKey);
					var response = await client.SendAsync(request);
					response.EnsureSuccessStatusCode();
					var tokenString = await response.Content.ReadAsStringAsync();

					_authToken = "Bearer " + tokenString;

					var token = ReadToken(tokenString);
					_tokenExpiresAt = token?.ValidTo ?? DateTime.Now;

				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}\n{ex.Message}\n { ex.StackTrace}");
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

				//Check if readable token (string is in a JWT format)
				var readableToken = jwtHandler.CanReadToken(token);
				if (!readableToken)
				{
					return null;
				}

				return jwtHandler.ReadJwtToken(token);
			}
			catch
			{
				// catch all; ignore
			}

			return null;
		}

		private string ConvertLangCode(string languageCode)
		{
			//takes the language code input and converts it to one that MS Translate can use
			if (languageCode.Contains("sr-Cyrl")) return "sr-Cyrl";
			if (languageCode.Contains("sr-Latn")) return "sr-Latn";

			var ci = new CultureInfo(languageCode); //construct a CultureInfo object with the language code

			//deal with chinese..MS Translator has different ones
			if (new[] { "zh-TW", "zh-HK", "zh-MO", "zh-Hant", "zh-CHT" }.Contains(ci.Name)) return "zh-Hant";
			if (new[] { "zh-CN", "zh-SG", "zh-Hans-HK", "zh-Hans-MO", "zh-Hans", "zh-CHS" }.Contains(ci.Name)) return "zh-Hans";

			return ci.TwoLetterISOLanguageName;

		}
	}
}
