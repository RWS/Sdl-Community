using System;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;

namespace Sdl.Community.BeGlobalV4.Provider.Service
{
	public class BeGlobalV4Translator
	{
		private readonly IRestClient _client;
		private readonly string _flavor;
		public static readonly Log Log = Log.Instance;

		public BeGlobalV4Translator(
			string server,
			string user,
			string password,
			string flavor,
			bool useClientAuthentication)
		{
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
			AddTraceId(request);
			request.RequestFormat = DataFormat.Json;
			var response = _client.Execute(request);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				throw new Exception("Acquiring token failed: " + response.Content);
			}
			dynamic json = JsonConvert.DeserializeObject(response.Content);
			_client.AddDefaultHeader("Authorization", $"Bearer {json.accessToken}");
		}

		public int GetClientInformation()
		{
			try
			{
				var request = new RestRequest("/accounts/api-credentials/self")
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
				if (user != null)
				{
					return user.AccountId;
				}
				return 0;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"Get client information method: {e.Message}\n {e.StackTrace}");
			}
			return 0;
		}

		public string TranslateText(string text, string sourceLanguage, string targetLanguage)
		{
			try
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
			catch(Exception e)
			{
				Log.Logger.Error($"Translate text method: {e.Message}\n {e.StackTrace}");
			}
			return string.Empty;
		}

		public int GetUserInformation()
		{
			try
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
			catch (Exception e)
			{
				Log.Logger.Error($"Get user information method: {e.Message}\n {e.StackTrace}");
			}
			return 0;
		}

		public SubscriptionInfo GetLanguagePairs(string accountId)
		{
			try
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
			catch(Exception e)
			{
				Log.Logger.Error($"Subscription info method: {e.Message}\n {e.StackTrace}");
			}
			return new SubscriptionInfo();
		}

		private byte[] WaitForTranslation(string id)
		{
			try
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
			catch (Exception e)
			{
				Log.Logger.Error($"Wait for translation method: {e.Message}\n {e.StackTrace}");
			}
			return new byte[1];
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
			var pluginVersion = VersionHelper.GetPluginVersion();
			var studioVersion = VersionHelper.GetStudioVersion();
			request.AddHeader("Trace-ID", $"BeGlobal {pluginVersion} - {studioVersion}.{Guid.NewGuid().ToString()}");
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