using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using LanguageMappingProvider.Model;
using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.Studio.TranslationProvider
{
	public class MicrosoftApi
	{
		private readonly HtmlUtil _htmlUtil = new();
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly ITranslationOptions _options;

		private string _region;
		private string _subscriptionKey;
		private HashSet<string> _supportedLanguages;

		public MicrosoftApi(ITranslationOptions options)
		{
			_options = options;
			_subscriptionKey = options.ApiKey;
			_region = options.Region;
			SetSupportedLanguages();
		}

		public MicrosoftApi(string subscriptionKey, string region)
		{
			_subscriptionKey = subscriptionKey;
			_region = region;
			SetSupportedLanguages();
		}

		public List<LanguageMapping> GetSupportedLanguages()
		{
			try
			{
				return TryGetSupportedLanguages();
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleError(ex);
				return null;
			}
		}

		public bool IsSupportedLanguagePair(string sourceLanguage, string tarrgetLanguage)
		{
			var sourceCode = ConvertLanguageCode(sourceLanguage);
			var targetCode = ConvertLanguageCode(tarrgetLanguage);
			var sourceSupported = _supportedLanguages.TryGetValue(sourceCode, out _);
			var targetSupported = _supportedLanguages.TryGetValue(targetCode, out _);
			return sourceSupported && targetSupported;
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
			SetSupportedLanguages();
		}

		public string Translate(LanguagePair languagepair, string textToTranslate)
		{
			try
			{
				var mapping = _options.LanguageMappings.FirstOrDefault(x => x.LanguagePair.TargetCultureName == languagepair.TargetCultureName);
				var sourceLanguage = DatabaseExtensions.GetLanguageCode(new CultureInfo(languagepair.SourceCultureName));
				var targetLanguage = DatabaseExtensions.GetLanguageCode(new CultureInfo(languagepair.TargetCultureName));
				var categoryId = string.IsNullOrEmpty(mapping.CategoryID) ? "general" : mapping.CategoryID;
				return TryTranslate(sourceLanguage, targetLanguage, textToTranslate, categoryId);
			}
			catch (WebException webException)
			{
				ErrorHandler.HandleError(webException);
				return null;
			}
			catch (Exception exception)
			{
				if (exception.Message.Contains("The category parameter is invalid"))
				{
					ErrorHandler.HandleError("The Category ID is not valid for this language pair", "Category ID");
					return null;
				}

				ErrorHandler.HandleError(exception);
				return null;
			}
		}

		private string BuildTranslationUri(string sourceLanguage, string targetLanguage, string category)
		{
			const string path = "/translate?api-version=3.0";
			const string uri = $@"https://{Constants.MicrosoftProviderUriBase}";
			var languageParams = $"&from={sourceLanguage}&to={targetLanguage}&textType=html&category={category}";

			return string.Concat(uri, path, languageParams);
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

			httpRequest.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
			httpRequest.Headers.Add("Ocp-Apim-Subscription-Region", _region);

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

		private void SetSupportedLanguages()
		{
			try
			{
				TrySetSupportedLanguages();
			}
			catch (Exception exception)
			{
				ErrorHandler.HandleError(exception);
				return;
			}
		}

		private List<LanguageMapping> TryGetSupportedLanguages()
		{
			var uri = new Uri("https://" + Constants.MicrosoftProviderUriBase);
			var client = new RestClient(uri);

			var request = new RestRequest("languages", Method.Get);
			request.AddParameter("api-version", "3.0");
			request.AddParameter("scope", "translation");

			var languageResponse = client.ExecuteAsync(request).Result;
			if (!languageResponse.IsSuccessful)
				throw new HttpException($"Error: {languageResponse.StatusCode}, {languageResponse.StatusDescription}");

			var languages = JsonConvert.DeserializeObject<LanguageResponse>(languageResponse.Content)?.Translation?.Distinct();

			var output = new List<LanguageMapping>();
			foreach (var language in languages)
			{
				output.Add(new()
				{
					Name = language.Value.Name,
					LanguageCode = language.Key
				});
			}

			return output;
		}

		private void TrySetSupportedLanguages()
		{
			var languages = GetSupportedLanguages();
			_supportedLanguages = new();
			foreach (var language in languages)
			{
				_supportedLanguages.Add(language.LanguageCode);
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
	}
}