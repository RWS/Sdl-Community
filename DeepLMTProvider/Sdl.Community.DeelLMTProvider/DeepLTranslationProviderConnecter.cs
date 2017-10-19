using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
			var targetLanguage = "DE";
			var url = string.Format("https://api.deepl.com/v1/translate?text={0}&target_lang={1}&auth_key={2}"
				, sourcetext, targetLanguage, ApiKey);

			using (var webClient = new WebClient())
			{
				webClient.Encoding = Encoding.UTF8;
				var result = webClient.DownloadString(url);
				//try
				//{
				//	result = webClient.DownloadString(url);  //gets us the json data indicating supported source languages for this target
				//}
				//catch (WebException e) //will come back 400 bad request if there is a problem
				//{
				//	string eReason = GetExceptionReason(e);
				//	//get our localized error message from the resources file
				//	string message = PluginResources.ApiConnectorGoogleAuthErrorMsg1 + Environment.NewLine + PluginResources.ApiConnectorGoogleAuthErrorMsg2;
				//	throw new Exception(eReason);
				//}
			}

			return "";
		}
	}
}
