using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.DeepLMTProvider.WPF;
using Sdl.Community.DeelLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider
{
	public class DeepLTranslationProviderConnecter
	{
		private readonly Logger _logger = Log.GetLogger(nameof(DeepLTranslationProviderConnecter));
		private Formality _formality;
		private Dictionary<string, bool> SupportedTargetLanguagesAndFormalities { get; }
		private List<string> _supportedSourceLanguages;

		public DeepLTranslationProviderConnecter(string key, Formality formality)
		{
			ApiKey = key;
			_formality = formality;

			SupportedTargetLanguagesAndFormalities = Helpers.GetSupportedTargetLanguages(ApiKey);
			SupportedTargetLanguages = SupportedTargetLanguagesAndFormalities.Keys.ToList();
		}

		private string ApiKey { get; }
		private List<string> SupportedTargetLanguages { get; }

		private List<string> SupportedSourceLanguages => _supportedSourceLanguages ??= Helpers.GetSupportedSourceLanguages(ApiKey);

		public bool IsLanguagePairSupported(CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			var supportedSourceLanguage = GetLanguage(sourceCulture, SupportedSourceLanguages);
			// do not make a call again to the server if source languages are not supported, because the return condition requires both source and target languages to be supported
			var supportedTargetLanguage = !string.IsNullOrEmpty(supportedSourceLanguage) ? GetLanguage(targetCulture, SupportedTargetLanguages) : string.Empty;

			return !string.IsNullOrEmpty(supportedSourceLanguage) && !string.IsNullOrEmpty(supportedTargetLanguage);
		}

		public string Translate(LanguagePair languageDirection, string sourceText)
		{
			_formality = SupportedTargetLanguagesAndFormalities[languageDirection.TargetCulture.TwoLetterISOLanguageName.ToUpper()]
				? _formality
				: Formality.Default;

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
												$"&formality={_formality.ToString().ToLower()}" +
												"&preserve_formatting=1" +
												"&tag_handling=xml" +
												$"&auth_key={ApiKey}",
					Encoding.UTF8, "application/x-www-form-urlencoded");

				var response = Helpers.Client.PostAsync("https://api.deepl.com/v1/translate", content).Result;
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
					_logger.Error(innerEx);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
				throw;
			}

			return translatedText;
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