using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using NLog;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.GoogleApi
{
	public class GoogleV3Connecter: IGoogleV3Connecter
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly string _jsonFilePath;
		private readonly string _projectName;
		private readonly TranslationServiceClient _translationServiceClient;
		private static List<GoogleV3LanguageModel> _supportedLanguages;


		public GoogleV3Connecter(string projectName,string jsonFilePath)
		{
			_projectName = projectName;
			_jsonFilePath = jsonFilePath;
			_supportedLanguages = new List<GoogleV3LanguageModel>();

			Environment.SetEnvironmentVariable(PluginResources.GoogleApiEnvironmentVariableName, jsonFilePath);
			_translationServiceClient = TranslationServiceClient.Create();
		}

		public void GetAvailableLanguages()
		{
			var request = new GetSupportedLanguagesRequest
			{
				ParentAsLocationName = new LocationName(_projectName, "global"),
			};
			var response = _translationServiceClient.GetSupportedLanguages(request);

			foreach (var language in response.Languages)
			{
				var languageModel = new GoogleV3LanguageModel
				{
					LanguageCode = language.LanguageCode,
					SupportSource = language.SupportSource,
					SupportTarget = language.SupportTarget
				};
				_supportedLanguages.Add(languageModel);
			}
		}

		public void TryToAuthenticateUser()
		{
			var request = new TranslateTextRequest
			{
				Contents =
				{
					"test"
				},
				TargetLanguageCode = "fr-FR",
				Parent = new ProjectName(_projectName).ToString()

			};
			_translationServiceClient.TranslateText(request);
		}
	}
}
