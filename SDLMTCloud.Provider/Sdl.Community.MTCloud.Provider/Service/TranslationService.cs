using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Model.RateIt;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.LanguagePlatform.Core;
using Converter = Sdl.Community.MTCloud.Provider.XliffConverter.Converter.Converter;
using LogManager = NLog.LogManager;
using Task = System.Threading.Tasks.Task;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class TranslationService : ITranslationService
	{
		private readonly IHttpClient _httpClient;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly IMessageBoxService _messageService;

		public TranslationService(IConnectionService connectionService, IHttpClient httpClient, IMessageBoxService messageService)
		{
			_httpClient = httpClient;
			_httpClient.SetLogger(_logger);

			_messageService = messageService;
			ConnectionService = connectionService;
		}

		public event TranslationReceivedEventHandler TranslationReceived;

		public IConnectionService ConnectionService { get; }

		public bool IsActiveModelQeEnabled
			=> GetCorrespondingLanguageMappingModel()?.SelectedModel.Model.ToLower().Contains("qe") ?? false;

		public Options Options { get; set; }

		public async Task AddTermToDictionary(Term term)
		{
			CheckConnection();

			var model = GetCorrespondingLanguageMappingModel();
			var dictionaryId = model.SelectedDictionary.DictionaryId;

			if (string.IsNullOrWhiteSpace(dictionaryId))
			{
				_messageService.ShowWarningMessage(PluginResources.No_dictionary_has_been_selected, PluginResources.Operation_failed);
				return;
			}

			var uri = new Uri($@"{Constants.MTCloudTranslateAPIUri}/v4/accounts/{ConnectionService.Credential.AccountId}/dictionaries/{dictionaryId}/terms");
			var request = GetRequestMessage(HttpMethod.Post, uri);

			var content = JsonConvert.SerializeObject(term);
			request.Content = new StringContent(content, new UTF8Encoding(), "application/json");

			var httpResponseMessage = await _httpClient.SendRequest(request);

			if (httpResponseMessage is not null)
			{
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					_messageService.ShowInformationMessage(PluginResources.The_term_has_been_successfully_added_to_the_current_dictionary,
						PluginResources.Operation_complete);
				}
				else
				{
					_messageService.ShowWarningMessage(httpResponseMessage.Content.ReadAsStringAsync().Result,
						PluginResources.Operation_failed);
				}
			}
		}

		public async Task<MTCloudDictionaryInfo> GetDictionaries()
		{
			CheckConnection();

			var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4/accounts/{ConnectionService.Credential.AccountId}/dictionaries");
			var request = GetRequestMessage(HttpMethod.Get, uri);

			var response = await _httpClient.SendRequest(request);
			return await _httpClient.GetResult<MTCloudDictionaryInfo>(response);
		}

		public async Task<SubscriptionInfo> GetLanguagePairs()
		{
			CheckConnection();

			var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4/accounts/{ConnectionService.Credential.AccountId}/subscriptions/language-pairs");
			var request = GetRequestMessage(HttpMethod.Get, uri);

			var response = await _httpClient.SendRequest(request);
			return await _httpClient.GetResult<SubscriptionInfo>(response);
		}

		public async Task<HttpResponseMessage> SendFeedback(FeedbackInfo feedbackInfo)
		{
			var feedbackRequest = CreateFeedbackRequest(feedbackInfo);
			return await SendFeedback(feedbackRequest);
		}

		public async Task<Segment[]> TranslateText(string text, LanguageMappingModel model, FileAndSegmentIds fileAndSegments)
		{
			if (string.IsNullOrEmpty(model?.SelectedModel?.Model))
			{
				throw new Exception(PluginResources.Message_No_model_selected);
			}

			CheckConnection();

			var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4/mt/translations/async");
			var request = GetRequestMessage(HttpMethod.Post, uri);

			var engineModel = model.SelectedModel.Model;
			var translationRequestModel = new TranslationRequest
			{
				Input = new[] { text },
				SourceLanguageId = model.SelectedSource.CodeName,
				TargetLanguageId = model.SelectedTarget.CodeName,
				Model = engineModel,
				InputFormat = "xliff",
				QualityEstimation = engineModel.ToLower().Contains("qe") ? 1 : 0
			};

			if (!model.SelectedDictionary.Name.Equals(PluginResources.Message_No_dictionary_available)
				&& !model.SelectedDictionary.Name.Equals(PluginResources.Message_No_dictionary))
			{
				translationRequestModel.Dictionaries = new[] { model.SelectedDictionary?.DictionaryId };
			}

			var content = JsonConvert.SerializeObject(translationRequestModel);
			request.Content = new StringContent(content, new UTF8Encoding(), "application/json");

			var response = await _httpClient.SendRequest(request);
			var translationResponse = await _httpClient.GetResult<TranslationResponse>(response);

			if (response is null) return null;

			var responseMessage = await CheckTranslationStatus(translationResponse?.RequestId);
			var translations = await _httpClient.GetResult<TranslationResponse>(responseMessage);

			var translation = translations?.Translation?.FirstOrDefault();
			if (translation == null) return null;

			var translatedXliff = Converter.ParseXliffString(translation);
			if (translatedXliff == null) return null;

			var targetSegments = translatedXliff.GetTargetSegments();
			var segmentIds = fileAndSegments.Segments.Keys.ToList();

			OnTranslationReceived(new TranslationData
			{
				TargetSegments =
					segmentIds.Select((segmentId, index) => (segmentId, target:targetSegments[index].Segment.ToString())).ToDictionary(
						x => x.segmentId,
						x => x.target),
				TranslationOriginData = new TranslationOriginData
				{
					Model = translations.Model,
					QualityEstimations = segmentIds.Select((segmentId, index) => (segmentId, targetSegments[index].QualityEstimation))
						.ToDictionary(x => x.segmentId, x => x.QualityEstimation)
				},
				FilePath = fileAndSegments.FilePath,
				Segments = fileAndSegments.Segments,
				TargetLanguage = model.TargetTradosCode
			});

			return targetSegments.Select(seg => seg.Segment).ToArray();
		}

		private static void WaitForTranslation(TranslationResponseStatus responseStatus)
		{
			if (responseStatus.TranslationStatus.ToUpperInvariant() == Constants.DONE)
			{
				System.Threading.Thread.Sleep(300);
			}
		}

		private void CheckConnection()
		{
			if (ConnectionService.Credential.ValidTo >= DateTime.UtcNow) return;

			// attempt one connection
			var success = ConnectionService.Connect(ConnectionService.Credential);
			if (success.Item1) return;

			_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " +
						  $"{PluginResources.Message_Connection_token_has_expired}\n {ConnectionService.Credential.Token}");

			throw new Exception(PluginResources.Message_Connection_token_has_expired);
		}

		private async Task<HttpResponseMessage> CheckTranslationStatus(string id)
		{
			var translationStatus = string.Empty;

			do
			{
				var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4/mt/translations/async/{id}");
				var request = GetRequestMessage(HttpMethod.Get, uri);

				var responseMessage = await _httpClient.SendRequest(request);
				var responseStatus = await _httpClient.GetResult<TranslationResponseStatus>(responseMessage);

				if (responseStatus is null) continue;

				WaitForTranslation(responseStatus);

				translationStatus = responseStatus.TranslationStatus;
				if (translationStatus.ToUpperInvariant() != Constants.FAILED) continue;

				var response = await _httpClient.GetResult<ResponseError>(responseMessage);

				if (response?.Errors != null)
				{
					foreach (var error in response.Errors)
					{
						_logger.Error($"{nameof(CheckTranslationStatus)}: {error}");
					}
				}
				else
				{
					_logger.Error($"{nameof(CheckTranslationStatus)}: {responseMessage.StatusCode}, {responseMessage.Content}");
				}
			} while (translationStatus.ToUpperInvariant() == Constants.INIT || translationStatus.ToUpperInvariant() == Constants.TRANSLATING);

			return await GetTranslationResult(id);
		}

		private dynamic CreateFeedbackRequest(FeedbackInfo feedbackInfo)
		{
			var model = GetCorrespondingLanguageMappingModel();
			dynamic translationFeedbackRequest = new ExpandoObject();
			translationFeedbackRequest.Model = model?.SelectedModel.Model;
			translationFeedbackRequest.SourceLanguageId = model?.SelectedSource.CodeName;
			translationFeedbackRequest.SourceText = feedbackInfo.SegmentSource;
			translationFeedbackRequest.TargetLanguageId = model?.SelectedTarget.CodeName;
			translationFeedbackRequest.TargetMTText = feedbackInfo.OriginalMtCloudTranslation;

			dynamic feedbackRequest = new ExpandoObject();

			if (feedbackInfo.Evaluation?.UserChoseDifferently ?? false)
			{
				translationFeedbackRequest.QualityEstimationMt = new List<string> { feedbackInfo.Evaluation.OriginalEstimation };
				feedbackRequest.QualityEstimationMT = new List<string> { feedbackInfo.Evaluation.UserEstimation };
			}

			if (!string.IsNullOrWhiteSpace(feedbackInfo.Suggestion))
			{
				var improvementObject = new Improvement { Text = feedbackInfo.Suggestion };
				feedbackRequest.Improvement = improvementObject;
			}
			if (feedbackInfo.Rating is not null)
			{
				feedbackRequest.Rating = feedbackInfo.Rating;
			}
			feedbackRequest.Translation = translationFeedbackRequest;

			return feedbackRequest;
		}

		private LanguageMappingModel GetCorrespondingLanguageMappingModel()
		{
			var activeDocument = MtCloudApplicationInitializer.EditorController?.ActiveDocument;

			if (activeDocument is null) return null;

			var currentProject = activeDocument.Project.GetProjectInfo();

			var model = Options.LanguageMappings?.FirstOrDefault(l =>
				l.SourceTradosCode.Equals(currentProject.SourceLanguage.IsoAbbreviation,
					StringComparison.InvariantCultureIgnoreCase) &&
				l.TargetTradosCode.Equals(activeDocument.ActiveFile.Language.IsoAbbreviation,
					StringComparison.InvariantCultureIgnoreCase));
			return model;
		}

		private HttpRequestMessage GetRequestMessage(HttpMethod httpMethod, Uri uri)
		{
			var request = new HttpRequestMessage(httpMethod, uri);
			request.Headers.Add("Authorization", $"Bearer {ConnectionService.Credential.Token}");
			ConnectionService.AddTraceHeader(request);
			return request;
		}

		private async Task<HttpResponseMessage> GetTranslationResult(string id)
		{
			var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4" + $"/mt/translations/async/{id}/content");
			var request = GetRequestMessage(HttpMethod.Get, uri);
			var responseMessage = await _httpClient.SendRequest(request);
			return responseMessage.IsSuccessStatusCode ? responseMessage : null;
		}

		private void OnTranslationReceived(TranslationData translationData)
		{
			TranslationReceived?.Invoke(translationData);
		}

		private async Task<HttpResponseMessage> SendFeedback(dynamic translationFeedback)
		{
			CheckConnection();

			var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4/accounts/{ConnectionService.Credential.AccountId}/feedback/translations");

			var request = GetRequestMessage(HttpMethod.Post, uri);
			var serializerSettings =
				new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
			var content = JsonConvert.SerializeObject(translationFeedback, serializerSettings);

			request.Content = new StringContent(content, new UTF8Encoding(), "application/json");

			var response = await _httpClient.SendRequest(request);
			var responseAsString = await _httpClient.GetResponseAsString(response);

			_logger.Info(PluginResources.SendFeedbackResponseFromServer, response?.StatusCode, responseAsString);
			return response;
		}
	}
}