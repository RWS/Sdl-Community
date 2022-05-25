using System;
using System.Collections;
using System.Globalization;

namespace Sdl.Community.MTEdge.LPConverter
{
	public static class MTEdgeLPExtensions
	{
		public static string ToMTEdgeCode(this CultureInfo language)
		{
			var languageCode = language.IetfLanguageTag.ToLower();

			// Make an exception for languages that require generics, but don't share a common ietf prefix with the generic
			// (ie nb-no doesn't have "no" as its first half, which in languages.xml, it should)
			if (Converter.ForcedGenericLPs.ContainsKey(languageCode))
			{
				return Converter.ForcedGenericLPs[languageCode];
			}

			var languageResource = MTEdgeLanguages.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
			var bestMatch = string.Empty;

			foreach (DictionaryEntry entry in languageResource)
			{
				var nonMTEdgeCode = entry.Value.ToString();
				// The more generic the nonMTEdgeCode, the better (less hyphens) with the
				// exception of certain cultures in the exception list above.
				if (languageCode.StartsWith(nonMTEdgeCode, StringComparison.OrdinalIgnoreCase))
				{
					if (Converter.LanguageCodeExceptions.Contains(nonMTEdgeCode.ToLower()))
					{
						return entry.Key.ToString();
					}
					if (nonMTEdgeCode.IndexOf('-') == -1)
					{
						bestMatch = entry.Key.ToString();
					}
				}
			}
			return bestMatch;
		}
	}
}