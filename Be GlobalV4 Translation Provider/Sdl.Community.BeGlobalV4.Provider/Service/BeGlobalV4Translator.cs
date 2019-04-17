using System;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.BeGlobalV4.Provider.Model;

namespace Sdl.Community.BeGlobalV4.Provider.Service
{
	public class BeGlobalV4Translator
	{
		private readonly IRestClient _client;
		private readonly string _source;
		private readonly string _target;
		private readonly string _flavor;

		public BeGlobalV4Translator(
			string server,
			string user,
			string password,
			string source,
			string target,
			string flavor,
			bool useClientAuthentication)
		{
			_source = source;
			_target = target;
			_flavor = flavor;

			_client = new RestClient(string.Format($"{server}/v4"));
			IRestRequest request;
			if (useClientAuthentication)
			{
				request = new RestRequest("/token", Method.POST)
				{
					RequestFormat = DataFormat.Json
				};
				request.AddBody(new { clientId = user, clientSecret = password });
			}
			else
			{
				request = new RestRequest("/token/user", Method.POST)
				{
					RequestFormat = DataFormat.Json
				};
				request.AddBody(new { username = user, password = password });
			}
			request.AddHeader("Trace-ID", Guid.NewGuid().ToString());
			request.RequestFormat = DataFormat.Json;
			var response = _client.Execute(request);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				throw new Exception("Acquiring token failed: " + response.Content);
			}
			dynamic json = JsonConvert.DeserializeObject(response.Content);
			_client.AddDefaultHeader("Authorization", $"Bearer {json.accessToken}");
		}

		public string TranslateText(string text)
		{
			var json = UploadText(text);

			var rawData = WaitForTranslation(json.requestId.Value);

			json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(rawData));
			return json != null ? json.translation[0] : string.Empty;
		}

		public int GetClientInformation()
		{
			var request = new RestRequest("/accounts/api-credentials/self")
			{
				RequestFormat = DataFormat.Json
			};
			var response = _client.Execute(request);
			var user = JsonConvert.DeserializeObject<UserDetails>(response.Content);
			if (!response.IsSuccessful)
			{
				ShowErrors(response);
			}
			if (user != null)
			{
				return user.AccountId;
			}
			return 0;
		}

		public int GetUserInformation()
		{
			var request = new RestRequest("/accounts/users/self")
			{
				RequestFormat = DataFormat.Json
			};
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
			var response = _client.Execute(request);
			if (!response.IsSuccessful)
			{
				ShowErrors(response);
			}
			var subscriptionInfo = JsonConvert.DeserializeObject<SubscriptionInfo>(response.Content);
			return subscriptionInfo;
		}


		public dynamic UploadText(string text)
		{
			var request = new RestRequest("/mt/translations/async", Method.POST)
			{
				RequestFormat = DataFormat.Json
			};
			string[] texts = { text };
			request.AddBody(new
			{
				input = texts,
				sourceLanguageId = _source,
				targetLanguageId = _target,
				model = _flavor,
				inputFormat = "xliff"
			});
			var response = _client.Execute(request);
			if (!response.IsSuccessful)
			{
				ShowErrors(response);
			}
			dynamic json = JsonConvert.DeserializeObject(response.Content);
			return json;
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
			}
			while (!status.Equals("DONE", StringComparison.CurrentCultureIgnoreCase)); // check for FAILED to catch errors

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
			var response = _client.Execute(request);
			return response;
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