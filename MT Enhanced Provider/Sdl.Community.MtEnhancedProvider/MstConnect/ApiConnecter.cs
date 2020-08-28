using System;
using System.Collections.Generic;
using System.Globalization;
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
using Sdl.Community.MtEnhancedProvider.Helpers;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;

namespace Sdl.Community.MtEnhancedProvider.MstConnect
{
	internal class ApiConnecter
	{
		private static string _authToken;
		private static DateTime _tokenExpiresAt; //to keep track of when token expires
		public static List<string> SupportedLangs { get; set; }
		private string _subscriptionKey;
		private const string TranslatorUri = @"https://api.cognitive.microsofttranslator.com/";
		private static readonly Uri ServiceUrl = new Uri("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");
		private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// This class allows connection to the Microsoft Translation API
		/// </summary>
		/// <param name="options"></param>
		internal ApiConnecter(IMtTranslationOptions options)
		{
			_subscriptionKey = options.ClientId;
			if (_authToken == null)
			{
				_authToken = GetAuthToken(); //if the class variable has not been set
			}
			if (SupportedLangs == null)
			{
				SupportedLangs = GetSupportedLanguages(); //if the class variable has not been set
			}
		}

		/// <summary>
		/// Allows static credentials to be updated by the calling program
		/// </summary>
		/// <param name="cid">the client Id obtained from Microsoft</param>
		/// <param name="cst">the client secret obtained from Microsoft</param>
		internal void ResetCrd(string cid)
		{
			_subscriptionKey = cid;
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
			//check to see if token expired and if so, get a new one
			if (DateTime.Now.CompareTo(_tokenExpiresAt) >= 0) _authToken = GetAuthToken();
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
							throw  new Exception(responseMessage.Error.Message);
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
			foreach (var lang in SupportedLangs)
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
			//check to see if token expired and if so, get a new one
			if (DateTime.Now.CompareTo(_tokenExpiresAt) >= 0) _authToken = GetAuthToken();

			var languageCodeList = new List<string>();
			try
			{
				var client = new RestClient(TranslatorUri);
				var request = new RestRequest("languages", Method.GET);
				request.AddHeader("Authorization", _authToken);
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
					using (var sr = new StreamReader(responseStream, System.Text.Encoding.ASCII))
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
				//throw task.Exception;
			}
			if (task.IsCanceled)
			{
				throw new Exception("Timeout obtaining access token.");
			}
			return accessToken;
		}

		public async Task<string> GetAccessTokenAsync()
		{
			if (!string.IsNullOrWhiteSpace(_authToken)) return _authToken;
			if (string.IsNullOrEmpty(_subscriptionKey)) return string.Empty;

			using (var client = new HttpClient())
			using (var request = new HttpRequestMessage())
			{
				request.Method = HttpMethod.Post;
				request.RequestUri = ServiceUrl;
				request.Headers.TryAddWithoutValidation(OcpApimSubscriptionKeyHeader, _subscriptionKey);
				var response = await client.SendAsync(request);
				response.EnsureSuccessStatusCode();
				var token = await response.Content.ReadAsStringAsync();
				_tokenExpiresAt = DateTime.Now;
				_authToken = "Bearer " + token;
				return _authToken;
			}
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
