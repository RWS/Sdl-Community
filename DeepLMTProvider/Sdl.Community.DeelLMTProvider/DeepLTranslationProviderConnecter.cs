using System;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.DeelLMTProvider.Model;
using Sdl.LanguagePlatform.Core;
using System.Xml;

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
				var client = new RestClient(@"https://api.deepl.com/v1")
				{
					UserAgent = "SDL Trados 2019 (v" + _pluginVersion + ",id" + _identifier + ")"
				};
				var request = new RestRequest("translate", Method.POST);

				sourceText = normalizeHelper.NormalizeText(sourceText);

				request.AddParameter("text", sourceText);
				request.AddParameter("source_lang", sourceLanguage);
				request.AddParameter("target_lang", targetLanguage);
				//adding this resolve line breaks issue and missing ##login##
				request.AddParameter("preserve_formatting", 1);
				//tag handling cause issues on uppercase words
				request.AddParameter("tag_handling", tagOption);
				//if we add this the formattiong is not right
				//request.AddParameter("split_sentences", 0);
				request.AddParameter("auth_key", ApiKey);

				var response = client.Execute(request).Content;
				var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(response);
				if (translatedObject != null)
				{
					translatedText = translatedObject.Translations[0].Text;
					translatedText = HttpUtility.UrlDecode(translatedText);
					translatedText = HttpUtility.HtmlDecode(translatedText);
				}
			}
			catch (WebException e)
			{
				var eReason = Helpers.ProcessWebException(e);
				throw new Exception(eReason);
			}

			return translatedText;
		}
	}
}