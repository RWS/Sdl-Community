using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider
{
	public static class Helpers
	{
		private static readonly List<string> SourceSupportedLanguages = new List<string>
		{
			"EN",
			"DE",
			"FR",
			"IT",
			"NL",
			"PL",
			"ES",
			"PT",
			"RU",
			"JA",
			"ZH"
		};

		private static readonly List<string> TargetSupportedLanguages = new List<string>
		{
			"EN",
			"DE",
			"FR",
			"IT",
			"NL",
			"PL",
			"ES",
			"PT",
			"PT-PT",
			"PT-BR",
			"RU",
			"JA",
			"ZH"
		};

		public static bool IsSupportedLanguagePair(string sourceLang, string targetLang)
		{
			return SourceSupportedLanguages.Contains(sourceLang) && TargetSupportedLanguages.Contains(targetLang);
		}
	}
}
