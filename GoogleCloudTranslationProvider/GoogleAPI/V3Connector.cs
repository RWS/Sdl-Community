using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.AutoML.V1;
using Google.Cloud.Translate.V3;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using NLog;
using Sdl.LanguagePlatform.Core;

namespace GoogleCloudTranslationProvider.GoogleAPI
{
	public class V3Connector
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly ITranslationOptions _options;

		private TranslationServiceClient _translationServiceClient;
		private List<V3LanguageModel> _supportedLanguages;
		private RetrievedCustomModel _customModel;

		public V3Connector(ITranslationOptions options, bool authenticateUser = false)
		{
			_options = options;
			ConnectToApi();
		}

		#region Connection
		private void ConnectToApi()
		{
			try
			{
				Environment.SetEnvironmentVariable(Constants.GoogleApiEnvironmentVariableName, _options.JsonFilePath);
				_translationServiceClient = TranslationServiceClient.Create();
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
				ErrorHandler.HandleError(e);
			}
		}

		public void TryToAuthenticateUser(LanguagePair[] languagePair = null)
		{
			languagePair ??= new LanguagePair[]
				{
					new LanguagePair("en-US", "fr-FR")
				};

			foreach (var pair in languagePair)
			{
				TranslateText(pair.SourceCulture, pair.TargetCulture, "test", "text");
			}
		}
		#endregion

		#region Languages
		public bool IsSupportedLanguage(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			_supportedLanguages ??= new();
			if (!_supportedLanguages.Any())
			{
				SetGoogleAvailableLanguages();
			}

			var searchedSource = _supportedLanguages.FirstOrDefault(x => x.CultureInfo.Name.Equals(sourceLanguage.TwoLetterISOLanguageName));
			var searchedTarget = _supportedLanguages.FirstOrDefault(x => x.CultureInfo.Name.Equals(targetLanguage.TwoLetterISOLanguageName));

			return searchedSource.SupportSource && searchedTarget.SupportTarget;
		}

		private void SetGoogleAvailableLanguages()
		{
			try
			{
				TrySetGoogleAvailableLanguages();
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
			}
		}

		private void TrySetGoogleAvailableLanguages()
		{
			_supportedLanguages ??= new();
			var locationName = new LocationName(_options.ProjectId, "global");
			var request = new GetSupportedLanguagesRequest { ParentAsLocationName = locationName };
			var response = _translationServiceClient.GetSupportedLanguages(request);

			_supportedLanguages.AddRange(response.Languages.Select(language => new V3LanguageModel
			{
				GoogleLanguageCode = language.LanguageCode,
				SupportSource = language.SupportSource,
				SupportTarget = language.SupportTarget,
				CultureInfo = new CultureInfo(language.LanguageCode)
			}));
		}
		#endregion

		#region Translation
		public string TranslateText(CultureInfo sourceLanguage, CultureInfo targetLanguage, string sourceText, string format)
		{
			try
			{
				return TryTranslateText(sourceLanguage, targetLanguage, sourceText, format);
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
				throw e;
			}
		}

		private string TryTranslateText(CultureInfo sourceLanguage, CultureInfo targetLanguage, string sourceText, string format)
		{
			var request = CreateTranslationRequest(sourceLanguage, targetLanguage, sourceText, format);
			if (_translationServiceClient.TranslateText(request) is not TranslateTextResponse translationResponse)
			{
				return string.Empty;
			}

			return request.GlossaryConfig is null ? translationResponse.Translations[0].TranslatedText
												  : translationResponse.GlossaryTranslations[0].TranslatedText;
		}

		private TranslateTextRequest CreateTranslationRequest(CultureInfo sourceLanguage, CultureInfo targetLanguage, string sourceText, string format)
		{
			return new TranslateTextRequest
			{
				Contents = { sourceText },
				SourceLanguageCode = sourceLanguage.ConvertLanguageCode(),
				TargetLanguageCode =  targetLanguage.ConvertLanguageCode(),
				ParentAsLocationName = new LocationName(_options.ProjectId, _options.ProjectLocation),
				MimeType = format == "text" ? "text/plain" : "text/html",
				Model = SetCustomModel(sourceLanguage, targetLanguage),
				GlossaryConfig = SetGlossary(sourceLanguage, targetLanguage)
			};
		}
		#endregion

		#region Glossaries
		public List<Glossary> GetGlossaries(string location = null)
		{
			var locationName = LocationName.FromProjectLocation(_options.ProjectId, location ?? _options.ProjectLocation);
			var glossariesRequest = new ListGlossariesRequest { ParentAsLocationName = locationName };

			return _translationServiceClient.ListGlossaries(glossariesRequest).ToList();
		}

		private TranslateTextGlossaryConfig SetGlossary(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			if (string.IsNullOrEmpty(_options.GlossaryPath))
			{
				return null;
			}

			var glossaryFound = GetGlossaries().FirstOrDefault(x => x.GlossaryName.GlossaryId.Equals(_options.GlossaryPath));
			if (glossaryFound is null)
			{
				return null;
			}

			var retrievedGlossary = new RetrievedGlossary(glossaryFound, _options.ProjectId, _options.ProjectLocation);
			if (retrievedGlossary.Languages is not null
			 && retrievedGlossary.Languages.Contains(sourceLanguage.TwoLetterISOLanguageName)
			 && retrievedGlossary.Languages.Contains(targetLanguage.TwoLetterISOLanguageName))
			{
				return new TranslateTextGlossaryConfig
				{
					Glossary = retrievedGlossary.GlossaryResourceLocation,
					IgnoreCase = true
				};
			}

			if (retrievedGlossary.SourceLanguage is not null
			 && retrievedGlossary.TargetLanguage is not null
			 && retrievedGlossary.SourceLanguage.TwoLetterISOLanguageName.Equals(sourceLanguage.TwoLetterISOLanguageName)
			 && retrievedGlossary.TargetLanguage.TwoLetterISOLanguageName.Equals(targetLanguage.TwoLetterISOLanguageName))
			{
				return new TranslateTextGlossaryConfig
				{
					Glossary = retrievedGlossary.GlossaryResourceLocation,
					IgnoreCase = true
				};
			}

			return null;
		}
		#endregion

		#region AutoML
		public List<Model> GetCustomModels()
		{
			var request = new ListModelsRequest
			{
				ParentAsLocationName = new LocationName(_options.ProjectId, _options.ProjectLocation)
			};

			return AutoMlClient.Create().ListModels(request).ToList();
		}

		private string SetCustomModel(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			var defaultPath = $"projects/{_options.ProjectId}/locations/{_options.ProjectLocation}/models/general/nmt";
			if (string.IsNullOrEmpty(_options.GoogleEngineModel))
			{
				return defaultPath;
			}

			var customModelFound = GetCustomModels().FirstOrDefault(x => x.DatasetId == _options.GoogleEngineModel);
			if (customModelFound is null)
			{
				return defaultPath;
			}

			_customModel = new(customModelFound);
			if (!_customModel.SourceLanguage.Equals(sourceLanguage.ConvertLanguageCode())
			 || !_customModel.TargetLanguage.Equals(targetLanguage.ConvertLanguageCode()))
			{
				_customModel = null;
				return defaultPath;
			}

			return _customModel.ModelPath;
		}
		#endregion
	}
}