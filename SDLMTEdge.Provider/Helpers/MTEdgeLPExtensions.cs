using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Community.MTEdge.Provider.Helpers
{
	public static class MTEdgeLPExtensions
    {
        private static readonly HashSet<string> LanguageCodeExceptions = new()
        {
            "zh-hans", "zh-hant", "prs-af", "pt-br"
        };

        private static readonly Dictionary<string, string> ForcedGenericPairs = new()
        {
            { "nb-no", "nor" },
            { "nn-no", "nor" },
            { "nb-sj", "nor" },
            { "zh-mo", "cht" },
            { "zh-hk", "cht" },
            { "zh-tw", "cht" },
            { "zh-cn", "chi" },
            { "zh-sg", "chi" }
        };

        public static string ToMTEdgeCode(this CultureInfo language)
        {
            var languageCode = language.IetfLanguageTag.ToLower();
            if (ForcedGenericPairs.TryGetValue(languageCode, out string output))
            {
                return output;
            }

            var bestMatch = string.Empty;
			var entries = MTEdgeLanguages.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
			foreach (DictionaryEntry entry in entries)
            {
                var nonMTEdgeCode = (entry.Value as string).ToLower();
                if (!languageCode.StartsWith(nonMTEdgeCode, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                else if (LanguageCodeExceptions.Contains(nonMTEdgeCode))
                {
                    return entry.Key as string;
                }
                else if (nonMTEdgeCode.IndexOf('-') == -1)
                {
                    bestMatch = entry.Key as string;
                }
				else if (languageCode == "fr-ca"
					&& nonMTEdgeCode == languageCode)
				{
					return entry.Key as string;
				}
            }

            return bestMatch;
        }
    }
}