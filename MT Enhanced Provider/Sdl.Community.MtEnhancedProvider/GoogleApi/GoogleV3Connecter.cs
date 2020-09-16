using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using NLog;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MtEnhancedProvider.GoogleApi
{
	public class GoogleV3Connecter : IGoogleV3Connecter
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly TranslationServiceClient _translationServiceClient;
		private readonly string _modelPath;
		private readonly string _glossaryResourceLocation;

		public static List<GoogleV3LanguageModel> SupportedLanguages { get; set; }
		public string ProjectName { get; set; }
		public string JsonFilePath { get; set; }
		public string EngineModel { get; set; }
		public string Location { get; set; }
		public string GlossaryPath { get; set; }
		public string GlossaryId { get; set; }

		private IMtTranslationOptions _options; //TODO: Remove this for the final implementation

		public GoogleV3Connecter(IMtTranslationOptions options)
		{
			ProjectName = options.ProjectName;
			JsonFilePath = options.JsonFilePath;
			EngineModel = options.GoogleEngineModel;
			Location = options.ProjectLocation;
			GlossaryPath = options.GlossaryPath;
			_options = options;
			SupportedLanguages = new List<GoogleV3LanguageModel>();

			//We put by default NMT model if the nmt model is not supported for the language pair 
			//Google knows to use basic model
			var model = "general/nmt";
			if (!string.IsNullOrEmpty(EngineModel))
			{
				model = EngineModel;
			}

			_modelPath = $"projects/{ProjectName}/locations/{Location}/models/{model}";
			if (!string.IsNullOrEmpty(GlossaryPath))
			{
				_glossaryResourceLocation = $"projects/{ProjectName}/locations/{Location}/glossaries/{GlossaryPath}";
				GlossaryId = Path.GetFileNameWithoutExtension(GlossaryPath).Replace(" ",string.Empty); //Google doesn't allow spaces in glossary id
			}

			try
			{
				Environment.SetEnvironmentVariable(PluginResources.GoogleApiEnvironmentVariableName, JsonFilePath);
				_translationServiceClient = TranslationServiceClient.Create();
			}
			catch (Exception e)
			{

			}
		}

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
					Model = _modelPath,
					TargetLanguageCode = targetLanguage.Name,
					SourceLanguageCode = sourceLanguage.Name,
					Parent = new ProjectName(ProjectName).ToString()
				};
				if (!string.IsNullOrEmpty(_glossaryResourceLocation))
				{
					request.GlossaryConfig = new TranslateTextGlossaryConfig
					{
						Glossary = _glossaryResourceLocation,
						IgnoreCase = true
					};
				}
				var translationResponse = _translationServiceClient.TranslateText(request);
				//There are multiple translations only when we send a list of strings to be translated
				if (translationResponse != null)
				{
					if (!string.IsNullOrEmpty(_glossaryResourceLocation))
					{
						return translationResponse.GlossaryTranslations[0].TranslatedText;
					}
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

		public void CreateGoogleGlossary(LanguagePair[] languagePairs)
		{
			try
			{
				if (!string.IsNullOrEmpty(GlossaryPath))
				{
					//var glossaryLanguages = GetGlossaryLanguages(languagePairs);
					//var glossary = CreatetCsvGlossary(glossaryLanguages);
					var sourceLang = languagePairs[0].SourceCultureName;
					var targetLang = languagePairs[0].TargetCultureName;
					var glossary = CreateTmxGlossary(sourceLang, targetLang);

					var glossaryResponse = _translationServiceClient.CreateGlossary(
						new LocationName(ProjectName, Location).ToString(), glossary);
				}
			}
			catch (Exception e)
			{

			}
			finally
			{
				GetProjectGlossaries(_options);
			}
		}

		private Glossary CreatetCsvGlossary(List<string> glossaryLanguages)
		{
			var glossary = new Glossary
			{
				Name = new GlossaryName(ProjectName, Location, GlossaryId).ToString(),
				LanguageCodesSet = new Glossary.Types.LanguageCodesSet(),
				InputConfig = new GlossaryInputConfig
				{
					GcsSource = new GcsSource
					{
						InputUri = GlossaryPath,
					}
				}
			};
			glossary.LanguageCodesSet.LanguageCodes.AddRange(glossaryLanguages);
			return glossary;
		}

		private static List<string> GetGlossaryLanguages(LanguagePair[] languagePairs)
		{
			var glossaryLanguages = new List<string>
			{
				languagePairs[0].SourceCulture.TwoLetterISOLanguageName
			};
			foreach (var languagePair in languagePairs)
			{
				glossaryLanguages.Add(languagePair.TargetCulture.TwoLetterISOLanguageName);
			}
			return glossaryLanguages;
		}

		//This will be removed for the final version, we use it for testing
		public List<GoogleGlossary> GetProjectGlossaries(IMtTranslationOptions options)
		{
			var googleGlosaries = new List<GoogleGlossary>();
			var request = new ListGlossariesRequest
			{
				ParentAsLocationName = new LocationName(options.ProjectName, options.ProjectLocation),
				PageSize = 50,
			};
			try
			{
				var response = _translationServiceClient.ListGlossaries(request);
				var test = response.AsRawResponses();

				foreach (var glossaryResponse in test)
				{
					foreach (var glossary in glossaryResponse)
					{
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
				throw;
			}
			return googleGlosaries;
		}

		private Glossary CreateTmxGlossary(string sourceLanguage,string targetLanguage)
		{
			var glossary = new Glossary
			{
				Name = new GlossaryName(ProjectName, Location, GlossaryId).ToString(),
				LanguagePair = new Glossary.Types.LanguageCodePair
				{
					SourceLanguageCode = sourceLanguage,
					TargetLanguageCode = targetLanguage
				},
				InputConfig = new GlossaryInputConfig
				{
					GcsSource = new GcsSource
					{
						InputUri = GlossaryPath
					}
				}
			};

			return glossary;
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
			//if (!string.IsNullOrEmpty(EngineModel))
			//{
			//	request.Model = EngineModel;
			//}
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

			var searchedSource =
				SupportedLanguages?.FirstOrDefault(l => l.CultureInfo.Name.Equals(sourceLanguage.TwoLetterISOLanguageName));
			var searchedTarget =
				SupportedLanguages?.FirstOrDefault(l => l.CultureInfo.Name.Equals(targetLanguage.TwoLetterISOLanguageName));

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
