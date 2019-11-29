using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Interfaces;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;

namespace Sdl.Community.BeGlobalV4.Provider.Service
{
	public class BeGlobalV4Translator
	{
		private readonly IRestClient _client;
		private readonly string _flavor;
		private readonly IMessageBoxService _messageBoxService;

		public static readonly Log Log = Log.Instance;

		public BeGlobalV4Translator(string server, BeGlobalTranslationOptions options)
		{
			try
			{
				_messageBoxService = new MessageBoxService();
				_flavor = options.Model;
				_client = new RestClient(string.Format($"{server}/v4"));
				IRestRequest request;
				if (options.UseClientAuthentication)
				{
					request = new RestRequest("/token", Method.POST)
					{
						RequestFormat = DataFormat.Json
					};
					request.AddBody(new { clientId = options.ClientId, clientSecret = options.ClientSecret });
				}
				else
				{
					request = new RestRequest("/token/user", Method.POST)
					{
						RequestFormat = DataFormat.Json
					};
					request.AddBody(new { username = options.ClientId, password = options.ClientSecret });
				}
				AddTraceId(request);
				request.RequestFormat = DataFormat.Json;
				var response = _client.Execute(request);
				if (response.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception(Constants.TokenFailed + response.Content);
				}
				dynamic json = JsonConvert.DeserializeObject(response.Content);
				_client.AddDefaultHeader("Authorization", $"Bearer {json.accessToken}");
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.BeGlobalV4Translator} {ex.Message}\n {ex.StackTrace}");
			}
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
				Log.Logger.Error($"{Constants.GetClientInformation} {e.Message}\n {e.StackTrace}");
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
					return string.Empty;
				}
				dynamic json = JsonConvert.DeserializeObject(response.Content);

				var rawData = WaitForTranslation(json?.requestId?.Value);

				json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(rawData));
				return json != null ? json.translation[0] : string.Empty;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.TranslateTextMethod} {e.Message}\n {e.StackTrace}");
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
				Log.Logger.Error($"{ Constants.GetUserInformation} { e.Message}\n {e.StackTrace}");
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
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.SubscriptionInfoMethod} {e.Message}\n {e.StackTrace}");
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
						return new byte[1];
					}

					dynamic json = JsonConvert.DeserializeObject(response.Content);
					if (json == null)
					{
						return new byte[1];
					}
					status = json.translationStatus;

					if (!status.Equals(Constants.DONE, StringComparison.CurrentCultureIgnoreCase))
					{
						System.Threading.Thread.Sleep(300);
					}
					if (status.Equals(Constants.FAILED))
					{
						ShowErrors(response);
					}
				} while (status.Equals(Constants.INIT, StringComparison.CurrentCultureIgnoreCase) ||
						 status.Equals(Constants.TRANSLATING, StringComparison.CurrentCultureIgnoreCase));

				response = RestGet($"/mt/translations/async/{id}/content");
				if (!response.IsSuccessful)
				{
					ShowErrors(response);
				}
				return response.RawBytes;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.WaitTranslationMethod} {e.Message}\n {e.StackTrace}");
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
			request.AddHeader(Constants.TraceId, $"{Constants.SDLMachineTranslationCloudProvider} {pluginVersion} - {studioVersion}.{Guid.NewGuid().ToString()}");
		}

		private void ShowErrors(IRestResponse response)
		{
			if(response.StatusCode == 0)
			{
				_messageBoxService.ShowWarningMessage(Constants.CheckInternetConnection, Constants.SDLMTCloud);
			}
			var responseContent = JsonConvert.DeserializeObject<ResponseError>(response?.Content);
			if (responseContent?.Errors != null)
			{
				foreach (var error in responseContent.Errors)
				{
					throw new Exception($"{Constants.ErrorCode} {error.Code}, {error.Description}");
				}
			}
		}
	}
}