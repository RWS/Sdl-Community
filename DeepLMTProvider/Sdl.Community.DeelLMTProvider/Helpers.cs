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
			"JA",
			"NL",
			"PL",
			"ES",
			"PT",
			"PT-PT",
			"PT-BR",
			"RU",
			"ZH"
		};

		private static readonly List<string> TargetSupportedLanguages = new List<string>
		{
			"EN",
			"DE",
			"FR",
			"IT",
			"JA",
			"NL",
			"PL",
			"ES",
			"PT",
			"PT-PT",
			"PT-BR",
			"RU",
			"ZH"
		};

		public static bool IsSuportedLanguagePair(string sourceLang, string targetLang)
		{
			return SourceSupportedLanguages.Contains(sourceLang) && TargetSupportedLanguages.Contains(targetLang);
		}
	}
}
