using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.DeelLMTProvider.Model;
using Sdl.LanguagePlatform.Core;
using System.Xml;
using RestSharp.Extensions;

namespace Sdl.Community.DeepLMTProvider
{
	public class DeepLTranslationProviderConnecter
	{

		public string ApiKey { get; set; }
		private readonly string _pluginVersion = "";
		private readonly string _identifier;

		public DeepLTranslationProviderConnecter(string key, string identifier)
		{
			ApiKey = key;
			_identifier = identifier;

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
			}
		}

		public string Translate(LanguagePair languageDirection, string sourceText)
		{
			const string tagOption = @"xml";
			var targetLanguage = languageDirection.TargetCulture.TwoLetterISOLanguageName;
			var sourceLanguage = languageDirection.SourceCulture.TwoLetterISOLanguageName;
			var translatedText = string.Empty;
			var normalizeHelper = new NormalizeSourceTextHelper();

			try
			{
				sourceText = normalizeHelper.NormalizeText(sourceText);

				using (var httpClient = new HttpClient())
				{
					var values = new Dictionary<string, string>
					{
						{"text", sourceText},
						{"source_lang",sourceLanguage },
						{"target_lang",targetLanguage},
						{"preserve_formatting","1" },
						{"tag_handling","xml" },
						{"auth_key",ApiKey },
					};
					httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
					var content = new FormUrlEncodedContent(values);
					var response =  httpClient.PostAsync("https://api.deepl.com/v1/translate", content).Result.Content.ReadAsStringAsync().Result;
					var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(response);

					if (translatedObject != null)
					{
						translatedText = translatedObject.Translations[0].Text;
						translatedText = DecodeWhenNeeded(translatedText);
						translatedText = HttpUtility.HtmlDecode(translatedText);
					}	
				}
			}
			catch (WebException e)
			{
				var eReason = Helpers.ProcessWebException(e);
				throw new Exception(eReason);
			}

			return translatedText;
		}

		private string DecodeWhenNeeded(string translatedText)
		{
			if (translatedText.Contains("%"))
			{
				translatedText = HttpUtility.UrlDecode(translatedText);
				translatedText = RemovePercentSymbols(translatedText);
			}

			return translatedText;
		}

		private string RemovePercentSymbols(string translatedText)
		{
			while (translatedText.Contains("%"))
			{
				var index = translatedText.IndexOf("%", StringComparison.Ordinal);
				var sb = new StringBuilder(translatedText);
				sb.Remove(index, 1);
				translatedText = sb.ToString();
			}

			return translatedText;
		}
	}
}