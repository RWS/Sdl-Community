using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

		private readonly TranslationServiceClient _translationServiceClient;
		public  static List<GoogleV3LanguageModel> SupportedLanguages { get; set; }
		public string ProjectName { get; set; }
		public string JsonFilePath { get; set; }

		public string TranslateText(CultureInfo sourceLanguage, CultureInfo targetLanguage, string sorceText)
		{
			try
			{
				var request = new TranslateTextRequest
				{
					Contents =
					{
						sorceText
					},
					TargetLanguageCode = targetLanguage.Name,
					SourceLanguageCode = sourceLanguage.Name,
					Parent = new ProjectName(ProjectName).ToString()

				};
				var translationResponse = _translationServiceClient.TranslateText(request);
				//There are multiple translations only when we send a list of strings to be translated
				if (translationResponse != null)
				{
					return translationResponse.Translations?[0].TranslatedText;
				}
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
				throw;
			}
			
			return string.Empty;
		}

		public GoogleV3Connecter(string projectName,string jsonFilePath)
		{
			ProjectName = projectName;
			JsonFilePath = jsonFilePath;
			SupportedLanguages = new List<GoogleV3LanguageModel>();

			Environment.SetEnvironmentVariable(PluginResources.GoogleApiEnvironmentVariableName, jsonFilePath);
			_translationServiceClient = TranslationServiceClient.Create();
		}

		public void SetGoogleAvailableLanguages()
		{
			try
			{
				var request = new GetSupportedLanguagesRequest
				{
					ParentAsLocationName = new LocationName(ProjectName, "global"),
				};
				var response = _translationServiceClient.GetSupportedLanguages(request);

				foreach (var language in response.Languages)
				{
					var languageModel = new GoogleV3LanguageModel
					{
						GoogleLanguageCode = language.LanguageCode,
						SupportSource = language.SupportSource,
						SupportTarget = language.SupportTarget,
						CultureInfo = new CultureInfo(language.LanguageCode)
					};
					SupportedLanguages.Add(languageModel);
				}
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
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
				Parent = new ProjectName(ProjectName).ToString()

			};
			_translationServiceClient.TranslateText(request);
		}

		public bool IsSupportedLanguage(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			if (!SupportedLanguages.Any())
			{
				SetGoogleAvailableLanguages();
			}

			var suportsSource = false;
			var supportsTarget = false;

			var searchedSource = SupportedLanguages?.FirstOrDefault(l => l.CultureInfo.Name.Equals(sourceLanguage.TwoLetterISOLanguageName));
			var searchedTarget = SupportedLanguages?.FirstOrDefault(l => l.CultureInfo.Name.Equals(targetLanguage.TwoLetterISOLanguageName));

			if (searchedSource != null)
			{
				suportsSource = searchedSource.SupportSource;
			}
			if (searchedTarget != null)
			{
				supportsTarget = searchedTarget.SupportTarget;
			}
			return suportsSource && supportsTarget;
		}


	}
}
