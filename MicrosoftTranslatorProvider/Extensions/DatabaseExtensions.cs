using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LanguageMappingProvider.Database;
using LanguageMappingProvider.Model;
using MicrosoftTranslatorProvider.Service;
using MicrosoftTranslatorProvider.Interfaces;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;

namespace MicrosoftTranslatorProvider.Extensions
{
	public static class DatabaseExtensions
	{
		public static string GetLanguageCode(this CultureInfo cultureInfo)
		{
			var regex = new Regex(@"^(.*?)\s*(?:\((.*?)\))?$");
			var match = regex.Match(cultureInfo.DisplayName);
			var languageName = match.Groups[1].Value;
			var languageRegion = match.Groups[2].Success ? match.Groups[2].Value : null;

			var database = new LanguageMappingDatabase(Constants.DatabaseName, null);
			var mappings = database.GetMappedLanguages();

			var targetLanguage = mappings.FirstOrDefault(x => x.Name == languageName && x.Region == languageRegion)
							  ?? mappings.FirstOrDefault(x => x.TradosCode == cultureInfo.Name);

			return targetLanguage.LanguageCode;
		}

		public static List<LanguageMapping> GetDefaultMapping(ITranslationOptions translationOptions)
		{
			return null;
			/*var supportedLanguages = MicrosoftService.GetSupportedLanguageCodes().Where(x => !x.Name.Contains("Chinese"));
			var regex = new Regex(@"^(.*?)\s*(?:\((.*?)\))?$");
			foreach (var supportedLanguage in supportedLanguages.Where(x => x.Name.Contains("(")))
			{
				if (supportedLanguage.Name.Contains("Chinese"))
				{
					continue;
				}

				var match = regex.Match(supportedLanguage.Name);

				supportedLanguage.Name = match.Groups[1].Value;
				supportedLanguage.Region = match.Groups[2].Value;
			}

			return supportedLanguages.Union(CreateChineseMapping())
									 .OrderBy(x => x.Name).ThenBy(x => x.Region)
									 .ToList();*/
		}

		public static void CreateDatabase(ITranslationOptions translationOptions)
		{
			var supportedLanguages = GetDefaultMapping(translationOptions);
			_ = new LanguageMappingDatabase(Constants.DatabaseName, supportedLanguages);
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
	}
}