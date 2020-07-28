using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Newtonsoft.Json;
using Sdl.Community.DeelLMTProvider.Model;
using Sdl.LanguagePlatform.Core;
using System.Xml;
using Sdl.Community.DeepLMTProvider.WPF.Model;

namespace Sdl.Community.DeepLMTProvider
{
	public class DeepLTranslationProviderConnecter
	{
		public string ApiKey { get; set; }
		private readonly string _pluginVersion = "";
		private readonly string _identifier;
		private readonly Formality _formality;
		public static readonly Log Log = Log.Instance;

		public DeepLTranslationProviderConnecter(string key, string identifier, Formality formality)
		{
			ApiKey = key;
			_identifier = identifier;
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
				Log.Logger.Error($"{e.Message}\n {e.StackTrace}");
			}
		}

		public string Translate(LanguagePair languageDirection, string sourceText)
		{
			var targetLanguage = GetTargetLanguage(languageDirection);
			var sourceLanguage = languageDirection.SourceCulture.TwoLetterISOLanguageName;
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
													$"&tag_handling=xml&auth_key={ApiKey}", Encoding.UTF8, "application/x-www-form-urlencoded");

					httpClient.DefaultRequestHeaders.Add("Trace-ID", $"SDL Trados Studio 2019 /plugin {_pluginVersion}");

					var response = httpClient.PostAsync("https://api.deepl.com/v1/translate", content).Result;
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
                        Log.Logger.Error($"HTTP Request to DeepL Translate REST API endpoint failed with status code '{response.StatusCode}'. " +
                            $"Response content: {response.Content?.ReadAsStringAsync().Result}.");
                        MessageBox.Show(response.ReasonPhrase, string.Empty, MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{e.Message}\n {e.StackTrace}");
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

		// Get the target language based on language direction
		// (for Portuguese, the leftLanguageTag (pt-PT or pt-BR) should be used, so the translations will correspond to the specific language flavor)
		private string GetTargetLanguage(LanguagePair languageDirection)
		{
			return languageDirection.TargetCulture.DisplayName.Contains("Portuguese")
				? languageDirection.TargetCulture.Name
				: languageDirection.TargetCulture.TwoLetterISOLanguageName;
		}
	}
}