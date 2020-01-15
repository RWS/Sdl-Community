using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly IMessageBoxService _messageBoxService;
		private Constants _constants = new Constants();
		private LanguageMappingsService _languageMappingService;
		private List<LanguageMappingModel> _languageMappings = new List<LanguageMappingModel>();

		public static readonly Log Log = Log.Instance;

		public BeGlobalV4Translator(string server, BeGlobalTranslationOptions options)
		{
			try
			{
				_messageBoxService = new MessageBoxService();
				_languageMappingService = new LanguageMappingsService();
				_languageMappings = _languageMappingService.GetLanguageMappingSettings()?.LanguageMappings?.ToList();

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
					throw new Exception(_constants.TokenFailed + response.Content);
				}
				dynamic json = JsonConvert.DeserializeObject(response.Content);
				_client.AddDefaultHeader("Authorization", $"Bearer {json.accessToken}");
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{_constants.BeGlobalV4Translator} {ex.Message}\n {ex.StackTrace}");
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
				Log.Logger.Error($"{_constants.GetClientInformation} {e.Message}\n {e.StackTrace}");
			}
			return 0;
		}

		public string TranslateText(string text, string sourceLanguage, string targetLanguage)
		{
			try
			{
				var projectLanguagePair = $"{sourceLanguage} - {targetLanguage}";
				var selectedModel = _languageMappings?.FirstOrDefault(l => l.ProjectLanguagePair.Contains(sourceLanguage) && l.ProjectLanguagePair.Contains(targetLanguage));

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
					model = selectedModel?.SelectedModelOption?.Model,
					inputFormat = "xliff"
				});
				var response = _client.Execute(request);
				if (!response.IsSuccessful)
				{
					ShowErrors(response);

					if (response.StatusCode == 0)
					{
						throw new WebException(_constants.InternetConnection);
					}
				}
				dynamic json = JsonConvert.DeserializeObject(response.Content);

				var rawData = WaitForTranslation(json?.requestId?.Value);

				json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(rawData));
				return json != null ? json.translation[0] : string.Empty;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{_constants.TranslateTextMethod} {e.Message}\n {e.StackTrace}");
				throw;
			}
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
				Log.Logger.Error($"{_constants.GetUserInformation} { e.Message}\n {e.StackTrace}");
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
				Log.Logger.Error($"{_constants.SubscriptionInfoMethod} {e.Message}\n {e.StackTrace}");
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

						if (response.StatusCode == 0)
						{
							throw new WebException(_constants.InternetConnection);
						}
					}

					dynamic json = JsonConvert.DeserializeObject(response.Content);
					if (json == null)
					{
						return new byte[1];
					}
					status = json.translationStatus;

					if (!status.Equals(_constants.DONE, StringComparison.CurrentCultureIgnoreCase))
					{
						System.Threading.Thread.Sleep(300);
					}
					if (status.Equals(_constants.FAILED))
					{
						ShowErrors(response);
					}
				} while (status.Equals(_constants.INIT, StringComparison.CurrentCultureIgnoreCase) ||
						 status.Equals(_constants.TRANSLATING, StringComparison.CurrentCultureIgnoreCase));

				response = RestGet($"/mt/translations/async/{id}/content");
				if (!response.IsSuccessful)
				{
					ShowErrors(response);
					if (response.StatusCode == 0)
					{
						throw new WebException(_constants.InternetConnection);
					}
				}
				return response.RawBytes;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{_constants.WaitTranslationMethod} {e.Message}\n {e.StackTrace}");
				throw;
			}
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
			request.AddHeader(_constants.TraceId, $"{_constants.SDLMachineTranslationCloudProvider} {pluginVersion} - {studioVersion}.{Guid.NewGuid().ToString()}");
		}

		private void ShowErrors(IRestResponse response)
		{
			var responseContent = JsonConvert.DeserializeObject<ResponseError>(response?.Content);
			if (responseContent?.Errors != null)
			{
				foreach (var error in responseContent.Errors)
				{
					throw new Exception($"{_constants.ErrorCode} {error.Code}, {error.Description}");
				}
			}
		}
	}
}