using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Sdl.LC.AddonBlueprint.Enums;
using Sdl.LC.AddonBlueprint.Interfaces;
using Sdl.LC.AddonBlueprint.Models;

namespace Sdl.LC.AddonBlueprint.Services
{
	public class TranslationService : ITranslationService
	{
		private readonly string LanguagesUrl = "https://api.deepl.com/v1/languages";

		public async Task<List<string>> GetAvailableDeeplLanguages(string apiKey, LanguageEnum languageType)
		{
			try
			{
				using var httpClient = new HttpClient();
				var languageList = new List<string>();
				var content = new StringContent($"type={languageType}&auth_key={apiKey}", Encoding.UTF8, "application/x-www-form-urlencoded");
				var responseMessage = await httpClient.PostAsync(LanguagesUrl, content);

				if (responseMessage.IsSuccessStatusCode)
				{
					var languagesResponse = await responseMessage.Content?.ReadAsStringAsync();

					using (var document = JsonDocument.Parse(languagesResponse))
					{
						var array = document.RootElement.EnumerateArray();
						foreach (var element in array)
						{
							var success = element.TryGetProperty("language", out var codeElement);
							if (success)
							{
								languageList.Add(codeElement.GetString().ToLower());
							}
						}
					}
					return languageList;
				}
				else
				{
					var responseContent = await responseMessage.Content?.ReadAsStringAsync();
					var message = $"Request to DeepL Translate REST API endpoint failed with status code {responseMessage.StatusCode}. Response content: {responseContent}";

					throw new HttpRequestException(message);
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Based on the source and target languages available in DeepL create the response which we'll send to LC
		/// </summary>
		/// <param name="apiKey">DeepL API Key</param>
		/// <param name="sourceLanguageCode">LC source language code</param>
		/// <param name="targetLanguagesCode">LC list of target language codes</param>
		/// <returns><see cref="TranslationEngineResponse"/>/returns>
		public async Task<TranslationEngineResponse> GetCorrespondingEngines(string apiKey, string sourceLanguageCode, List<string> targetLanguagesCode)
		{
			var deeplAvailableSourceLanguages = await GetAvailableDeeplLanguages(apiKey, LanguageEnum.source);
			var deeplAvailableTargetLanguages = await GetAvailableDeeplLanguages(apiKey, LanguageEnum.target);

			var sourceEngineCode = GetLcCorrespondingSourceLanguageCodeEngine(sourceLanguageCode, deeplAvailableSourceLanguages);
			var targetEngineCodes = GetLCCorrespondingTargetLanguagesEngineCode(targetLanguagesCode, deeplAvailableTargetLanguages);

			var translationEngines = new List<TranslationEngine>();

			foreach (var targetCode in targetEngineCodes)
			{
				var engine = new TranslationEngine
				{
					//Id = Guid.NewGuid().ToString(),
					//Model = "nmt",
					//MatchingSourceLanguage = sourceEngineCode,
					//EngineSourceLanguage = sourceEngineCode,
					//EngineTargetLanguage = targetCode,
					//MatchingTargetLanguages = new List<string> { targetCode },

					//Id = Guid.NewGuid().ToString(),
					//Model = "nmt",
					//MatchingSourceLanguage = "en",
					//EngineSourceLanguage = "en",
					//EngineTargetLanguage = "fr",
					//MatchingTargetLanguages = new List<string> { "fr-fr","fr-ca" },
				};
				translationEngines.Add(engine);
			}

			var translationEngineResponse = new TranslationEngineResponse
			{
				Items = translationEngines,
				ItemCount = translationEngines.Count
			};
			return translationEngineResponse;
		}

		/// <summary>
		/// Gets the corresponding language code for target language
		/// </summary>
		/// <param name="targetEngineCodes">LC target languages code</param>
		/// <param name="deeplAvailableTargetLanguages">List of the DeepL Target language</param>
		/// <returns>List of LC codes which are supported by DeepL</returns>
		private List<string> GetLCCorrespondingTargetLanguagesEngineCode(List<string> targetEngineCodes, List<string> deeplAvailableTargetLanguages)
		{
			var matchingTargetLanguageCode = new List<string>();

			foreach (var lcLanguageCode in targetEngineCodes)
			{
				var matchingLanguage = GetLanguage(new CultureInfo(lcLanguageCode), deeplAvailableTargetLanguages);
				if (!string.IsNullOrEmpty(matchingLanguage))
				{
					matchingTargetLanguageCode.Add(lcLanguageCode);
				}
			}

			return matchingTargetLanguageCode;
		}

		/// <summary>
		/// Gets the corresponding language code for source language
		/// </summary>
		/// <param name="sourceLanguageCode">LC source language code</param>
		/// <param name="deeplAvailableLanguages">List of the DeepL Source language</param>
		/// <returns>Name of the language e.g "en-us"</returns>
		private string GetLcCorrespondingSourceLanguageCodeEngine(string sourceLanguageCode, List<string> deeplAvailableLanguages)
		{
			var sourceCultureInfo = new CultureInfo(sourceLanguageCode);
			var isSourceLanguageAvailable = deeplAvailableLanguages.Any(d => d.Equals(sourceCultureInfo.TwoLetterISOLanguageName));
			if (isSourceLanguageAvailable)
			{
				return sourceLanguageCode;
			}

			return string.Empty;
		}

		/// <summary>
		/// In target languages we also have flavours of the language "en-us", "en-gb"
		/// Get the language based on availability in DeepL; if we have a flavour use that, otherwise use general culture of that flavour (two letter iso) if available, otherwise return null
		/// (e.g. for Portuguese, the leftLanguageTag (pt-PT or pt-BR) should be used, so the translations will correspond to the specific language flavor) 
		/// </summary>
		/// <param name="culture">Language for which we search the availability</param>
		/// <param name="languageList">DeepL Languages list available</param>
		private string GetLanguage(CultureInfo culture, List<string> languageList)
		{
			if (languageList != null && languageList.Any())
			{
				var leftLangTag = culture.IetfLanguageTag.ToLowerInvariant();
				var twoLetterIso = culture.TwoLetterISOLanguageName;

				var selectedTargetLanguage = languageList.FirstOrDefault(tl => tl == leftLangTag) ?? languageList.FirstOrDefault(tl => tl == twoLetterIso);

				return selectedTargetLanguage ?? (languageList.Any(tl => tl.Contains(twoLetterIso)) ? twoLetterIso : null);
			}

			return string.Empty;
		}
	}
}
