using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Community.MTEdge.LPConverter
{
	public class Converter
	{
		// A list of language codes that do not default to the more generic parent's SDLMTEdge Code
		internal static readonly List<string> LanguageCodeExceptions = new List<string>
		{
			"zh-hans", "zh-hant", "prs-af", "pt-br"
		};

		internal static readonly Dictionary<string, string> ForcedGenericLPs = new Dictionary<string, string>()
		{
			// Norwegian
			{"nb-no", "nor"},
			{"nn-no", "nor"},
			{"nb-sj", "nor"},

			// Traditional Chinese
			{"zh-mo", "cht"},
			{"zh-hk", "cht"},
			{"zh-tw", "cht"},

			// Simplified Chinese
			{"zh-cn", "chi"},
			{"zh-sg", "chi"}
		};

		public static CultureInfo MTEdgeCodeToCulture(string mtEdgeCode)
		{
			// Make an exception for languages that require generics, but don't share a common ietf prefix with the generic
			// (ie nb-no doesn't have "no" as its first half, which in languages.xml, it should)
			if (ForcedGenericLPs.ContainsValue(mtEdgeCode))
			{
				return CultureInfo.GetCultureInfo(ForcedGenericLPs.First(lp => lp.Value == mtEdgeCode).Key);
			}

			var languageResource =
				MTEdgeLanguages.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
			var bestMatch = string.Empty;

			foreach (DictionaryEntry entry in languageResource)
			{
				var nonMtEdgeCode = entry.Value.ToString();
				// The more generic the nonMtEdgeCode, the better (less hyphens) with the
				// exception of certain cultures in the exception list above.
				if (mtEdgeCode.StartsWith(entry.Key.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					if (LanguageCodeExceptions.Contains(nonMtEdgeCode.ToLower()))
					{
						return CultureInfo.GetCultureInfo(nonMtEdgeCode);
					}

					if (string.IsNullOrEmpty(bestMatch) || nonMtEdgeCode.IndexOf('-') == -1)
					{
						bestMatch = entry.Value.ToString();
					}
				}
			}

			return CultureInfo.GetCultureInfo(bestMatch);
		}
	}
}