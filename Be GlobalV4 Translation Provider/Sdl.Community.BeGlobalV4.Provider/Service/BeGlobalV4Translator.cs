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
		private readonly string _url = "https://translate-api.sdlbeglobal.com";
		public static readonly Log Log = Log.Instance;
		//private string AccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IlJUbEZSVGMxTnpCRE9EbEdSRFl5TVRsRk1rUTNPRGhEUWtSRFFUTXhSRVUxTlVVME1EUXhRUSJ9.eyJodHRwczovL3NkbC5jb20vZW1haWwiOiJhZ2hpc2FAc2RsLmNvbSIsImlzcyI6Imh0dHBzOi8vc2RsLXByb2QuZXUuYXV0aDAuY29tLyIsInN1YiI6ImFkfEdMT0JBTC1TREwtQ09SUHxmOTFkNGYyNi05MDU1LTQwMGYtOWQ2Ny05ZDliOGVhMzU1ZWIiLCJhdWQiOlsiaHR0cHM6Ly9hcGkuc2RsLmNvbSIsImh0dHBzOi8vc2RsLXByb2QuZXUuYXV0aDAuY29tL3VzZXJpbmZvIl0sImlhdCI6MTU2Mzk2ODkxMywiZXhwIjoxNTY0MDU1MzEzLCJhenAiOiJMeFFGYlRaeGsyUGM3bWcwTjRSZklPcDJXQ2V5cHV6TiIsInNjb3BlIjoib3BlbmlkIGVtYWlsIHByb2ZpbGUgb2ZmbGluZV9hY2Nlc3MifQ.Ax-QwW8KuDJO1ZWFRMZaOVmLzrgkViuXGajJfz8T6CxAhTmB6Kd4TaIkhsA3KgtWRyTqnK8D70-HIldERc7pHveOQkD6JYgGesdSoSNDGwTZE_DaXd3TjArUvvbAwMsuY_tSzZHzD8OGz5w7JcV6ejw4brPKBrJ26Hi75x2rz3PcTbYKkQAjB5vlbiV7yYEFAaIG32gST-9ctTa_slhzXBFdz6tzGgoin_IC6FIHEzV1cAUfgl8AmXpFRJEFDPLKMjn6QT3eR2TixufeWysflaXnQAW3sJeKP12jqy_YwChaFF4JnwlwlGwtjyS9ov-DD6hHTo6Nb4zh24rA8QlXcw\",\"refresh_token\":\"s7jjJOyvfoXEQ8X9bAAsM0MZ408PzxBeK45kx_iX5rgSS\",\"id_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IlJUbEZSVGMxTnpCRE9EbEdSRFl5TVRsRk1rUTNPRGhEUWtSRFFUTXhSRVUxTlVVME1EUXhRUSJ9.eyJlbWFpbCI6ImFnaGlzYUBzZGwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImdpdmVuX25hbWUiOiJBbmRyZWEtTWVsaW5kYSIsImZhbWlseV9uYW1lIjoiR2hpc2EiLCJuaWNrbmFtZSI6ImFnaGlzYSIsIm5hbWUiOiJBbmRyZWEtTWVsaW5kYSBHaGlzYSIsInBpY3R1cmUiOiJodHRwczovL3MuZ3JhdmF0YXIuY29tL2F2YXRhci9hNTVjOTM5NGU4ZjIzZGZjYmJmYzU3ZjY4YjFjZWFlMj9zPTQ4MCZyPXBnJmQ9aHR0cHMlM0ElMkYlMkZjZG4uYXV0aDAuY29tJTJGYXZhdGFycyUyRmFnLnBuZyIsInVwZGF0ZWRfYXQiOiIyMDE5LTA3LTI0VDExOjQ4OjMyLjcyMVoiLCJpc3MiOiJodHRwczovL3NkbC1wcm9kLmV1LmF1dGgwLmNvbS8iLCJzdWIiOiJhZHxHTE9CQUwtU0RMLUNPUlB8ZjkxZDRmMjYtOTA1NS00MDBmLTlkNjctOWQ5YjhlYTM1NWViIiwiYXVkIjoiTHhRRmJUWnhrMlBjN21nME40UmZJT3AyV0NleXB1ek4iLCJpYXQiOjE1NjM5Njg5MTMsImV4cCI6MTU2NDAwNDkxM30.bDjlb_qz8hnfMFfYnm-Mr3tyervzo3Qq7aiP97s7UrH0lE7cGwFs0fSlzE7YHQHGwAcS0d1wtYwvqoaurtlwY9mwYB9bhM4mA4OwdVNvBf_OdMC_eYnb6MjmiUzo5qWCoJhOmXSKUuYtUNRGmzA5Hpu0B4GdQfivgCKFBJo72eJPToCmlhhQUc6ofNFG9zwxaXb21EeUHCBnJXErBxRHnXHC6qXUMcvPJdjJv7uENE6TM-OLaEy3NWTWYE_MnPfeDHBqKprVVUIjhGhgToDYtyGyKm7csVodOOWlfA9R7EwDcyTYm2CS_RjW_8Zc6fYG166lcZxQEecShFnhpeHEtw";
		private readonly StudioCredentials _studioCredentials;

		private string ExpiredAccessToken =
				"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IlJUbEZSVGMxTnpCRE9EbEdSRFl5TVRsRk1rUTNPRGhEUWtSRFFUTXhSRVUxTlVVME1EUXhRUSJ9.eyJodHRwczovL3NkbC5jb20vZW1haWwiOiJhZ2hpc2FAc2RsLmNvbSIsImlzcyI6Imh0dHBzOi8vc2RsLXByb2QuZXUuYXV0aDAuY29tLyIsInN1YiI6ImFkfEdMT0JBTC1TREwtQ09SUHxmOTFkNGYyNi05MDU1LTQwMGYtOWQ2Ny05ZDliOGVhMzU1ZWIiLCJhdWQiOlsiaHR0cHM6Ly9hcGkuc2RsLmNvbSIsImh0dHBzOi8vc2RsLXByb2QuZXUuYXV0aDAuY29tL3VzZXJpbmZvIl0sImlhdCI6MTU2Mzk2ODkxMywiZXhwIjoxNTY0MDU1MzEzLCJhenAiOiJMeFFGYlRaeGsyUGM3bWcwTjRSZklPcDJXQ2V5cHV6TiIsInNjb3BlIjoib3BlbmlkIGVtYWlsIHByb2ZpbGUgb2ZmbGluZV9hY2Nlc3MifQ.Ax-QwW8KuDJO1ZWFRMZaOVmLzrgkViuXGajJfz8T6CxAhTmB6Kd4TaIkhsA3KgtWRyTqnK8D70-HIldERc7pHveOQkD6JYgGesdSoSNDGwTZE_DaXd3TjArUvvbAwMsuY_tSzZHzD8OGz5w7JcV6ejw4brPKBrJ26Hi75x2rz3PcTbYKkQAjB5vlbiV7yYEFAaIG32gST-9ctTa_slhzXBFdz6tzGgoin_IC6FIHEzV1cAUfgl8AmXpFRJEFDPLKMjn6QT3eR2TixufeWysflaXnQAW3sJeKP12jqy_YwChaFF4JnwlwlGwtjyS9ov-DD6hHTo6Nb4zh24rA8QlXcw"
			;

		private string RefreshToken =
				"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IlJUbEZSVGMxTnpCRE9EbEdSRFl5TVRsRk1rUTNPRGhEUWtSRFFUTXhSRVUxTlVVME1EUXhRUSJ9.eyJodHRwczovL3NkbC5jb20vZW1haWwiOiJhZ2hpc2FAc2RsLmNvbSIsImlzcyI6Imh0dHBzOi8vc2RsLXByb2QuZXUuYXV0aDAuY29tLyIsInN1YiI6ImFkfEdMT0JBTC1TREwtQ09SUHxmOTFkNGYyNi05MDU1LTQwMGYtOWQ2Ny05ZDliOGVhMzU1ZWIiLCJhdWQiOlsiaHR0cHM6Ly9hcGkuc2RsLmNvbSIsImh0dHBzOi8vc2RsLXByb2QuZXUuYXV0aDAuY29tL3VzZXJpbmZvIl0sImlhdCI6MTU2NjE5OTEwMywiZXhwIjoxNTY2Mjg1NTAzLCJhenAiOiJMeFFGYlRaeGsyUGM3bWcwTjRSZklPcDJXQ2V5cHV6TiIsInNjb3BlIjoib3BlbmlkIGVtYWlsIHByb2ZpbGUgb2ZmbGluZV9hY2Nlc3MifQ.1vyEW13wfg-LawicbLTNNvEUgQFgvBDNmfEOIcBixO900lOpGvd9CwVXA-zcnaJEtiUbw7uq7DiaexQeBgyglfoPl6XPuI9rKnAxnT5VpaPF9z9oF80SUHwxKIGHwnO7An0AO-lQT6CtdimVsSQGrmGRABz4Rc17Dl1WPsFlXWwYpNjs-fOE8kGsMvTsFCInH53nsV3jwVyAg2GWcD_a6kyzfKaka70wUsTJss66AYp9uwiM2BglnOJHnxxzA9upnBoYuHrD93HCWVMJe_CspyQ5NPL7HOFYnDZkzrRNEtVoCCUySxARzEQXkMvQbRMHVIDisNZAsuB1lHuyFiONbg"
			;

		private const string PluginName = "SDL BeGlobal (NMT) Translation Provider";

		public BeGlobalV4Translator(string flavor)
		{
			_flavor = flavor;
			 _studioCredentials = new StudioCredentials();
			//var accessToken = string.Empty;

			//Application.Current?.Dispatcher?.Invoke(() =>
			//{
			//	accessToken = studioCredentials.GetToken();
			//});
			//accessToken = studioCredentials.GetToken();


			_client = new RestClient($"{_url}/v4")
			{
				CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
			};

			if (!string.IsNullOrEmpty(ExpiredAccessToken))
			{
				_client.AddDefaultHeader("Authorization", $"Bearer {ExpiredAccessToken}");
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
				GetTraceId(request);

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
				if (!response.IsSuccessful || response.StatusCode != HttpStatusCode.OK)
				{
					ShowErrors(response);
				}
				dynamic json = JsonConvert.DeserializeObject(response.Content);

				var rawData = WaitForTranslation(json?.requestId?.Value);

				json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(rawData));
				return json != null ? json.translation[0] : string.Empty;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"Translate text method: {e.Message}\n {e.StackTrace}");
			}
			return string.Empty;
		}

		public int GetUserInformation()
		{

			var request = new RestRequest("/accounts/users/self")
			{
				RequestFormat = DataFormat.Json
			};
			var traceId = GetTraceId(request);

			var response = _client.Execute(request);
			var user = JsonConvert.DeserializeObject<UserDetails>(response.Content);
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				// Get refresh token
				var token = _studioCredentials.EnsureValidConnection();
				if (!string.IsNullOrEmpty(token))
				{
					var newClient = new RestClient($"{_url}/v4")
					{
						CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
					};
					newClient.AddDefaultHeader("Authorization", $"Bearer {token}");
					var userInfoResponse = newClient.Execute(request);
					//_client.RemoveDefaultParameter("Authorization");
					//_client.AddDefaultHeader("Authorization", $"Bearer {token}");
					//var userInfoResponse = _client.Execute(request);

					if (userInfoResponse.StatusCode == HttpStatusCode.Unauthorized)
					{
						MessageBox.Show("Unauthorized: Please check your credentials", PluginName, MessageBoxButton.OK);

						Log.Logger.Error($"Unauthorized: Get UserInfo with Refresh Token: \n {token} \n Trace-Id: {traceId}");
					}
					else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized)
					{
						ShowErrors(response);
					}
				}
			}
			else if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Unauthorized)
			{
				ShowErrors(response);
			}

			return user?.AccountId ?? 0;
		}

		public SubscriptionInfo GetLanguagePairs(string accountId)
		{
			try
			{
				var request = new RestRequest($"/accounts/{accountId}/subscriptions/language-pairs")
				{
					RequestFormat = DataFormat.Json
				};
				GetTraceId(request);

				var response = _client.Execute(request);
				if (!response.IsSuccessful || response.StatusCode != HttpStatusCode.OK)
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
			var traceId = $"BeGlobal {pluginVersion} - {studioVersion}.{Guid.NewGuid().ToString()}";
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