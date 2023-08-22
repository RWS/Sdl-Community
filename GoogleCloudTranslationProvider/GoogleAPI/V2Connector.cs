using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using GoogleCloudTranslationProvider.Extensions;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Models;
using GoogleCloudTranslationProvider.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using LogManager = NLog.LogManager;

namespace GoogleCloudTranslationProvider.GoogleAPI
{
	public class V2Connector
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly HtmlUtil _htmlUtil;

		private Dictionary<string, List<string>> supportedLanguages;

		public V2Connector(string apiKey, HtmlUtil htmlUtil)
		{
			ApiKey = apiKey;
			_htmlUtil = htmlUtil;
		}

		public string ApiKey { get; set; }

		public async Task<List<V2LanguageModel>> GetLanguages()
		{
			var url = $"https://translation.googleapis.com/language/translate/v2/languages?key={ApiKey}&target=en";
			var httpClient = new HttpClient();
			var response = httpClient.GetAsync(url).Result;

			if (response.IsSuccessStatusCode)
			{
				var jsonResponse = response.Content.ReadAsStringAsync().Result;
				var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

				var languages = result.data.languages;

				var output = new List<V2LanguageModel>();
				foreach (var language in languages)
				{
					output.Add(new()
					{
						LanguageCode = language.language,
						LanguageName = language.name,
					});
				}

				return output;
			}
			else
			{
				// Handle the case where the request was not successful
				throw new Exception("Failed to retrieve the list of languages.");
			}
		}

		public bool CredentialsAreValid()
		{
			try
			{
				DownloadRequest(Constants.TranslationUri, "de", "test");
				return true;
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {ex.Message}\n {ex.StackTrace}");
				ErrorHandler.HandleError(ex);
				return false;
			}
		}

		public string Translate(LanguagePair languagePair, string text, string format = null)
		{
			text = format is null ? HttpUtility.HtmlEncode(text) : text;
			format ??= "html";
			return DoTranslate(languagePair, text, format);
		}

		public bool IsSupportedLanguagePair(CultureCode sourceCulture, CultureCode targetCulture)
		{
			supportedLanguages ??= new Dictionary<string, List<string>>();
			var sourceLanguage = sourceCulture.GetLanguageCode(ApiVersion.V2);
			var targetLanguage = targetCulture.GetLanguageCode(ApiVersion.V2);

			if (string.IsNullOrEmpty(sourceLanguage) || string.IsNullOrEmpty(targetLanguage))
			{
				return false;
			}

			if (!supportedLanguages.ContainsKey(targetLanguage))
			{
				UpdateSupportedLanguages(targetLanguage);
			}

			return supportedLanguages[targetLanguage].Any(source => source == sourceLanguage);
		}

		private void UpdateSupportedLanguages(string target)
		{
			if (GetSourceLanguages(target) is not List<string> sourceLanguages)
			{
				var message = PluginResources.LangPairAuthErrorMsg1 + Environment.NewLine + PluginResources.LangPairAuthErrorMsg2;
				throw new Exception(message);
			}

			if (!supportedLanguages.ContainsKey(target))
			{
				supportedLanguages.Add(target, sourceLanguages);
			}
		}

		private List<string> GetSourceLanguages(string targetLanguage)
		{
			try
			{
				return TryGetSourceLanguages(targetLanguage);
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {e.Message}\n {e.StackTrace}");
				ErrorHandler.HandleError(e);
			}

			return new List<string>();
		}

		private List<string> TryGetSourceLanguages(string targetLanguage)
		{
			var result = DownloadRequest(Constants.LanguagesUri, targetLanguage);
			return GetLanguageCodes(result);
		}

		private List<string> GetLanguageCodes(string input)
		{
			var jsonObject = JObject.Parse(input)["data"]["languages"].ToList();
			return jsonObject.Select(language => language["language"].ToString()).ToList();
		}

		private string DoTranslate(LanguagePair languagePair, string text, string format)
		{
			try
			{
				return TryDoTranslate(languagePair, text, format);
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} {e.Message}\n {e.StackTrace}");
				if (text.Length >= 5000)
				{
					throw new Exception(string.Format(PluginResources.V2Api_CharacterLimit, text.Length));
				}

				throw new Exception(e.Message);
			}
		}

		private string TryDoTranslate(LanguagePair languagePair, string text, string format)
		{
			if (string.IsNullOrEmpty(ApiKey))
			{
				throw new Exception(PluginResources.ApiConnectionGoogleNoKeyErrorMessage);
			}

			var targetCode = languagePair.TargetCulture.GetLanguageCode(ApiVersion.V2);
			var result = DownloadRequest(Constants.TranslationUri, targetCode, text, format);
			var returnedResult = GetTranslation(result);
			var decodedResult = _htmlUtil.HtmlDecode(returnedResult).RemoveZeroWidthSpaces();
			return decodedResult;
		}

		private string DownloadRequest(string uri, string targetLanguage, string text = null, string format = null)
		{
			var url = string.Format(
				"{0}?key={1}{2}&target={3}{4}",
				uri,
				ApiKey,
				text is not null ? $"&q={text.EncodeSpecialChars()}" : string.Empty,
				targetLanguage,
				format is not null ? $"&format={format}" : string.Empty);

			var webClient = new WebClient() { Encoding = Encoding.UTF8 };
			return webClient.DownloadString(url);
		}

		private string GetTranslation(string input)
		{
			var jsonObject = JObject.Parse(input)["data"]["translations"];
			return jsonObject[0]["translatedText"].ToString();
		}
	}
}