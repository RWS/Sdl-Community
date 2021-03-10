using System;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.Events;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
using Converter = Sdl.Community.MTCloud.Provider.XliffConverter.Converter.Converter;
using LogManager = NLog.LogManager;

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

			if (httpResponseMessage != null)
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

		public async Task<HttpResponseMessage> SendFeedback(SegmentId? segmentId, dynamic rating, string originalText, string improvement)
		{
			var feedbackRequest = CreateFeedbackRequest(segmentId, rating, originalText, improvement);
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

			var (responseMessage, qualityEstimation) = await CheckTranslationStatus(translationResponse?.RequestId);
			var translations = await _httpClient.GetResult<TranslationResponse>(responseMessage);

			var translation = translations?.Translation?.FirstOrDefault();
			if (translation == null) return null;

			var translatedXliff = Converter.ParseXliffString(translation);
			if (translatedXliff == null) return null;

			var targetSegments = translatedXliff.GetTargetSegments(out var sourceSegments);

			OnTranslationReceived(new TranslationData
			{
				SourceSegments = sourceSegments,
				TargetSegments = targetSegments.Select(seg => seg.ToString()).ToList(),
				TranslationOriginInformation = new TranslationOriginInformation
				{
					Model = translations.Model,
					QualityEstimation = qualityEstimation
				},
				FilePath = fileAndSegments.FilePath,
				SegmentIds = fileAndSegments.SegmentIds,
				TargetLanguage = model.TargetTradosCode
			});

			return targetSegments;
		}

		private static string GetQualityEstimation(TranslationResponseStatus responseStatus)
		{
			return responseStatus.QualityEstimation?[0];
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

		private async Task<(HttpResponseMessage, string)> CheckTranslationStatus(string id)
		{
			var translationStatus = string.Empty;
			string qualityEstimation = null;

			do
			{
				var uri = new Uri($"{Constants.MTCloudTranslateAPIUri}/v4/mt/translations/async/{id}");
				var request = GetRequestMessage(HttpMethod.Get, uri);

				var responseMessage = await _httpClient.SendRequest(request);
				var responseStatus = await _httpClient.GetResult<TranslationResponseStatus>(responseMessage);

				if (responseStatus is null) continue;

				WaitForTranslation(responseStatus);

				translationStatus = responseStatus.TranslationStatus;
				qualityEstimation = GetQualityEstimation(responseStatus);

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

			return (await GetTranslationResult(id), qualityEstimation);
		}

		private dynamic CreateFeedbackRequest(SegmentId? segmentId, dynamic rating, string originalText, string improvement)
		{
			var activeDocument = MtCloudApplicationInitializer.EditorController.ActiveDocument;

			var segmentSource = segmentId != null
				? activeDocument.SegmentPairs.ToList().FirstOrDefault(sp => sp.Properties.Id.Equals(segmentId))?.Source.ToString()
				: activeDocument.ActiveSegmentPair.Source.ToString();

			var model = GetCorrespondingLanguageMappingModel();
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

		private LanguageMappingModel GetCorrespondingLanguageMappingModel()
		{
			var activeDocument = MtCloudApplicationInitializer.EditorController.ActiveDocument;
			var currentProject = MtCloudApplicationInitializer.EditorController.ActiveDocument.Project.GetProjectInfo();

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