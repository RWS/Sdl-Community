using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Converter = Sdl.Community.MTCloud.Provider.XliffConverter.Converter.Converter;
using LogManager = NLog.LogManager;
using Task = System.Threading.Tasks.Task;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class TranslationService : ITranslationService
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public TranslationService(IConnectionService connectionService)
		{
			ConnectionService = connectionService;
		}

		public event TranslationFeedbackEventRaiser TranslationReceived;

		public IConnectionService ConnectionService { get; }
		public Options Options { get; set; }

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
						_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{PluginResources.Message_Connection_token_has_expired}\n {ConnectionService.Credential.Token}");
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

					_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{responseMessage.StatusCode}\n {responseMessage.RequestMessage}");
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{ex.Message}\n {ex.StackTrace}");
				throw;
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
						_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{PluginResources.Message_Connection_token_has_expired}\n {ConnectionService.Credential.Token}");
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

					_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{responseMessage.StatusCode}\n {responseMessage.RequestMessage}");
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{ex.Message}\n {ex.StackTrace}");
				throw;
			}

			return null;
		}

		public async Task SendFeedback(SegmentId? segmentId, dynamic rating, string originalText, string improvement)
		{
			var feedbackRequest = CreateFeedbackRequest(segmentId, rating, originalText, improvement);
			await SendFeedback(feedbackRequest);
		}

		private async Task SendFeedback(dynamic translationFeedback)
		{
			if (ConnectionService.Credential.ValidTo < DateTime.UtcNow)
			{
				// attempt one connection
				var success = ConnectionService.Connect(ConnectionService.Credential);
				if (!success.Item1)
				{
					_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{PluginResources.Message_Connection_token_has_expired}\n {ConnectionService.Credential.Token}");
					throw new Exception(PluginResources.Message_Connection_token_has_expired);
				}
			}

			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConnectionService.Credential.Token);

				var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4/accounts/{ConnectionService.Credential.AccountId}/feedback/translations");
				var request = new HttpRequestMessage(HttpMethod.Post, uri);
				ConnectionService.AddTraceHeader(request);

				var serializerSettings =
					new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
				var content = JsonConvert.SerializeObject(translationFeedback, serializerSettings);

				request.Content = new StringContent(content, new UTF8Encoding(), "application/json");

				var responseMessage = await httpClient.SendAsync(request);
				var response = await responseMessage.Content.ReadAsStringAsync();

				_logger.Info(PluginResources.SendFeedbackResponseFromServer, responseMessage.StatusCode, response);
			}
		}

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
					_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + $"{PluginResources.Message_Connection_token_has_expired}\n {ConnectionService.Credential.Token}");
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

		private dynamic CreateFeedbackRequest(SegmentId? segmentId, dynamic rating, string originalText, string improvement)
		{
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();

			var activeDocument = editorController.ActiveDocument;
			var currentProject = editorController.ActiveDocument.Project.GetProjectInfo();

			var segmentSource = segmentId != null
				? activeDocument.SegmentPairs.ToList().FirstOrDefault(sp => sp.Properties.Id.Equals(segmentId))?.Source.ToString()
				: activeDocument.ActiveSegmentPair.Source.ToString();

			var model = GetCorrespondingLanguageMappingModel(currentProject, activeDocument);
			var translationFeedbackRequest = new TranslationFeedbackRequest
			{
				Model = model?.SelectedModel.Model,
				SourceLanguageId = model?.SelectedSource.CodeName,
				SourceText = segmentSource,
				TargetLanguageId = model?.SelectedTarget.CodeName,
				TargetMtText = originalText
			};

			dynamic feedbackRequest = new ExpandoObject();
			if (!string.IsNullOrWhiteSpace(improvement))
			{
				var improvementObject = new Improvement { Text = improvement };
				feedbackRequest.Improvement = improvementObject;
			}
			if (rating != null)
			{
				feedbackRequest.Rating = rating;
			}
			feedbackRequest.Translation = translationFeedbackRequest;

			return feedbackRequest;
		}

		private LanguageMappingModel GetCorrespondingLanguageMappingModel(ProjectInfo currentProject, Document activeDocument)
		{
			var model = Options.LanguageMappings?.FirstOrDefault(l =>
				l.SourceTradosCode.Equals(currentProject.SourceLanguage.IsoAbbreviation,
					StringComparison.InvariantCultureIgnoreCase) &&
				l.TargetTradosCode.Equals(activeDocument.ActiveFile.Language.IsoAbbreviation,
					StringComparison.InvariantCultureIgnoreCase));
			return model;
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
	}
}