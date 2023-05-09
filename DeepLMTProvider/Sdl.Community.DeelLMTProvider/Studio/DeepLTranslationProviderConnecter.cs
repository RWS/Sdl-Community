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

namespace Sdl.Community.DeepLMTProvider.Studio
{
	public class DeepLTranslationProviderConnecter
	{
		private static readonly Logger _logger = Log.GetLogger(nameof(DeepLTranslationProviderConnecter));
		private static string _apiKey;
		private List<string> _supportedSourceLanguages;

		/// <summary>
		/// Maximum count of translation retries
		/// </summary>
		private const int TranslateMaxRetryCount = 3;
		/// <summary>
		/// Too Many Requests status code as it does not exists in <see cref="System.Net.HttpStatusCode"/>
		/// </summary>
		private const int HttpStatusCodeTooManyRequests = 429;
		/// <summary>
		/// The total count of translation retry attempts in a row. 
		/// </summary>
		private ulong _totalTranslationRetryAttemptInARowCount = 0;
		/// <summary>
		/// The maximum count of translation retry attempts in a row after which we give up executing retry routine.
		/// </summary>
		private const ulong MaxTranslationRetryAttemptInARowCount = TranslateMaxRetryCount * 5L;

		public DeepLTranslationProviderConnecter(string key, Formality formality = Formality.Default)
		{
			ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			ApiKey = key;
			Formality = formality != Formality.Default ? formality : Formality;
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
		private Formality Formality { get; set; }
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
				_logger.Error($"{ex}");
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
				_logger.Error($"{ex}");
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

		public string Translate(LanguagePair languageDirection, string sourceText)
		{
			return TranslateWithRetry(languageDirection, AsEnumerable(sourceText)).First();
		}

		private static IEnumerable<TObject> AsEnumerable<TObject>(TObject value)
		{
			yield return value;
		}

		/// <summary>
		/// Translate <paramref name="sourceTexts"/> in batch
		/// </summary>
		/// <param name="languageDirection"></param>
		/// <param name="sourceTexts"></param>
		/// <returns>Translated source texts in the same order</returns>
		public string[] Translate(LanguagePair languageDirection, IEnumerable<string> sourceTexts)
		{
			return TranslateWithRetry(languageDirection, sourceTexts);
		}

		/// <summary>
		/// Translate <paramref name="sourceTexts"/> in batch with retrying routine in case of errors 
		/// that might occur temporary, eg. request timeout, bad gateway
		/// </summary>
		/// <param name="languageDirection"></param>
		/// <param name="sourceTexts"></param>
		/// <param name="retryAttemptCount"></param>
		/// <returns></returns>
		private string[] TranslateWithRetry(LanguagePair languageDirection, IEnumerable<string> sourceTexts, int retryAttemptCount = 0)
		{
			string[] translatedTexts = null;
			try
			{
				translatedTexts = BatchTranslate(languageDirection, sourceTexts);
			}
			catch (HttpRequestException ex)
			{
				if (!ex.GetHttpStatusCode().HasValue || retryAttemptCount < TranslateMaxRetryCount || DoesDeepLServiceLooksLikeIsUnavailable())
					throw;

				switch (ex.GetHttpStatusCode().Value)
				{
					case System.Net.HttpStatusCode.RequestTimeout:
					case System.Net.HttpStatusCode.GatewayTimeout:
					case System.Net.HttpStatusCode.BadGateway:
					case System.Net.HttpStatusCode.ServiceUnavailable:
					case (System.Net.HttpStatusCode)HttpStatusCodeTooManyRequests:
						++retryAttemptCount;
						++_totalTranslationRetryAttemptInARowCount;
						TimeSpan retryDelay = CalculateRetryDelayTimeSpan(retryAttemptCount);
						_logger.Info($"Retrying translation ({retryAttemptCount}) in {retryDelay} because of: {ex.Message} ({ex.GetHttpStatusCode().Value}).");
						translatedTexts = TranslateWithRetry(languageDirection, sourceTexts, retryAttemptCount);
						break;
					default:
						throw;
				}
			}

			_totalTranslationRetryAttemptInARowCount = 0;
			return translatedTexts;
		}

		private TimeSpan CalculateRetryDelayTimeSpan(int retryAttemptCount)
		{
			return TimeSpan.FromSeconds(retryAttemptCount * 5);
		}

		/// <summary>
		/// Does DeepL service looks like is unavailable for a while? We would like to prevent continuous and time consuming 
		/// retrying routine in case of some serious problem on DeepL service side. If the service is unavailable for lets say few hours,
		/// we would end up executing pre-translation task for very long.
		/// </summary>
		/// <returns></returns>
		private bool DoesDeepLServiceLooksLikeIsUnavailable()
		{
			return _totalTranslationRetryAttemptInARowCount > MaxTranslationRetryAttemptInARowCount;
		}

		private string[] BatchTranslate(LanguagePair languageDirection, IEnumerable<string> sourceTexts)
		{
			var formality = GetFormality(languageDirection);

			var targetLanguage = GetLanguage(languageDirection.TargetCulture, SupportedTargetLanguages);
			var sourceLanguage = GetLanguage(languageDirection.SourceCulture, SupportedSourceLanguages);

			var translatedTexts = new string[sourceTexts.Count()];
			var normalizeHelper = new NormalizeSourceTextHelper();
			string stringContent = null;
			HttpResponseMessage response = null;

			try
			{
				stringContent = $"source_lang={sourceLanguage}" +
								$"&target_lang={targetLanguage}" +
								$"&formality={formality.ToString().ToLower()}" +
								"&preserve_formatting=1" +
								"&tag_handling=xml" +
								$"&auth_key={ApiKey}";


				foreach (string sourceText in sourceTexts)
				{
					string normalizedSourceText = normalizeHelper.NormalizeText(sourceText);
					stringContent += $"&text={normalizedSourceText}";
				}

				var content = new StringContent(stringContent, Encoding.UTF8, "application/x-www-form-urlencoded");
				response = AppInitializer.Client.PostAsync("https://api.deepl.com/v1/translate", content).Result;
				response.EnsureSuccessStatusCode();

				var translationResponse = response.Content?.ReadAsStringAsync().Result;
				var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(translationResponse);

				if (translatedObject != null)
				{
					for (int i = 0; i < translatedObject.Translations.Count; i++)
					{
						translatedTexts[i] = translatedObject.Translations[i].Text;
						translatedTexts[i] = DecodeWhenNeeded(translatedTexts[i]);

					}
				}

			}
			catch (AggregateException aEx)
			{
				foreach (var innerEx in aEx.InnerExceptions)
				{
					_logger.Error(innerEx);
				}
				throw;
			}
			catch (HttpRequestException ex)
			{
				if (response != null)
				{
					ex.SetHttpStatusCode(response.StatusCode);
				}

				_logger.Error(ex);
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
				throw;
			}

			return translatedTexts;
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

			if (!IsApiKeyValidResponse.IsSuccessStatusCode) return;

			if (SupportedTargetLanguagesAndFormalities is { Count: not 0 }) return;

			SupportedTargetLanguagesAndFormalities = GetSupportedTargetLanguages(ApiKey);
			SupportedTargetLanguages = SupportedTargetLanguagesAndFormalities.Keys.ToList();
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

		private Formality GetFormality(LanguagePair languageDirection)
		{
			if (!SupportedTargetLanguagesAndFormalities.TryGetValue(
							languageDirection.TargetCulture.TwoLetterISOLanguageName.ToUpper(), out var supportsFormality))
			{
				SupportedTargetLanguagesAndFormalities.TryGetValue(languageDirection.TargetCulture.ToString().ToUpper(),
					out supportsFormality);
			}

			return supportsFormality
				? Formality
				: Formality.Default;
		}

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