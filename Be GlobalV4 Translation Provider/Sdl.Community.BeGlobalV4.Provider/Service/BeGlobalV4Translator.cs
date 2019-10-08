using System;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using DataFormat = RestSharp.DataFormat;

namespace Sdl.Community.BeGlobalV4.Provider.Service
{
	public class BeGlobalV4Translator
	{
		private readonly IRestClient _client;
		private readonly string _flavor;
		private const string Url = "https://translate-api.sdlbeglobal.com";
		public static readonly Log Log = Log.Instance;
		private readonly StudioCredentials _studioCredentials;
		private string _authenticationMethod = string.Empty;
		private const string PluginName = "Machine Translation Cloud Provider";

		public BeGlobalV4Translator(BeGlobalTranslationOptions beGlobalTranslationOptions)
		{
			_flavor = beGlobalTranslationOptions.Model;
			_authenticationMethod = beGlobalTranslationOptions.AuthenticationMethod;
			_studioCredentials = new StudioCredentials();
			_client = new RestClient($"{Url}/v4")
			{
				CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
			};

			if (!string.IsNullOrEmpty(_authenticationMethod))
			{
				if (_authenticationMethod.Equals(Enums.GetDisplayName(Enums.LoginOptions.APICredentials)))
				{
					var request = new RestRequest("/token", Method.POST)
					{
						RequestFormat = DataFormat.Json
					};
					request.AddBody(new { clientId = beGlobalTranslationOptions.ClientId, clientSecret = beGlobalTranslationOptions.ClientSecret });
					request.RequestFormat = DataFormat.Json;
					var response = _client.Execute(request);
					if (response.StatusCode != HttpStatusCode.OK)
					{
						throw new Exception("Acquiring token failed: " + response.Content);
					}
					dynamic json = JsonConvert.DeserializeObject(response.Content);
					_client.AddDefaultHeader("Authorization", $"Bearer {json.accessToken}");
				}
				else
				{
					var accessToken = string.Empty;

					Application.Current?.Dispatcher?.Invoke(() =>
					{
						accessToken = _studioCredentials.GetToken();
					});
					accessToken = _studioCredentials.GetToken();

					if (!string.IsNullOrEmpty(accessToken))
					{
						_client.AddDefaultHeader("Authorization", $"Bearer {accessToken}");
					}
				}
			}
		}

		public string TranslateText(string text, string sourceLanguage, string targetLanguage)
		{
			try
			{
				var request = new RestRequest("/mt/translations/async", Method.POST)
				{
					RequestFormat = DataFormat.Json
				};
				var traceId = GetTraceId(request);

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

				if (response.StatusCode == HttpStatusCode.Unauthorized)
				{
					// Get refresh token
					var token = _studioCredentials.EnsureValidConnection();
					if (!string.IsNullOrEmpty(token))
					{
						UpdateRequestHeadersForRefreshToken(request, token);

						var translationAsyncResponse = _client.Execute(request);

						if (translationAsyncResponse.StatusCode == HttpStatusCode.Unauthorized)
						{
							MessageBox.Show("Unauthorized: Please check your credentials", PluginName, MessageBoxButton.OK);

							Log.Logger.Error($"Unauthorized: Translate text using refresh token \nTrace-Id: {traceId}");
						}
						else if (!translationAsyncResponse.IsSuccessful && translationAsyncResponse.StatusCode != HttpStatusCode.Unauthorized)
						{
							ShowErrors(translationAsyncResponse);
						}
						return ReturnTranslation(translationAsyncResponse);
					}
				}
				else if (!response.IsSuccessful && response.StatusCode != HttpStatusCode.Unauthorized)
				{
					ShowErrors(response);
				}
				return ReturnTranslation(response);
			}
			catch (Exception e)
			{
				Log.Logger.Error($"Translate text method: {e.Message}\n {e.StackTrace}");
			}
			return string.Empty;
		}

		private string ReturnTranslation(IRestResponse response)
		{
			dynamic json = JsonConvert.DeserializeObject(response.Content);

			var rawData = WaitForTranslation(json?.requestId?.Value);

			json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(rawData));
			return json != null ? json.translation[0] : string.Empty;
		}

