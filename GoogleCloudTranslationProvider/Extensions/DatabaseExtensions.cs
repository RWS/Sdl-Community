using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GoogleCloudTranslationProvider.GoogleAPI;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using LanguageMappingProvider.Database;
using LanguageMappingProvider.Model;
using Sdl.Core.Globalization.LanguageRegistry;

namespace GoogleCloudTranslationProvider.Extensions
{
	public static class DatabaseExtensions
	{
		public static async Task<List<LanguageMapping>> GetGoogleDefaultMapping(ITranslationOptions translationOptions)
		{
			if (translationOptions.SelectedGoogleVersion == ApiVersion.V2)
			{
				if (!File.Exists(string.Format(Constants.DatabaseFilePath, PluginResources.Database_PluginName_V2)))
				{
					return null;
				}

				return CreateV2Database(translationOptions).Result;
			}
			else if (translationOptions.SelectedGoogleVersion == ApiVersion.V3)
			{
				if (!File.Exists(string.Format(Constants.DatabaseFilePath, PluginResources.Database_PluginName_V3)))
				{
					return null;
				}

				return CreateV3Database(translationOptions);
			}

			return null;
		}

		public static void CreateDatabase(ITranslationOptions translationOptions)
		{
			var languageMappings = new List<LanguageMapping>();
			if (translationOptions.SelectedGoogleVersion == ApiVersion.V2)
			{
				if (File.Exists(string.Format(Constants.DatabaseFilePath, PluginResources.Database_PluginName_V2)))
				{
					return;
				}

				languageMappings = CreateV2Database(translationOptions).Result;
			}
			else if (translationOptions.SelectedGoogleVersion == ApiVersion.V3)
			{
				if (File.Exists(string.Format(Constants.DatabaseFilePath, PluginResources.Database_PluginName_V3)))
				{
					return;
				}

				languageMappings = CreateV3Database(translationOptions);
			}

			languageMappings = languageMappings.OrderBy(x => x.Name).ThenBy(x => x.Region).ToList();
			var database = translationOptions.SelectedGoogleVersion == ApiVersion.V2
						 ? PluginResources.Database_PluginName_V2
						 : PluginResources.Database_PluginName_V3;

			_ = new LanguageMappingDatabase(database, languageMappings);
		}

		private static async Task<List<LanguageMapping>> CreateV2Database(ITranslationOptions translationOptions)
		{
			var v2Connector = new V2Connector(translationOptions.ApiKey, null);
			var v2Languages = v2Connector.GetLanguages().Result;

			var languageMapping = new List<LanguageMapping>();
			foreach (var language in v2Languages)
			{
				if (!IsValidLanguage(translationOptions.SelectedGoogleVersion, language))
				{
					continue;
				}

				var regex = new Regex(@"^(.*?)\s*(?:\((.*?)\))?$");
				var match = regex.Match(language.LanguageName);

				languageMapping.Add(new LanguageMapping
				{
					Name = match.Groups[1].Value,
					Region = match.Groups[2].Success ? match.Groups[2].Value : null,
					LanguageCode = language.LanguageCode
				});
			}

			return languageMapping.Union(CreateChineseMapping()).ToList();
		}

		private static List<LanguageMapping> CreateV3Database(ITranslationOptions translationOptions)
		{
			var v3Connector = new V3Connector(translationOptions);
			var v3Languages = v3Connector.GetLanguages();

			var languageMapping = new List<LanguageMapping>();
			foreach (var language in v3Languages)
			{
				if (!IsValidLanguage(translationOptions.SelectedGoogleVersion, language))
				{
					continue;
				}

				languageMapping.Add(new()
				{
					Name = language.CultureInfo.DisplayName,
					LanguageCode = language.GoogleLanguageCode
				});
			}

			return languageMapping.Union(CreateChineseMapping()).ToList();
		}

		private static List<LanguageMapping> CreateChineseMapping()
		{
			var tradosChinese = LanguageRegistryApi.Instance.GetAllLanguages().Where(x => x.EnglishName.StartsWith("Chinese")).ToList();
			var chineseLanguageMapping = new List<LanguageMapping>();
			foreach (var language in tradosChinese)
			{
				var regex = new Regex(@"^(.*?)\s*(?:\((.*?)\))?$");
				var match = regex.Match(language.DisplayName);

				var languageName = match.Groups[1].Value;
				var languageRegion = match.Groups[2].Success ? match.Groups[2].Value : null;

				if (chineseLanguageMapping.Any(x => x.Name == languageName && x.Region == languageRegion)
				 || languageRegion is null)
				{
					continue;
				}

				chineseLanguageMapping.Add(new LanguageMapping
				{
					Name = languageName,
					Region = languageRegion,
					LanguageCode = $"zh-{language.Script}"
				});
			}

			return chineseLanguageMapping;
		}

		private static bool IsValidLanguage(ApiVersion apiVersion, object targetLanguage)
		{
			// Some languages are duplicates, which may cause issues when creating the database.
			// Ignore them or handle them as a special case if they at least have different regions.

			// Unknown languages are not added to the database.
			// Only be available when retrieving the V3 supported languages and creating a CultureInfo object using their code,
			// for example: new CultureInfo("xx-XX").

			// Chinese languages will be handled differently due to their language codes and the presence
			// of both traditional and simplified variations.
			// Here we can also find some duplicates.

			return apiVersion switch
			{
				ApiVersion.V2 when targetLanguage is V2LanguageModel v2Language =>
				  !(v2Language.LanguageCode == "zh"
				 || v2Language.LanguageCode == "iw"
				 || v2Language.LanguageCode == "jw"
				 || v2Language.LanguageName.StartsWith("Chinese")),
				ApiVersion.V3 when targetLanguage is V3LanguageModel v3Language =>
				  !(v3Language.CultureInfo.DisplayName.StartsWith("Unknown")
				 || v3Language.CultureInfo.DisplayName.StartsWith("Chinese")
				 || v3Language.GoogleLanguageCode == "ckb"),
				_ => false
			};
		}
	}
}