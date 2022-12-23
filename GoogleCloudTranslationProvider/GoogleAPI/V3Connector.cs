using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.AutoML.V1;
using Google.Cloud.Translate.V3;
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
		private RetrievedGlossary _glossary;

		public V3Connector(ITranslationOptions options, bool authenticateUser = false)
		{
			_options = options;
			ConnectToApi();
		}

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
			}
		}

		public List<Glossary> GetGlossaries(string location = null)
		{
			var locationName = LocationName.FromProjectLocation(_options.ProjectId, location ?? _options.ProjectLocation);
			var glossariesRequest = new ListGlossariesRequest { ParentAsLocationName = locationName };

			return _translationServiceClient.ListGlossaries(glossariesRequest).ToList();
		}

		public List<Model> GetCustomModels()
		{
			var request = new ListModelsRequest
			{
				ParentAsLocationName = new LocationName(_options.ProjectId, _options.ProjectLocation)
			};

			return AutoMlClient.Create().ListModels(request).ToList();
		}

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
				TargetLanguageCode = GetLanguageCode(targetLanguage),
				SourceLanguageCode = GetLanguageCode(sourceLanguage),
				Parent = new ProjectName(_options.ProjectId).ToString(),
				MimeType = format == "text" ? "text/plain" : "text/html",
				Model = SetCustomModel(sourceLanguage, targetLanguage),
				GlossaryConfig = SetGlossary(sourceLanguage, targetLanguage)
			};
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
			if (!_customModel.SourceLanguage.Equals(GetLanguageCode(sourceLanguage))
			 || !_customModel.TargetLanguage.Equals(GetLanguageCode(targetLanguage)))
			{
				_customModel = null;
				return defaultPath;
			}

			return _customModel.ModelPath;
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
			if (retrievedGlossary.SourceLanguage is not null
			 && retrievedGlossary.TargetLanguage is not null
			 && retrievedGlossary.SourceLanguage.Equals(sourceLanguage)
			 && retrievedGlossary.TargetLanguage.Equals(targetLanguage))
			{
				return null;
			}

			if (!retrievedGlossary.Languages.TryGetValue(GetLanguageCode(sourceLanguage), out _)
			 || !retrievedGlossary.Languages.TryGetValue(GetLanguageCode(targetLanguage), out _))
			{
				return null;
			}

			return new TranslateTextGlossaryConfig
			{
				Glossary = retrievedGlossary.GlossaryResourceLocation,
				IgnoreCase = true
			};
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

		private string GetLanguageCode(CultureInfo cultureInfo)
		{
			if (cultureInfo.Name == "fr-HT") { return "ht"; }
			if (cultureInfo.Name == "zh-TW" || cultureInfo.Name == "zh-CN") { return cultureInfo.Name; } //just get the name for zh-TW which Google can process..google can also process simplified when specifying target as zh-CN but it breaks when you specify that as source??
			if (cultureInfo.Name.Equals("nb-NO") || cultureInfo.Name.Equals("nn-NO")) return "no";
			if (cultureInfo.TwoLetterISOLanguageName.Equals("sr") && cultureInfo.DisplayName.ToLower().Contains("latin")) return "sr-Latn";

			if (cultureInfo.DisplayName == "Samoan") { return "sm"; }

			var isoLanguageName = cultureInfo.TwoLetterISOLanguageName; //if not chinese trad or norweigian get 2 letter code
																		//convert tagalog and hebrew for Google
			if (isoLanguageName == "fil") { isoLanguageName = "tl"; }
			if (isoLanguageName == "he") { isoLanguageName = "iw"; }

			return isoLanguageName;
		}
	}
}