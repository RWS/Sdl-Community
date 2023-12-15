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
		public static List<LanguageMapping> GetGoogleDefaultMapping(ITranslationOptions translationOptions)
		{
			var databaseFilePath = GetDatabaseFilePath(translationOptions.SelectedGoogleVersion);
			if (!File.Exists(databaseFilePath))
			{
				return null;
			}

			return translationOptions.SelectedGoogleVersion switch
			{
				ApiVersion.V2 => CreateDatabase(translationOptions, CreateV2Database),
				ApiVersion.V3 => CreateDatabase(translationOptions, CreateV3Database),
				_ => null
			};
		}

		public static void CreateDatabase(ITranslationOptions translationOptions)
		{
			var languageMappings = CreateLanguageMappings(translationOptions);
			languageMappings = languageMappings.OrderBy(x => x.Name).ThenBy(x => x.Region).ToList();
			var database = translationOptions.SelectedGoogleVersion == ApiVersion.V2
						 ? PluginResources.Database_PluginName_V2
						 : PluginResources.Database_PluginName_V3;

			_ = new LanguageMappingDatabase(database, languageMappings);
		}

		private static List<LanguageMapping> CreateDatabase(ITranslationOptions translationOptions, Func<ITranslationOptions, List<LanguageMapping>> createDatabaseFunc)
		{
			var languageMappings = createDatabaseFunc(translationOptions);
			return languageMappings;
		}

		private static List<LanguageMapping> CreateV2Database(ITranslationOptions translationOptions)
		{
			var v2Languages = translationOptions.V2SupportedLanguages;

			return v2Languages
				.Where(language => IsValidLanguage(translationOptions.SelectedGoogleVersion, language))
				.Select(language => ParseLanguageMapping(language.LanguageName, language.LanguageCode))
				.Union(CreateChineseMapping())
				.ToList();
		}

		private static List<LanguageMapping> CreateV3Database(ITranslationOptions translationOptions)
		{
			if (translationOptions.V3SupportedLanguages is null || !translationOptions.V3SupportedLanguages.Any())
			{
				var v3Connector = new V3Connector(translationOptions);
				translationOptions.V3SupportedLanguages = v3Connector.GetLanguages();
			}

			var v3Languages = translationOptions.V3SupportedLanguages;
			return v3Languages
				.Where(language => IsValidLanguage(translationOptions.SelectedGoogleVersion, language))
				.Select(language => new LanguageMapping
				{
					Name = language.CultureInfo.DisplayName,
					LanguageCode = language.GoogleLanguageCode
				})
				.Union(CreateChineseMapping())
				.ToList();
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

				var languageCode = languageRegion.StartsWith("Simplified") ? $"zh-CN" : languageRegion.StartsWith("Traditional") ? "zh-TW" : "zh";
				chineseLanguageMapping.Add(new LanguageMapping
				{
					Name = languageName,
					Region = languageRegion,
					LanguageCode = languageCode
				});
			}

			return chineseLanguageMapping;
		}

		private static List<LanguageMapping> CreateLanguageMappings(ITranslationOptions translationOptions)
		{

			return translationOptions.SelectedGoogleVersion switch
			{
				ApiVersion.V2 => CreateV2Database(translationOptions),
				ApiVersion.V3 => CreateV3Database(translationOptions),
				_ => new List<LanguageMapping>()
			};
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

		private static LanguageMapping ParseLanguageMapping(string languageName, string languageCode)
		{
			var regex = new Regex(@"^(.*?)\s*(?:\((.*?)\))?$");
			var match = regex.Match(languageName);

			return new LanguageMapping
			{
				Name = match.Groups[1].Value,
				Region = match.Groups[2].Success ? match.Groups[2].Value : null,
				LanguageCode = languageCode
			};
		}

		private static string GetDatabaseFilePath(ApiVersion apiVersion)
		{
			var pluginName = apiVersion == ApiVersion.V2
				? PluginResources.Database_PluginName_V2
				: PluginResources.Database_PluginName_V3;
			return string.Format(Constants.DatabaseFilePath, pluginName);
		}
	}
}