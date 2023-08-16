using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Sdl.Community.DeepLMTProvider.Helpers;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider.Client
{
	public class DeepLTranslationProviderClient
	{
		private static readonly Logger Logger = Log.GetLogger(nameof(DeepLTranslationProviderClient));
		private static string _apiKey;
		private List<string> _supportedSourceLanguages;

		public static event EventHandler ApiKeyChanged;

		public DeepLTranslationProviderClient(string key)
		{
			ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			ApiKey = key;
		}

		public static string ApiKey
		{
			get => _apiKey;
			set
			{
				_apiKey = value;
				OnApiKeyChanged();
			}
		}

		public static HttpResponseMessage IsApiKeyValidResponse { get; private set; }
		private static List<string> SupportedTargetLanguages { get; set; }
		private static Dictionary<string, bool> SupportedTargetLanguagesAndFormalities { get; set; }

		private List<string> SupportedSourceLanguages => _supportedSourceLanguages ??= GetSupportedSourceLanguages(ApiKey);

		public static List<string> GetFormalityIncompatibleLanguages(List<CultureInfo> targetLanguages)
		{
			return targetLanguages.Where(tl => !IsLanguageCompatible(tl)).Select(tl => tl.Name).ToList();
		}

		public static List<string> GetSupportedSourceLanguages(string apiKey)
		{
			var supportedLanguages = new List<string>();
			try
			{
				var response = GetSupportedLanguages("source", apiKey);
				supportedLanguages = JArray.Parse(response).Select(item => item["language"].ToString().ToUpperInvariant()).ToList();
			}
			catch (Exception ex)
			{
				Logger.Error($"{ex}");
			}

			return supportedLanguages;
		}

		public static Dictionary<string, bool> GetSupportedTargetLanguages(string apiKey)
		{
			var supportedLanguages = new Dictionary<string, bool>();
			try
			{
				var response = GetSupportedLanguages("target", apiKey);
				supportedLanguages =
					JArray.Parse(response).ToDictionary(
						item => item["language"].ToString().ToUpperInvariant(), item => bool.Parse(item["supports_formality"].ToString()));
			}
			catch (Exception ex)
			{
				Logger.Error($"{ex}");
			}

			return supportedLanguages;
		}

		public bool IsLanguagePairSupported(CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			var supportedSourceLanguage = GetLanguage(sourceCulture, SupportedSourceLanguages);
			// do not make a call again to the server if source languages are not supported, because the return condition requires both source and target languages to be supported
			var supportedTargetLanguage = !string.IsNullOrEmpty(supportedSourceLanguage) ? GetLanguage(targetCulture, SupportedTargetLanguages) : string.Empty;

			return !string.IsNullOrEmpty(supportedSourceLanguage) && !string.IsNullOrEmpty(supportedTargetLanguage);
		}

		public string Translate(LanguagePair languageDirection, string sourceText, Formality formality, string glossaryId)
		{
			var targetLanguage = GetLanguage(languageDirection.TargetCulture, SupportedTargetLanguages);
			var sourceLanguage = GetLanguage(languageDirection.SourceCulture, SupportedSourceLanguages);
			var translatedText = string.Empty;
			var normalizeHelper = new NormalizeSourceTextHelper();

			try
			{
				sourceText = normalizeHelper.NormalizeText(sourceText);

				var content = new StringContent($"text={sourceText}" +
												$"&source_lang={sourceLanguage}" +
												$"&target_lang={targetLanguage}" +
												$"&formality={formality.ToString().ToLower()}" +
												"&preserve_formatting=1" +
												"&tag_handling=xml" +
												$"&auth_key={ApiKey}" +
												$"&glossary_id={glossaryId}",
					Encoding.UTF8, "application/x-www-form-urlencoded");

				var response = AppInitializer.Client.PostAsync("https://api.deepl.com/v1/translate", content).Result;
				response.EnsureSuccessStatusCode();

				var translationResponse = response.Content?.ReadAsStringAsync().Result;
				var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(translationResponse);

				if (translatedObject != null && translatedObject.Translations.Any())
				{
					translatedText = translatedObject.Translations[0].Text;
					translatedText = DecodeWhenNeeded(translatedText);
				}
			}
			catch (AggregateException aEx)
			{
				foreach (var innerEx in aEx.InnerExceptions)
				{
					Logger.Error(innerEx);
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				throw;
			}

			return translatedText;
		}

		private static string GetSupportedLanguages(string type, string apiKey)
		{
			var content = new StringContent($"type={type}" + $"&auth_key={apiKey}", Encoding.UTF8,
				"application/x-www-form-urlencoded");

			var response = AppInitializer.Client.PostAsync("https://api.deepl.com/v1/languages", content).Result;
			response.EnsureSuccessStatusCode();

			return response.Content?.ReadAsStringAsync().Result;
		}

		private static bool IsLanguageCompatible(CultureInfo targetLanguage)
		{
			if (!SupportedTargetLanguagesAndFormalities.TryGetValue(targetLanguage.ToString().ToUpper(), out var supportsFormality))
			{
				SupportedTargetLanguagesAndFormalities.TryGetValue(targetLanguage.TwoLetterISOLanguageName.ToUpper(), out supportsFormality);
			}

			return supportsFormality;
		}

		private static HttpResponseMessage IsValidApiKey(string apiKey)
		{
			return AppInitializer.Client.GetAsync($"https://api.deepl.com/v1/usage?auth_key={apiKey}").Result;
		}

		private static void OnApiKeyChanged()
		{
			IsApiKeyValidResponse = IsValidApiKey(ApiKey);

			if (!IsApiKeyValidResponse.IsSuccessStatusCode)
				return;

			if (SupportedTargetLanguagesAndFormalities is { Count: not 0 })
				return;

			SupportedTargetLanguagesAndFormalities = GetSupportedTargetLanguages(ApiKey);
			SupportedTargetLanguages = SupportedTargetLanguagesAndFormalities.Keys.ToList();

			ApiKeyChanged?.Invoke(null, null);
		}

		private string DecodeWhenNeeded(string translatedText)
		{
			if (translatedText.Contains("%"))
			{
				translatedText = Uri.UnescapeDataString(translatedText);
			}

			var greater = new Regex(@"&gt;");
			var less = new Regex(@"&lt;");

			translatedText = greater.Replace(translatedText, ">");
			translatedText = less.Replace(translatedText, "<");

			//the only HTML encodings that appear to be used by DeepL
			//besides the ones we're sending to escape tags
			var amp = new Regex("&amp;|&amp");
			translatedText = amp.Replace(translatedText, "&");

			return translatedText;
		}

		//private Formality GetFormality(LanguagePair languageDirection)
		//{
		//	if (!SupportedTargetLanguagesAndFormalities.TryGetValue(
		//					languageDirection.TargetCulture.RegionNeutralName.ToUpper(), out var supportsFormality))
		//	{
		//		SupportedTargetLanguagesAndFormalities.TryGetValue(languageDirection.TargetCulture.ToString().ToUpper(),
		//			out supportsFormality);
		//	}

		//	return supportsFormality
		//		? Formality
		//		: Formality.Default;
		//}

		// Get the target language based on availability in DeepL; if we have a flavour use that, otherwise use general culture of that flavour (two letter iso) if available, otherwise return null
		// (e.g. for Portuguese, the leftLanguageTag (pt-PT or pt-BR) should be used, so the translations will correspond to the specific language flavor)
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