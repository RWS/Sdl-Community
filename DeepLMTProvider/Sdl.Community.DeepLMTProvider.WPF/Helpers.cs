using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider
{
	public static class Helpers
	{
		private static readonly List<string> FormalityIncompatibleTargetLanguages = new List<string>
		{
			"ja",
			"zh",
			"en",
			"en-gb",
			"en-us",
		};

		public static bool AreLanguagesCompatibleWithFormalityParameter(List<CultureInfo> targetLanguages)
		{
			return targetLanguages.All(IsLanguageCompatible);
		}

		public static bool IsLanguageCompatible(CultureInfo targetLanguage)
		{
			var twoLetterIsoLanguage = targetLanguage.TwoLetterISOLanguageName.ToLowerInvariant();
			return !FormalityIncompatibleTargetLanguages.Contains(twoLetterIsoLanguage);
		}
	}
}