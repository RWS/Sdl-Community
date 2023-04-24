using System.Globalization;

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
	}
}