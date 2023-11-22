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
		private readonly IMtTranslationOptions _options;
		public static List<GoogleV3LanguageModel> SupportedLanguages { get; set; }
		public string GlossaryId { get; set; }

		public GoogleV3Connecter(IMtTranslationOptions options)
		{
			_options = options;
			SupportedLanguages = new List<GoogleV3LanguageModel>();

			//We put by default NMT model if the nmt model is not supported for the language pair 
			//Google knows to use basic model
			var model = "general/nmt";
			if (!string.IsNullOrEmpty(_options.GoogleEngineModel))
			{
				model = _options.GoogleEngineModel;
			}

			_modelPath = $"projects/{_options.ProjectName}/locations/{_options.ProjectLocation}/models/{model}";
			if (!string.IsNullOrEmpty(_options.GlossaryPath))
			{
				GlossaryId = Path.GetFileNameWithoutExtension(_options.GlossaryPath).Replace(" ",string.Empty); //Google doesn't allow spaces in glossary id
				_glossaryResourceLocation = $"projects/{_options.ProjectName}/locations/{_options.ProjectLocation}/glossaries/{GlossaryId}";
			}

			try
			{
				Environment.SetEnvironmentVariable(PluginResources.GoogleApiEnvironmentVariableName, _options.JsonFilePath);
				_translationServiceClient = TranslationServiceClient.Create();
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
			}
		}

		public string TranslateText(CultureInfo sourceLanguage, CultureInfo targetLanguage, string sourceText, string format)
		{
			try
			{
				// V2 uses "html" and "text" but for V3 its "text/html" and "text/plain"
				var mimeType = format == "text" ? "text/plain" : "text/html";
				var request = new TranslateTextRequest
				{
					Contents =
					{
						sourceText
					},
					Model = _modelPath,
					TargetLanguageCode = targetLanguage.TwoLetterISOLanguageName,
					SourceLanguageCode = sourceLanguage.TwoLetterISOLanguageName,
					Parent = new ProjectName(_options.ProjectName).ToString(),
					MimeType = mimeType
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
				if (!string.IsNullOrEmpty(_options.GlossaryPath))
				{
					Glossary glossary;
					if (_options.BasicCsv)
					{
						var sourceLang = languagePairs[0].SourceCultureName;
						var targetLang = languagePairs[0].TargetCultureName;
						glossary = CreateUnidirectionalCsvGlossary(sourceLang, targetLang);
					}
					else
					{
						var glossaryLanguages = GetGlossaryLanguages(languagePairs);
						glossary = CreatetCsvGlossary(glossaryLanguages);
					}

					var glossaryResponse = _translationServiceClient.CreateGlossary(
						new LocationName(_options.ProjectName, _options.ProjectLocation).ToString(), glossary);
				}
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
			}
		}

		private Glossary CreatetCsvGlossary(List<string> glossaryLanguages)
		{
			var glossary = new Glossary
			{
				Name = new GlossaryName(_options.ProjectName, _options.ProjectLocation, GlossaryId).ToString(),
				LanguageCodesSet = new Glossary.Types.LanguageCodesSet(),
				InputConfig = new GlossaryInputConfig
				{
					GcsSource = new GcsSource
					{
						InputUri = _options.GlossaryPath
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

		private Glossary CreateUnidirectionalCsvGlossary(string sourceLanguage,string targetLanguage)
		{
			var glossary = new Glossary
			{
				Name = new GlossaryName(_options.ProjectName, _options.ProjectLocation, GlossaryId).ToString(),
				LanguagePair = new Glossary.Types.LanguageCodePair
				{
					SourceLanguageCode = sourceLanguage,
					TargetLanguageCode = targetLanguage
				},
				InputConfig = new GlossaryInputConfig
				{
					GcsSource = new GcsSource
					{
						InputUri = _options.GlossaryPath
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
					ParentAsLocationName = new LocationName(_options.ProjectName, "global"),
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
				Parent = new ProjectName(_options.ProjectName).ToString()
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
