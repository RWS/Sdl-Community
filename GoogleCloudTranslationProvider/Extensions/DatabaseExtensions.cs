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

namespace GoogleCloudTranslationProvider.Extensions
{
	public static class DatabaseExtensions
	{
		public static async Task<List<LanguageMapping>> GetGoogleDefaultMapping(ITranslationOptions translationOptions)
		{
			var databaseFilePath = GetDatabaseFilePath(translationOptions.SelectedGoogleVersion);
			if (!File.Exists(databaseFilePath))
			{
				return null;
			}

			return translationOptions.SelectedGoogleVersion switch
			{
				ApiVersion.V2 => await CreateDatabase(translationOptions, CreateV2Database),
				ApiVersion.V3 => await CreateDatabase(translationOptions, CreateV3Database),
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

		private static async Task<List<LanguageMapping>> CreateDatabase(ITranslationOptions translationOptions, Func<ITranslationOptions, Task<List<LanguageMapping>>> createDatabaseFunc)
		{
			var languageMappings = await createDatabaseFunc(translationOptions);
			return languageMappings;
		}

		private static async Task<List<LanguageMapping>> CreateV2Database(ITranslationOptions translationOptions)
		{
			var v2Connector = new V2Connector(translationOptions.ApiKey, null);
			var v2Languages = await v2Connector.GetLanguages();

			return v2Languages
				.Where(language => IsValidLanguage(translationOptions.SelectedGoogleVersion, language))
				.Select(language => ParseLanguageMapping(language.LanguageName, language.LanguageCode))
				.ToList();
		}

		private static Task<List<LanguageMapping>> CreateV3Database(ITranslationOptions translationOptions)
		{
			var v3Connector = new V3Connector(translationOptions);
			var v3Languages = v3Connector.GetLanguages();

			return Task.FromResult(v3Languages
				.Where(language => IsValidLanguage(translationOptions.SelectedGoogleVersion, language))
				.Select(language => new LanguageMapping
				{
					Name = language.CultureInfo.DisplayName,
					LanguageCode = language.GoogleLanguageCode
				})
				.ToList());
		}


		private static List<LanguageMapping> CreateLanguageMappings(ITranslationOptions translationOptions)
		{
			return translationOptions.SelectedGoogleVersion switch
			{
				ApiVersion.V2 => CreateV2Database(translationOptions).Result,
				ApiVersion.V3 => CreateV3Database(translationOptions).Result,
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
			string pluginName = apiVersion == ApiVersion.V2
				? PluginResources.Database_PluginName_V2
				: PluginResources.Database_PluginName_V3;
			return string.Format(Constants.DatabaseFilePath, pluginName);
		}
	}
}