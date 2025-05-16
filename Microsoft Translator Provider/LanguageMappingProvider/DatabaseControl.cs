using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LanguageMappingProvider;
using MicrosoftTranslatorProvider.Service;
using Sdl.Core.Globalization.LanguageRegistry;

namespace MicrosoftTranslatorProvider.LanguageMappingProvider
{
	public static class DatabaseControl
	{
		public static LanguageMappingDatabase InitializeDatabase()
		{
			var mappedLanguages = Task.Run(GetMappeddLanguages).Result;
			return new LanguageMappingDatabase("microsoft", mappedLanguages);
		}

		public async static Task<List<LanguageMapping>> GetMappeddLanguages()
		{
			var languages = await MicrosoftService.GetSupportedLanguages();
			var output = new List<LanguageMapping>();
			foreach (var language in languages)
			{
				if (language.LanguageCode.Contains("zh-"))
				{
					continue;
				}

				var parts = language.LanguageName.Split('\t');
				var nameParts = parts[0].Trim().Split('(');
				var name = nameParts[0].Trim();
				var region = nameParts.Length > 1 ? nameParts[1].Trim(')', ' ') : null;
				var languageCode = language.LanguageCode;

				var mappedLanguage = new LanguageMapping
				{
					Name = name,
					Region = region,
					LanguageCode = languageCode
				};

				output.Add(mappedLanguage);
			}

			output = output.Union(CreateChineseMapping()).OrderBy(x => x.Name).ThenBy(x => x.Region).ToList();
			return output;
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
