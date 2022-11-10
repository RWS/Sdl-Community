using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
		private readonly TranslationServiceClient _translationServiceClient;
		private readonly List<V3LanguageModel> _supportedLanguages;
		private readonly string _glossaryResourceLocation;
		private readonly string _modelPath;

		private string _glossaryID;

		public V3Connector(ITranslationOptions options)
		{
			_supportedLanguages = new List<V3LanguageModel>();
			_options = options;
			_modelPath = SetModelPath();
			_glossaryResourceLocation = SetGlossary();

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

		public string TranslateText(CultureInfo sourceLanguage, CultureInfo targetLanguage, string sourceText, string format)
		{
			try
			{
				return TryTranslateText(sourceLanguage, targetLanguage, sourceText, format);
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
				throw;
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
		
		public void CreateGoogleGlossary(LanguagePair[] languagePairs)
		{
			if (string.IsNullOrEmpty(_options.GlossaryPath))
			{
				return;
			}

			try
			{
				TryCreateGoogleGlossary(languagePairs);
			}
			catch (Exception e)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
			}
		}

		private string TryTranslateText(CultureInfo sourceLanguage, CultureInfo targetLanguage, string sourceText, string format)
		{
			var request = new TranslateTextRequest
			{
				Contents = { sourceText },
				Model = _modelPath,
				TargetLanguageCode = targetLanguage.TwoLetterISOLanguageName,
				SourceLanguageCode = sourceLanguage.TwoLetterISOLanguageName,
				Parent = new ProjectName(_options.ProjectName).ToString(),
				MimeType = format == "text" ? "text/plain" : "text/html"
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
			return translationResponse is null
				? string.Empty
				: string.IsNullOrEmpty(_glossaryResourceLocation) ? translationResponse.Translations?[0].TranslatedText
																  : translationResponse.GlossaryTranslations[0].TranslatedText;
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
			var locationName = new LocationName(_options.ProjectName, "global");
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

		private void TryCreateGoogleGlossary(LanguagePair[] languagePairs)
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

			_ = _translationServiceClient.CreateGlossary(new LocationName(_options.ProjectName, _options.ProjectLocation).ToString(),
														 glossary);
		}

		private Glossary CreatetCsvGlossary(List<string> glossaryLanguages)
		{
			var glossary = new Glossary
			{
				Name = new GlossaryName(_options.ProjectName, _options.ProjectLocation, _glossaryID).ToString(),
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

		private Glossary CreateUnidirectionalCsvGlossary(string sourceLanguage, string targetLanguage)
		{
			return new Glossary
			{
				Name = new GlossaryName(_options.ProjectName, _options.ProjectLocation, _glossaryID).ToString(),
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
		}

		private string SetModelPath()
		{
			const string defaultModel = "general/nmt";
			return string.Format(
				"projects/{0}/locations/{1}/models/{2}",
				_options.ProjectName,
				_options.ProjectLocation,
				string.IsNullOrEmpty(_options.GoogleEngineModel) ? defaultModel : _options.GoogleEngineModel);
		}

		private string SetGlossary()
		{
			if (string.IsNullOrEmpty(_options.GlossaryPath))
			{
				return null;
			}

			_glossaryID = Path.GetFileNameWithoutExtension(_options.GlossaryPath).Replace(" ", string.Empty);
			return string.Format(
				"projects/{0}/locations/{1}/glossaries/{2}",
				_options.ProjectName,
				_options.ProjectLocation,
				_glossaryID);
		}
	}
}