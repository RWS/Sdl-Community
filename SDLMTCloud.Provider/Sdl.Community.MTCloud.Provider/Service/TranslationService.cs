using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.Toolkit.LanguagePlatform.XliffConverter;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class TranslationService : ITranslationService
	{
		public event TranslationFeedbackEventRaiser TranslationReceived;

		public TranslationService(IConnectionService connectionService)
		{
			ConnectionService = connectionService;
		}

		public IConnectionService ConnectionService { get; }

		
		public async Task<Segment[]> TranslateText(string text, LanguageMappingModel model)
		{			
			if (string.IsNullOrEmpty(model?.SelectedModel?.Model))
			{
				throw new Exception(PluginResources.Message_No_model_selected);
			}

			if (ConnectionService.Credential.ValidTo < DateTime.UtcNow)
			{
				// attempt one connection
				var success = ConnectionService.Connect(ConnectionService.Credential);
				if (!success.Item1)
				{
					Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{PluginResources.Message_Connection_token_has_expired}\n {ConnectionService.Credential.Token}");
					throw new Exception(PluginResources.Message_Connection_token_has_expired);
				}
			}

			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConnectionService.Credential.Token);

				var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4" + "/mt/translations/async");
				var request = new HttpRequestMessage(HttpMethod.Post, uri);
				ConnectionService.AddTraceHeader(request);

				var translationRequestModel = new TranslationRequest
				{
					Input = new[] { text },
					SourceLanguageId = model.SelectedSource.CodeName,
					TargetLanguageId = model.SelectedTarget.CodeName,
					Model = model.SelectedModel.Model,
					InputFormat = "xliff"
				};
				var feedback = new Feedback
				{
					SourceText = text,
					SourceLanguageId = model.SelectedSource.CodeName,
					TargetLanguageId = model.SelectedTarget.CodeName,
					EngineModel = model.SelectedModel.Model
				};

				if (!model.SelectedDictionary.Name.Equals(PluginResources.Message_No_dictionary_available)
					&& !model.SelectedDictionary.Name.Equals(PluginResources.Message_No_dictionary))
				{
					translationRequestModel.Dictionaries = new[] { model.SelectedDictionary?.DictionaryId };
				}

				var content = JsonConvert.SerializeObject(translationRequestModel);
				request.Content = new StringContent(content, new UTF8Encoding(), "application/json");


				var responseMessage = await httpClient.SendAsync(request);
				var response = await responseMessage.Content.ReadAsStringAsync();
				
				if (!responseMessage.IsSuccessStatusCode)
				{
					return null;
				}

				if (JsonConvert.DeserializeObject<TranslationResponse>(response) is TranslationResponse translationResponse)
				{
					var dataResponse = await GetTranslations(httpClient, translationResponse.RequestId);
					if (JsonConvert.DeserializeObject<TranslationResponse>(dataResponse) is TranslationResponse translations)
					{
						var translation = translations.Translation.FirstOrDefault();
						if (translation == null)
						{
							return null;
						}
						feedback.TargetMTText = translation;
						TranslationReceived?.Invoke(feedback);
						var translatedXliff = Converter.ParseXliffString(translation);
						if (translatedXliff != null)
						{
							var segments = translatedXliff.GetTargetSegments();
							return segments;
						}
					}
				}
			}

			return null;
		}

		public async Task<SubscriptionInfo> GetLanguagePairs(string accountId)
		{
			try
			{
				if (ConnectionService.Credential.ValidTo < DateTime.UtcNow)
				{
					// attempt one connection
					var success = ConnectionService.Connect(ConnectionService.Credential);
					if (!success.Item1)
					{
						Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{PluginResources.Message_Connection_token_has_expired}\n {ConnectionService.Credential.Token}");
						throw new Exception(PluginResources.Message_Connection_token_has_expired);
					}
				}

				using (var httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConnectionService.Credential.Token);

					var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4" + $"/accounts/{accountId}/subscriptions/language-pairs");
					var request = new HttpRequestMessage(HttpMethod.Get, uri);
					ConnectionService.AddTraceHeader(request);

					var responseMessage = await httpClient.SendAsync(request);
					var response = await responseMessage.Content.ReadAsStringAsync();

					if (responseMessage.StatusCode == HttpStatusCode.OK) // 200
					{
						var result = JsonConvert.DeserializeObject<SubscriptionInfo>(response);
						return result;
					}

					Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{responseMessage.StatusCode}\n {responseMessage.RequestMessage}");
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{ex.Message}\n {ex.StackTrace}");
				throw;
			}

			return null;
		}

		public async Task<MTCloudDictionaryInfo> GetDictionaries(string accountId)
		{
			try
			{
				if (ConnectionService.Credential.ValidTo < DateTime.UtcNow)
				{
					// attempt one connection
					var success = ConnectionService.Connect(ConnectionService.Credential);
					if (!success.Item1)
					{
						Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{PluginResources.Message_Connection_token_has_expired}\n {ConnectionService.Credential.Token}");
						throw new Exception(PluginResources.Message_Connection_token_has_expired);
					}
				}

				using (var httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConnectionService.Credential.Token);

					var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4" + $"/accounts/{accountId}/dictionaries");
					var request = new HttpRequestMessage(HttpMethod.Get, uri);
					ConnectionService.AddTraceHeader(request);

					var responseMessage = await httpClient.SendAsync(request);
					var response = await responseMessage.Content.ReadAsStringAsync();

					if (responseMessage.StatusCode == HttpStatusCode.OK) // 200
					{
						var result = JsonConvert.DeserializeObject<MTCloudDictionaryInfo>(response);
						return result;
					}

					Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{responseMessage.StatusCode}\n {responseMessage.RequestMessage}");
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{ex.Message}\n {ex.StackTrace}");
				throw;
			}

			return null;
		}


		private async Task<string> GetTranslations(HttpClient httpClient, string id)
		{
			var translationStatus = string.Empty;

			do
			{
				var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4" + $"/mt/translations/async/{id}");
				var request = new HttpRequestMessage(HttpMethod.Get, uri);
				ConnectionService.AddTraceHeader(request);

				var responseMessage = await httpClient.SendAsync(request);
				var response = await responseMessage.Content.ReadAsStringAsync();

				if (!responseMessage.IsSuccessStatusCode)
				{
					if (responseMessage.StatusCode == 0)
					{
						throw new WebException(Constants.InternetConnection);
					}
				}

				if (JsonConvert.DeserializeObject<TranslationResponseStatus>(response) is TranslationResponseStatus responseStatus)
				{
					translationStatus = responseStatus.TranslationStatus;

					if (string.Compare(responseStatus.TranslationStatus, Constants.DONE, StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						System.Threading.Thread.Sleep(300);
					}

					if (string.Compare(responseStatus.TranslationStatus, Constants.FAILED, StringComparison.CurrentCultureIgnoreCase) == 0)
					{
						var responseContent = JsonConvert.DeserializeObject<ResponseError>(response);
						if (responseContent?.Errors != null)
						{
							foreach (var error in responseContent.Errors)
							{
								throw new Exception($"{Constants.ErrorCode} {error.Code}, {error.Description}");
							}
						}
						else
						{
							throw new Exception($"{Constants.ErrorCode} {responseMessage.StatusCode}, {responseMessage.Content}");
						}
					}
				}

			} while (string.Compare(translationStatus, Constants.INIT, StringComparison.CurrentCultureIgnoreCase) == 0
					 || string.Compare(translationStatus, Constants.TRANSLATING, StringComparison.CurrentCultureIgnoreCase) == 0);


			return await GetTranslationResult(httpClient, id);
		}

		private async Task<string> GetTranslationResult(HttpClient httpClient, string id)
		{
			var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4" + $"/mt/translations/async/{id}/content");
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			ConnectionService.AddTraceHeader(request);

			var responseMessage = await httpClient.SendAsync(request);
			var response = await responseMessage.Content.ReadAsStringAsync();

			if (responseMessage.IsSuccessStatusCode)
			{
				return response;
			}

			return string.Empty;
		}
	}
}
