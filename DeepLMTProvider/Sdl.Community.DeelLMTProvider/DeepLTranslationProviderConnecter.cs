using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Sdl.Community.DeelLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider
{
	public class DeepLTranslationProviderConnecter
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly string _pluginVersion = "";
		private Formality _formality;
		private List<string> _supportedTargetLanguages;
		private List<string> _supportedSourceLanguages;

		public bool IsLanguagePairSupported(CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			return GetLanguage(sourceCulture, SupportedSourceLanguages) != null &&
			       GetLanguage(targetCulture, SupportedTargetLanguages) != null;
		}

		public DeepLTranslationProviderConnecter(string key, Formality formality)
		{
			ApiKey = key;
			_formality = formality;

			try
			{
				// fetch the version of the plugin from the manifest deployed
				var pexecutingAsseblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				pexecutingAsseblyPath = Path.Combine(pexecutingAsseblyPath, "pluginpackage.manifest.xml");
				var doc = new XmlDocument();
				doc.Load(pexecutingAsseblyPath);

				if (doc.DocumentElement == null) return;
				foreach (XmlNode n in doc.DocumentElement.ChildNodes)
				{
					if (n.Name == "Version")
					{
						_pluginVersion = n.InnerText;
					}
				}
			}
			catch (Exception e)
			{
				// broad catch here, if anything goes wrong with determining the version we don't want the user to be disturbed in any way
				_logger.Error($"{e.Message}\n {e.StackTrace}");
			}
		}

		public string ApiKey { get; set; }

		private List<string> SupportedTargetLanguages
			=> _supportedTargetLanguages ?? (_supportedTargetLanguages = GetSupportedLanguages("target"));

		private List<string> SupportedSourceLanguages
			=> _supportedSourceLanguages ?? (_supportedSourceLanguages = GetSupportedLanguages("source"));

		public string Translate(LanguagePair languageDirection, string sourceText)
		{
			_formality = Helpers.IsLanguageCompatible(languageDirection.TargetCulture) ? _formality : Formality.Default;

			var targetLanguage = GetLanguage(languageDirection.TargetCulture, SupportedTargetLanguages);
			var sourceLanguage = GetLanguage(languageDirection.SourceCulture, SupportedSourceLanguages);
			var translatedText = string.Empty;
			var normalizeHelper = new NormalizeSourceTextHelper();

			try
			{
				sourceText = normalizeHelper.NormalizeText(sourceText);

				using (var httpClient = new HttpClient())
				{
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

					httpClient.Timeout = TimeSpan.FromMinutes(5);
					var content = new StringContent($"text={sourceText}" +
													$"&source_lang={sourceLanguage}" +
													$"&target_lang={targetLanguage}" +
													$"&formality={_formality.ToString().ToLower()}" +
													"&preserve_formatting=1" +
													"&tag_handling=xml" +
													$"&auth_key={ApiKey}",
						Encoding.UTF8, "application/x-www-form-urlencoded");

					httpClient.DefaultRequestHeaders.Add("Trace-ID", $"SDL Trados Studio 2019 /plugin {_pluginVersion}");

					var response = httpClient.PostAsync("https://api.deepl.com/v2/translate", content).Result;
					if (response.IsSuccessStatusCode)
					{
						var translationResponse = response.Content?.ReadAsStringAsync().Result;
						var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(translationResponse);

						if (translatedObject != null && translatedObject.Translations.Any())
						{
							translatedText = translatedObject.Translations[0].Text;
							translatedText = DecodeWhenNeeded(translatedText);
						}
					}
					else
					{
						var message =
							$"HTTP Request to DeepL Translate REST API endpoint failed with status code '{response.StatusCode}'. " +
							$"Response content: {response.Content?.ReadAsStringAsync().Result}.";
						_logger.Error(message);
						throw new HttpRequestException(message);
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error($"{e.Message}\n {e.StackTrace}");
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

		private List<string> GetSupportedLanguages(string type)
		{
			using (var httpClient = new HttpClient())
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 |
													   SecurityProtocolType.Tls;

				httpClient.Timeout = TimeSpan.FromMinutes(5);
				var content = new StringContent($"type={type}" +
												$"&auth_key={ApiKey}",
					Encoding.UTF8, "application/x-www-form-urlencoded");

				httpClient.DefaultRequestHeaders.Add("Trace-ID", $"SDL Trados Studio 2021 /plugin {_pluginVersion}");

				var response = httpClient.PostAsync("https://api.deepl.com/v2/languages", content).Result;
				if (response.IsSuccessStatusCode)
				{
					var languagesResponse = response.Content?.ReadAsStringAsync().Result;

					return JArray.Parse(languagesResponse).Select(item => item["language"].ToString().ToUpperInvariant()).ToList();
				}
			}

			return null;
		}

		// Get the target language based on availability in DeepL; if we have a flavour use that, otherwise use general culture of that flavour (two letter iso) if available, otherwise return null
		// (e.g. for Portuguese, the leftLanguageTag (pt-PT or pt-BR) should be used, so the translations will correspond to the specific language flavor)
		private string GetLanguage(CultureInfo culture, List<string> languageList)
		{
			var leftLangTag = culture.IetfLanguageTag.ToUpperInvariant();
			var twoLetterIso = culture.TwoLetterISOLanguageName.ToUpperInvariant();

			var selectedTargetLanguage = languageList.FirstOrDefault(tl => tl == leftLangTag) ??
			                             languageList.FirstOrDefault(tl => tl == twoLetterIso);

			return selectedTargetLanguage ?? (languageList.Any(tl => tl.Contains(twoLetterIso))
				? twoLetterIso
				: null);
		}
	}
}