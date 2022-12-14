using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Models;
using NLog;
using Sdl.LanguagePlatform.Core;

namespace GoogleTranslatorProvider.GoogleAPI
{
	public class V3Connector
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly ITranslationOptions _options;
		private readonly List<V3LanguageModel> _supportedLanguages;
		private readonly string _modelPath;

		private RetrievedGlossary _glossary;
		private TranslationServiceClient _translationServiceClient;

		public V3Connector(ITranslationOptions options)
		{
			_supportedLanguages = new List<V3LanguageModel>();
			_options = options;
			ConnectToApi();
			_modelPath = SetModelPath();
			SetGlossary();
		}

		public void ConnectToApi()
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

		public bool IsSupportedLanguage(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			if (!_supportedLanguages.Any())
			{
				SetGoogleAvailableLanguages();
			}

			var searchedSource = _supportedLanguages.FirstOrDefault(x => x.CultureInfo.Name.Equals(sourceLanguage.TwoLetterISOLanguageName));
			var searchedTarget = _supportedLanguages.FirstOrDefault(x => x.CultureInfo.Name.Equals(targetLanguage.TwoLetterISOLanguageName));

			return searchedSource.SupportSource && searchedTarget.SupportTarget;
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
			var translateTextRequest = new TranslateTextRequest
			{
				Contents = { sourceText },
				Model = _modelPath,
				TargetLanguageCode = GetLanguageCode(targetLanguage),
				SourceLanguageCode = GetLanguageCode(sourceLanguage),
				Parent = new ProjectName(_options.ProjectId).ToString(),
				MimeType = format == "text" ? "text/plain" : "text/html"
			};

			if (CanUseGlossary(sourceLanguage, targetLanguage))
			{
				translateTextRequest.GlossaryConfig = new TranslateTextGlossaryConfig
				{
					Glossary = _glossary.GlossaryResourceLocation,
					IgnoreCase = true
				};
			}

			return translateTextRequest;
		}

		private bool CanUseGlossary(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			if (_glossary?.GlossaryID is null)
			{
				return false;
			}

			if (_glossary.SourceLanguage is not null
			 && _glossary.TargetLanguage is not null
			 && _glossary.SourceLanguage.Equals(sourceLanguage)
			 && _glossary.TargetLanguage.Equals(targetLanguage))
			{
				return true;
			}

			return _glossary.Languages.Intersect(new List<CultureInfo>() { sourceLanguage, targetLanguage }).Count() == 2;
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

		private string SetModelPath()
		{
			const string defaultModel = "general/nmt";
			return string.Format(
				"projects/{0}/locations/{1}/models/{2}",
				_options.ProjectId,
				_options.ProjectLocation,
				string.IsNullOrEmpty(_options.GoogleEngineModel) ? defaultModel : _options.GoogleEngineModel);
		}

		private void SetGlossary()
		{
			if (string.IsNullOrEmpty(_options.GlossaryPath))
			{
				return;
			}

			var glossaryFound = GetGlossaries().FirstOrDefault(x => x.GlossaryName.GlossaryId.Equals(_options.GlossaryPath));
			_glossary = new(glossaryFound, _options.ProjectId, _options.ProjectLocation);
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