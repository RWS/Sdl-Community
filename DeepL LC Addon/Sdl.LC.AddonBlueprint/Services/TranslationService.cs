using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Sdl.Community.DeeplAddon.Exceptions;
using Sdl.Community.DeeplAddon.Helpers;
using Sdl.Community.DeeplAddon.Interfaces;
using Sdl.Community.DeeplAddon.Models;

namespace Sdl.Community.DeeplAddon.Services
{
	public class TranslationService : ITranslationService
	{
		private readonly ILanguageService _languageService;

		public TranslationService(ILanguageService languageService)
		{
			_languageService = languageService;
		}

		public async Task<TranslationResponse> Translate(TranslationRequest translationRequest, string apiKey,string formality)
		{
			var translationEngine = _languageService.GetLanguagesFromEngineId(translationRequest.EngineId);

			var translationsResponse = new TranslationResponse
			{
				Translations = new List<string>()
			};

			var translations = GetTranslations(translationRequest, translationEngine, apiKey, formality);

			translationsResponse.Translations.AddRange(translations);

			return translationsResponse;
		}

		private List<string> GetTranslations(TranslationRequest translationRequest, TranslationEngine translationEngine, string apiKey, string formality)
		{
			var translations = new List<string>();

			Parallel.ForEach(translationRequest.Contents, (text) =>
			{
				var encodedText = HttpUtility.UrlEncode(text);
				using var httpClient = new HttpClient();			

				var content = new StringContent($"text={encodedText}" +
												$"&source_lang={translationEngine.EngineSourceLanguage}" +
												$"&target_lang={translationEngine.EngineTargetLanguage}" +
												$"&formality={formality.ToLower()}" +
												"&preserve_formatting=1" +
												"&tag_handling=xml" +
												$"&auth_key={apiKey}", Encoding.UTF8, "application/x-www-form-urlencoded");

				var response =  httpClient.PostAsync("https://api.deepl.com/v2/translate", content).Result;
				if (response.IsSuccessStatusCode)
				{
					var translationResponse =  response.Content?.ReadAsStringAsync().Result;
					var translatedObject = JsonSerializer.Deserialize<DeeplTranslationResponse>(translationResponse, JsonSettings.Default());
					var translatedText = translatedObject.Translations[0].Text;

					translations.Add(translatedText);
				}
				else
				{
					var responseContent = response.Content?.ReadAsStringAsync().Result;

					throw new AddonValidationException($"Request to DeepL Translate endpoint failed with status code {response.StatusCode}", new Details { Code = ErrorCodes.GeneralError, Value = responseContent });
				}
			});
			return translations;
		}
	}
}
