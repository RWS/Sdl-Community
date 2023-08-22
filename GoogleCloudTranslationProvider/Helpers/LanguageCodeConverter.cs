using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GoogleCloudTranslationProvider.Extensions;
using GoogleCloudTranslationProvider.Models;
using LanguageMappingProvider.Database;
using Sdl.Core.Globalization;

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

		public static string GetLanguageCode(this CultureCode cultureInfo, ApiVersion targetVersion)
		{
			var targetDatabase = targetVersion == ApiVersion.V2
							   ? PluginResources.Database_PluginName_V2
							   : PluginResources.Database_PluginName_V3;

			var database = new LanguageMappingDatabase(targetDatabase, null);
			var mappings = database.GetMappedLanguages();

			var mappedLanguage = mappings.FirstOrDefault(x => x.TradosCode.ToLower().Equals(cultureInfo.Name.ToLower()));
			return mappedLanguage?.LanguageCode;
		}
	}
}