		public int GetUserInformation()
		{
			RestRequest request;
			if (_authenticationMethod.Equals(Enums.GetDisplayName(Enums.LoginOptions.APICredentials)))
			{
				request = new RestRequest("/accounts/api-credentials/self")
				{
					RequestFormat = DataFormat.Json
				};
			}
			else
			{
				request = new RestRequest("/accounts/users/self")
				{
					RequestFormat = DataFormat.Json
				};
			}
			var traceId = GetTraceId(request);
			var response = _client.Execute(request);
			var user = JsonConvert.DeserializeObject<UserDetails>(response.Content);
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				// Get refresh token
				var token = _studioCredentials.EnsureValidConnection();
				if (!string.IsNullOrEmpty(token))
				{
					// Update authorization parameters
					UpdateRequestHeadersForRefreshToken(request, token);

					var userInfoResponse = _client.Execute(request);
					if (userInfoResponse.StatusCode == HttpStatusCode.OK)
					{
						return JsonConvert.DeserializeObject<UserDetails>(userInfoResponse.Content).AccountId;
					}
					if (userInfoResponse.StatusCode == HttpStatusCode.Unauthorized)
					{
						MessageBox.Show("Unauthorized: Please check your credentials", PluginName, MessageBoxButton.OK);

						Log.Logger.Error($"Unauthorized: Get UserInfo using refresh token\nTrace-Id: {traceId}");
					}
					else if (userInfoResponse.StatusCode != HttpStatusCode.OK && userInfoResponse.StatusCode != HttpStatusCode.Unauthorized)
					{
						ShowErrors(userInfoResponse);
					}
				}
			}
			else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized)
			{
				ShowErrors(response);
			}

			return user?.AccountId ?? 0;
		}

		private void UpdateRequestHeadersForRefreshToken(IRestRequest request, string token)
		{
			// Update authorization parameters
			_client.RemoveDefaultParameter("Authorization");
			_client.AddDefaultHeader("Authorization", $"Bearer {token}");
			request.AddOrUpdateParameter("Authorization", $"Bearer {token}");
		}
		public SubscriptionInfo GetLanguagePairs(string accountId)
		{
			try
			{
				var request = new RestRequest($"/accounts/{accountId}/subscriptions/language-pairs")
				{
					RequestFormat = DataFormat.Json
				};
				var traceId =GetTraceId(request);

				var response = _client.Execute(request);
				if (response.StatusCode == HttpStatusCode.Unauthorized)
				{
					// Get refresh token
					var token = _studioCredentials.EnsureValidConnection();
					if (!string.IsNullOrEmpty(token))
					{
						// Update authorization parameters
						UpdateRequestHeadersForRefreshToken(request, token);
						var languagePairsResponse = _client.Execute(request);

						if (languagePairsResponse.StatusCode == HttpStatusCode.OK)
						{
							return JsonConvert.DeserializeObject<SubscriptionInfo>(languagePairsResponse.Content); 
						}
						if (languagePairsResponse.StatusCode == HttpStatusCode.Unauthorized)
						{
							MessageBox.Show("Unauthorized: Please check your credentials", PluginName, MessageBoxButton.OK);

							Log.Logger.Error($"Unauthorized: Get Language Pairs using refresh token \nTrace-Id: {traceId}");
						}
						else if (languagePairsResponse.StatusCode != HttpStatusCode.OK && languagePairsResponse.StatusCode != HttpStatusCode.Unauthorized)
						{
							ShowErrors(languagePairsResponse);
						}
					}
				}
				else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized)
				{
					ShowErrors(response);
				}
				var subscriptionInfo = JsonConvert.DeserializeObject<SubscriptionInfo>(response.Content);
				return subscriptionInfo;
			}
			catch (Exception e)
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
					if (!response.IsSuccessful || response.StatusCode != HttpStatusCode.OK)
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
				if (!response.IsSuccessful || response.StatusCode != HttpStatusCode.OK)
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
			GetTraceId(request);

			var response = _client.Execute(request);
			return response;
		}
		private string GetTraceId(IRestRequest request)
		{
			var pluginVersion = VersionHelper.GetPluginVersion();
			var studioVersion = VersionHelper.GetStudioVersion();
			var traceId = $"BeGlobal {pluginVersion} - {studioVersion} - {Guid.NewGuid().ToString()}";
			request.AddHeader("Trace-ID", traceId);
			return traceId;
		}

		private void ShowErrors(IRestResponse response)
		{
			var responseContent = JsonConvert.DeserializeObject<ResponseError>(response.Content);

			if (response.StatusCode == HttpStatusCode.Forbidden)
			{
				MessageBox.Show("Forbidden: Please check your license", "BeGlobal translation provider", MessageBoxButton.OK);
				throw new Exception("Forbidden: Please check your license");
			}
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