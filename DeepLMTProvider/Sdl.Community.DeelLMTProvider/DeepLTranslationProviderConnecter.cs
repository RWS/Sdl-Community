using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using Sdl.Community.DeelLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Model;
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
			var translatedText = string.Empty;
			try
			{
				var client = new RestClient(@"https://api.deepl.com/v1");
				var request = new RestRequest("translate", Method.POST);
				request.AddParameter("text", sourcetext);
				request.AddParameter("target_lang", targetLanguage);
				request.AddParameter("tag_handling", tagOption);
				request.AddParameter("split_sentences", 0);
				request.AddParameter("auth_key", ApiKey);


				var response = client.Execute(request).Content;
				var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(response);
				if (translatedObject != null)
				{
					translatedText = translatedObject.Translations[0].Text;
				}
			}
			catch (WebException e) 
			{
				var eReason = Helpers.ProcessWebException(e);
				_telemetryTracker.TrackException(e);
				_telemetryTracker.TrackTrace(e.StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
				throw new Exception(eReason);
			}

			return translatedText;
		}
	}
}
