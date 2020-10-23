using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Sdl.LC.AddonBlueprint.Exceptions;
using Sdl.LC.AddonBlueprint.Helpers;
using Sdl.LC.AddonBlueprint.Interfaces;
using Sdl.LC.AddonBlueprint.Models;

namespace Sdl.LC.AddonBlueprint.Services
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
				using var httpClient = new HttpClient
				{
					Timeout = TimeSpan.FromMinutes(5)
				};

				var content = new StringContent($"text={text}" +
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

					//translationsResponse.Translations.Add(translatedText);
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
