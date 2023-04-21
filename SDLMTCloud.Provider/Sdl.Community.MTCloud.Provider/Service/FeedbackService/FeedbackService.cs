using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using Sdl.Community.MTCloud.Provider.Extensions;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Model.RateIt;
using Sdl.Community.MTCloud.Provider.Service.FeedbackService.Model;
using Sdl.Community.MTCloud.Provider.Service.Interface;

namespace Sdl.Community.MTCloud.Provider.Service.FeedbackService
{
	public class FeedbackService : IFeedbackService
	{
		public FeedbackService(FeedbackSettings settings, List<LanguageMappingModel> models, IConnectionService connectionService)
		{
			Settings = settings;
			Models = models;
			ConnectionService = connectionService;
		}

		public bool IsActiveModelQeEnabled => Models.IsActiveModelQeEnabled();
		public FeedbackSettings Settings { get; }
		private IConnectionService ConnectionService { get; }
		private ILogger Logger { get; } = LogManager.GetCurrentClassLogger();
		private List<LanguageMappingModel> Models { get; }

		public async Task<HttpResponseMessage> SendFeedback(FeedbackInfo feedbackInfo)
		{
			var feedbackRequest = CreateFeedbackRequest(feedbackInfo);
			return await SendFeedback(feedbackRequest);
		}

		private dynamic CreateFeedbackRequest(FeedbackInfo feedbackInfo)
		{
			var model = Models.GetCurrentLanguageMappingModel();
			dynamic translationFeedbackRequest = new ExpandoObject();
			translationFeedbackRequest.Model = model?.SelectedModel.Model;
			translationFeedbackRequest.SourceLanguageId = model?.SelectedSource.CodeName;
			translationFeedbackRequest.SourceText = feedbackInfo.SegmentSource;
			translationFeedbackRequest.TargetLanguageId = model?.SelectedTarget.CodeName;
			translationFeedbackRequest.TargetMTText = feedbackInfo.OriginalMtCloudTranslation;

			dynamic feedbackRequest = new ExpandoObject();

			if (feedbackInfo.Evaluation?.UserChoseDifferently ?? false)
			{
				translationFeedbackRequest.QualityEstimationMT = feedbackInfo.Evaluation.OriginalEstimation.ToUpper();
				feedbackRequest.QualityEstimation = feedbackInfo.Evaluation.UserEstimation.ToUpper();
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

		private async Task<HttpResponseMessage> SendFeedback(dynamic translationFeedback)
		{
			ConnectionService.CheckConnection();

			var uri = new Uri($"{ConnectionService.CurrentWorkingPortalAddress}/v4/accounts/{ConnectionService.Credential.AccountId}/feedback/translations");

			var request = ConnectionService.GetRequestMessage(HttpMethod.Post, uri);
			var serializerSettings =
				new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
			var content = JsonConvert.SerializeObject(translationFeedback, serializerSettings);

			request.Content = new StringContent(content, new UTF8Encoding(), "application/json");

			var response = await MtCloudApplicationInitializer.Client.SendRequest(request);
			var responseAsString = await MtCloudApplicationInitializer.Client.GetResponseAsString(response);

			Logger.Info(PluginResources.SendFeedbackResponseFromServer, response?.StatusCode, responseAsString);
			return response;
		}
	}
}