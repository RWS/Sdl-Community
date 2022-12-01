using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMX_Lib.Db;
using TMX_Lib.TmxFormat;

namespace TMX_Lib.Utils
{
	public static class Util
	{
		// strips punctuation + transforms it to lower case, for exact-search 
		// 
		// the idea for lower case: mongodb does have lower-case search feature, but it requires a locale
		// therefore, since we have texts in several languages, we can either:
		// a) write them as lower-case in db directly
		// b) have a specific table for each language
		//
		// at this time (11/25/2022) I opted for a)
		public static string TextToDbText(string text, CultureInfo language)
		{
			var noPunct = new StringBuilder();
			foreach (var c in text.Where(ch => !Char.IsPunctuation(ch)))
			{
				var ch = c;
				if (Char.IsWhiteSpace(ch))
					ch = ' ';

				// also: trim extra spaces, like : 2 consecutive space chars
				if (ch == ' ' && noPunct.Length > 0 && noPunct[noPunct.Length - 1] == ' ')
					continue;

				noPunct.Append(ch);
			}

			var result = noPunct.ToString().ToLower(language).Trim();
			return result;
		}

		// normalize language, so we don't worry about "en-US", "en_us", "en-us"
		public static string NormalizeLanguage(string language)
		{
			return language.Replace("_", "-").ToLowerInvariant();
		}
	}
}
