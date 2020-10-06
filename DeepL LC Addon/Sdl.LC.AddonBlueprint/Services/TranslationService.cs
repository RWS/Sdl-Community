using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Sdl.LC.AddonBlueprint.Enums;
using Sdl.LC.AddonBlueprint.Helpers;
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
		/// <returns>List of <see cref="TranslationEngineResponse"/>/returns>
		public async Task<List<TranslationEngineResponse>> GetCorrespondingEngines(string apiKey, string sourceLanguageCode, List<string> targetLanguagesCode)
		{
			var deeplAvailableSourceLanguages = await GetAvailableDeeplLanguages(apiKey, LanguageEnum.source);
			var deepLAvailableTargetLanguages = await GetAvailableDeeplLanguages(apiKey, LanguageEnum.target);

			var sourceEngineCode = GetLcCorrespondingSourceLanguageCodeEngine("en-us", deeplAvailableSourceLanguages);
			return null;
		}

		/// <summary>
		/// Gets the corresponding language code for source
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
		/// Used for translation step
		/// Get the language based on availability in DeepL; if we have a flavour use that, otherwise use general culture of that flavour (two letter iso) if available, otherwise return null
		/// (e.g. for Portuguese, the leftLanguageTag (pt-PT or pt-BR) should be used, so the translations will correspond to the specific language flavor) 
		/// </summary>
		/// <param name="culture">Language for which we search the availability</param>
		/// <param name="languageList">DeepL Languages list available</param>
		private string GetLanguage(CultureInfo culture, List<string> languageList)
		{
			if (languageList != null && languageList.Any())
			{
				var leftLangTag = culture.IetfLanguageTag.ToUpperInvariant();
				var twoLetterIso = culture.TwoLetterISOLanguageName.ToUpperInvariant();

				var selectedTargetLanguage = languageList.FirstOrDefault(tl => tl == leftLangTag) ?? languageList.FirstOrDefault(tl => tl == twoLetterIso);

				return selectedTargetLanguage ?? (languageList.Any(tl => tl.Contains(twoLetterIso)) ? twoLetterIso : null);
			}

			return string.Empty;
		}
	}
}
