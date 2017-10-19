using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.DeelLMTProvider.Model;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeelLMTProvider
{
	public class DeepLTranslationProviderConnecter
	{
		public string ApiKey { get; set; }

		public DeepLTranslationProviderConnecter(string key)
		{
			ApiKey = key;
		}

		public string Translate(LanguagePair languageDirection, string sourcetext)
		{
			var targetLanguage = languageDirection.TargetCulture.TwoLetterISOLanguageName;
			var url = string.Format("https://api.deepl.com/v1/translate?text={0}&target_lang={1}&auth_key={2}"
				, sourcetext, targetLanguage, ApiKey);
			var translatedText = string.Empty;

			using (var webClient = new WebClient())
			{
				webClient.Encoding = Encoding.UTF8;
				try
				{
					var response = webClient.DownloadString(url);
					var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(response);
					if (translatedObject != null)
					{
						translatedText = translatedObject.Translations[0].Text;
					}
				}
				catch (WebException e) //will come back 400 bad request if there is a problem
				{
					var eReason = Helpers.ProcessWebException(e);
					//get our localized error message from the resources file
					//string message = PluginResources.ApiConnectorGoogleAuthErrorMsg1 + Environment.NewLine + PluginResources.ApiConnectorGoogleAuthErrorMsg2;
					throw new Exception(eReason);
				}
			}

			return translatedText;
		}
	}
}
