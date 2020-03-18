using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Studio;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class SdlMTCloudTranslator
	{
		private readonly IRestClient _client;
		private readonly IMessageBoxService _messageBoxService;
		private readonly LanguageMappingsService _languageMappingService;
		private readonly List<LanguageMappingModel> _languageMappings = new List<LanguageMappingModel>();

		public static readonly Log Log = Log.Instance;

		public SdlMTCloudTranslator(string server, SdlMTCloudTranslationOptions options)
		{
			try
			{
				_messageBoxService = new MessageBoxService();
				_languageMappingService = new LanguageMappingsService();
				_languageMappings = _languageMappingService.GetLanguageMappingSettings()?.LanguageMappings?.ToList();
				_client = new RestClient(string.Format($"{server}/v4"));

				Utils.LogServerIPAddresses();

				IRestRequest request;
				if (options.AuthenticationMethod.Equals("ClientLogin"))
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

		/// <summary>
		/// Get client information from the MTCloud server
		/// </summary>
		/// <returns>accountId</returns>
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

		/// <summary>
		/// Get translation for the current source text
		/// </summary>
		/// <param name="text">source text</param>
		/// <param name="sourceDisplayName">source language display name</param>
		/// <param name="targetDisplayName">target language display name</param>
		/// <returns>translated text</returns>
		public string TranslateText(string text, string sourceDisplayName, string targetDisplayName)
		{
			try
			{
				var selectedModel = _languageMappings?.FirstOrDefault(l => l.ProjectLanguagePair.Contains(sourceDisplayName) && l.ProjectLanguagePair.Contains(targetDisplayName));
				if(selectedModel?.SelectedModelOption == null || string.IsNullOrEmpty(selectedModel?.SelectedModelOption?.Model))
				{
					throw new Exception(Constants.NoTranslationMessage);
				}

				var request = new RestRequest("/mt/translations/async", Method.POST)
				{
					RequestFormat = DataFormat.Json
				};
				AddTraceId(request);

				string[] texts = { text };
				// set dictionaries parameter in case user has been selected an available dictionary
				if (!selectedModel.SelectedMTCloudDictionary.Name.Equals(Constants.NoAvailableDictionary)
					&& !selectedModel.SelectedMTCloudDictionary.Name.Equals(Constants.NoDictionary))
				{
					request.AddBody(new
					{
						input = texts,
						sourceLanguageId = selectedModel.SelectedMTCodeSource.CodeName,
						targetLanguageId = selectedModel.SelectedMTCodeTarget.CodeName,
						model = selectedModel?.SelectedModelOption?.Model,
						inputFormat = "xliff",
						dictionaries = new[] { selectedModel?.SelectedMTCloudDictionary?.DictionaryId?.ToString() }
					});
				}
				else
				{
					request.AddBody(new
					{
						input = texts,
						sourceLanguageId = selectedModel.SelectedMTCodeSource.CodeName,
						targetLanguageId = selectedModel.SelectedMTCodeTarget.CodeName,
						model = selectedModel.SelectedModelOption?.Model,
						inputFormat = "xliff"
					});
				}
				var response = _client.Execute(request);
				if (!response.IsSuccessful)
				{
					ShowErrors(response);

					if (response.StatusCode == 0)
					{
						throw new WebException(Constants.InternetConnection);
					}
				}
				dynamic json = JsonConvert.DeserializeObject(response.Content);

				var rawData = WaitForTranslation(json?.requestId?.Value);

				json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(rawData));
				return json != null ? json.translation[0] : string.Empty;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.TranslateTextMethod} {e.Message}\n {e.StackTrace}");
				throw;
			}
		}

		/// <summary>
		/// Get user information from the MTCloud server
		/// </summary>
		/// <returns>accountId</returns>
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
				return user?.AccountId ?? 0;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.GetUserInformation} { e.Message}\n {e.StackTrace}");
			}
			return 0;
		}

		/// <summary>
		/// Get language pairs for the current accountId
		/// </summary>
		/// <param name="accountId">accountId</param>
		/// <returns></returns>
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

		/// <summary>
		/// Get all the dictionaries for the specified accountId
		/// </summary>
		/// <param name="accountId">accountId for the current user</param>
		/// <returns>available dictionaries for current user</returns>
		public MTCloudDictionaryInfo GetDictionaries(int accountId)
		{
			try
			{
				var request = new RestRequest($"/accounts/{accountId}/dictionaries")
				{
					RequestFormat = DataFormat.Json
				};
				AddTraceId(request);

				var response = _client.Execute(request);
				if (!response.IsSuccessful)
				{
					ShowErrors(response);
				}
				var dictionaries = JsonConvert.DeserializeObject<MTCloudDictionaryInfo>(response.Content);
				return dictionaries;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.GetDictionaries} {e.Message}\n {e.StackTrace}");
			}
			return new MTCloudDictionaryInfo();
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
							throw new WebException(Constants.InternetConnection);
						}
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
					if (response.StatusCode == 0)
					{
						throw new WebException(Constants.InternetConnection);
					}
				}
				return response.RawBytes;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.WaitTranslationMethod} {e.Message}\n {e.StackTrace}");
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
			request.AddHeader(Constants.TraceId, $"{Constants.SDLMachineTranslationCloudProvider} {pluginVersion} - {studioVersion}.{Guid.NewGuid().ToString()}");
		}

		private void ShowErrors(IRestResponse response)
		{
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