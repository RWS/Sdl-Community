using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.DeelLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Telemetry;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeelLMTProvider
{
	public class DeepLTranslationProviderConnecter
	{
		private ITelemetryTracker _telemetryTracker;
		public string ApiKey { get; set; }

		public DeepLTranslationProviderConnecter(string key)
		{
			ApiKey = key;
			_telemetryTracker = new TelemetryTracker();
		}

		public string Translate(LanguagePair languageDirection, string sourcetext)
		{
			const string tagOption = @"xml";
			var targetLanguage = languageDirection.TargetCulture.TwoLetterISOLanguageName;
			var url = string.Format("https://api.deepl.com/v1/translate?text={0}&target_lang={1}&tag_handling={2}&auth_key={3}&split_sentences=0"
				, sourcetext, targetLanguage, tagOption, ApiKey);
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
					_telemetryTracker.TrackException(e);
					_telemetryTracker.TrackTrace(e.StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
					throw new Exception(eReason);
				}
			}

			return translatedText;
		}
	}
}
