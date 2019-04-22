using System;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;

namespace Sdl.Community.BeGlobalV4.Provider.Service
{
	public class BeGlobalV4Translator
	{
		private readonly IRestClient _client;
		private readonly string _flavor;
		private readonly string _url = "https://translate-api.sdlbeglobal.com";

		public BeGlobalV4Translator(string flavor)
		{
			_flavor = flavor;
			var studioCredentials = new StudioCredentials();
			var accessToken = studioCredentials.GetToken();

			_client = new RestClient($"{_url}/v4");

			if (!string.IsNullOrEmpty(accessToken))
			{
				_client.AddDefaultHeader("Authorization", $"Bearer {accessToken}");
			}
		}

		public string TranslateText(string text,string sourceLanguage, string targetLanguage)
		{
			var request = new RestRequest("/mt/translations/async", Method.POST)
			{
				RequestFormat = DataFormat.Json
			};
			AddTraceId(request);

			string[] texts = { text };
			request.AddBody(new
			{
				input = texts,
				sourceLanguageId = sourceLanguage,
				targetLanguageId = targetLanguage,
				model = _flavor,
				inputFormat = "xliff"
			});
			var response = _client.Execute(request);
			if (!response.IsSuccessful)
			{
				ShowErrors(response);
			}
			dynamic json = JsonConvert.DeserializeObject(response.Content);

			var rawData = WaitForTranslation(json?.requestId?.Value);

			json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(rawData));
			return json != null ? json.translation[0] : string.Empty;
		}

		public int GetUserInformation()
		{
			var request = new RestRequest("/accounts/users/self")
			{
				RequestFormat = DataFormat.Json
			};
			AddTraceId(request);

			var response = _client.Execute(request);
			var user = JsonConvert.DeserializeObject<UserDetails>(response.Content);
			if (!response.IsSuccessful)
			{
				ShowErrors(response);
			}
			return user != null ? user.AccountId : 0;
		}

		public SubscriptionInfo GetLanguagePairs(string accountId)
		{
			var request = new RestRequest($"/accounts/{accountId}/subscriptions/language-pairs")
			{
				RequestFormat = DataFormat.Json
			};
			AddTraceId(request);

			var response = _client.Execute(request);
			if (!response.IsSuccessful)
			{
				ShowErrors(response);
			}
			var subscriptionInfo = JsonConvert.DeserializeObject<SubscriptionInfo>(response.Content);
			return subscriptionInfo;
		}

		private byte[] WaitForTranslation(string id)
		{
			IRestResponse response;
			string status;
			do
			{
				response = RestGet($"/mt/translations/async/{id}");
				if (!response.IsSuccessful)
				{
					ShowErrors(response);
				}

				dynamic json = JsonConvert.DeserializeObject(response.Content);
				status = json.translationStatus;

				if (!status.Equals("DONE", StringComparison.CurrentCultureIgnoreCase))
				{
					System.Threading.Thread.Sleep(300);
				}
				if (status.Equals("FAILED"))
				{
					ShowErrors(response);

				}
			} while (status.Equals("INIT", StringComparison.CurrentCultureIgnoreCase) ||
			         status.Equals("TRANSLATING", StringComparison.CurrentCultureIgnoreCase));

			response = RestGet($"/mt/translations/async/{id}/content");
			if (!response.IsSuccessful)
			{
				ShowErrors(response);
			}
			return response.RawBytes;
		}

		private IRestResponse RestGet(string command)
		{
			var request = new RestRequest(command)
			{
				RequestFormat = DataFormat.Json
			};
			AddTraceId(request);

			var response = _client.Execute(request);
			return response;
		}
		private void AddTraceId(IRestRequest request)
		{
			request.AddHeader("Trace-ID", $"Studio2019_{Guid.NewGuid().ToString()}");
		}

		private void ShowErrors(IRestResponse response)
		{
			var responseContent = JsonConvert.DeserializeObject<ResponseError>(response.Content);
			if (responseContent?.Errors != null)
			{
				foreach (var error in responseContent.Errors)
				{
					throw new Exception($"Error code: {error.Code}, {error.Description}");
				}
			}
		}
	}
}