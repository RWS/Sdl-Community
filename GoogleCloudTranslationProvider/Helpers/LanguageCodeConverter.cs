using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GoogleCloudTranslationProvider.Extensions;
using GoogleCloudTranslationProvider.Models;
using LanguageMappingProvider.Database;

namespace GoogleCloudTranslationProvider.Helpers
{
	public static class LanguageCodeConverter
	{
		public static string ConvertLanguageCode(this CultureInfo cultureInfo)
		{
			if (cultureInfo.Name.StartsWith("zh"))
			{
				return cultureInfo.Parent.Name.ToLower().StartsWith("zh-cht") ? "zh-Hant" : "zh-Hans";
			}

			if (cultureInfo.Name == "fr-HT") { return "ht"; }
			if (cultureInfo.Name.Equals("nb-NO") || cultureInfo.Name.Equals("nn-NO")) return "no";
			if (cultureInfo.TwoLetterISOLanguageName.Equals("sr") && cultureInfo.DisplayName.ToLower().Contains("latin")) return "sr-Latn";

			if (cultureInfo.DisplayName == "Samoan") { return "sm"; }

			var isoLanguageName = cultureInfo.TwoLetterISOLanguageName; //if not chinese trad or norweigian get 2 letter code

			//convert tagalog and hebrew for Google
			if (isoLanguageName == "fil") { isoLanguageName = "tl"; }
			if (isoLanguageName == "he") { isoLanguageName = "iw"; }

			return isoLanguageName;
		}

		public static string GetLanguageCode(this CultureInfo cultureInfo, ApiVersion targetVersion)
		{
			var regex = new Regex(@"^(.*?)\s*(?:\((.*?)\))?$");
			var match = regex.Match(cultureInfo.DisplayName);
			var languageName = match.Groups[1].Value;
			var languageRegion = match.Groups[2].Success ? match.Groups[2].Value : null;

			var targetDatabase = targetVersion == ApiVersion.V2
							   ? PluginResources.Database_PluginName_V2
							   : PluginResources.Database_PluginName_V3;

			var database = new LanguageMappingDatabase(targetDatabase, null);
			var mappings = database.GetMappedLanguages();

			var targetLanguage = mappings.FirstOrDefault(x => x.Name == languageName && x.Region == languageRegion);

			return targetLanguage.LanguageCode;
		}
	}
}