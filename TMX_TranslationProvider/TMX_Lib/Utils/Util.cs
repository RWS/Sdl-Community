using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;
using TMX_Lib.Db;
using TMX_Lib.TmxFormat;

namespace TMX_Lib.Utils
{
	public static class Util
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

#if DEBUG
		public static bool IsDebug = true;
#else
		public static bool IsDebug = false;
#endif

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

		public static void ThrowTmxException(string msg, Exception e = null)
		{
			log.Error($"Exception {msg} : {(e != null ? "original msg:" + e.ToString() : "")}");
			LogManager.Flush();

			if (e != null)
				throw new TmxException(msg);
			else
				throw new TmxException(msg, e);
		}

		public static (char ch, int pos) StringCharAt(string s, int lineIdx, int colIdx)
		{
			var curLineIdx = 0;
			var curColIdx = 0;
			var posIdx = 0;

			for (var i = 0; i < s.Length; i++, posIdx++)
			{
				var ch = s[i];
				if (ch == '\r' || ch == '\n')
				{
					if (i < s.Length - 1)
					{
						var next = s[i + 1];
						if ((next == '\r' || next == '\n') && ch != next)
						{
							// \r\n or \n\r
							++i;
							++posIdx;
						}
					}

					++curLineIdx;
					curColIdx = 0;
				} else if (curLineIdx == lineIdx && curColIdx == colIdx)
					return (ch, posIdx);
				else
					curColIdx++;
			}

			throw new TmxException($"String does not have {lineIdx}:{colIdx} position");
		}

		// returns true on success
		// returns false if the node was not found
		public static bool TryRemoveXmlNodeContainingChar(ref string s, int pos, string xmlNodeName)
		{
			var start1 = $"<{xmlNodeName} ";
			var start2 = $"<{xmlNodeName}>";
			var idx1 = s.LastIndexOf(start1, pos, StringComparison.InvariantCultureIgnoreCase);
			var idx2 = s.LastIndexOf(start2, pos, StringComparison.InvariantCultureIgnoreCase);
			var idxStart = Math.Max(idx1, idx2);
			var end = $"</{xmlNodeName}>";
			var idxEnd = s.IndexOf(end, pos);
			if (idxStart < 0 || idxEnd < 0)
				return false;

			idxEnd += end.Length;
			var trimmed = s.Substring(0, idxStart) + s.Substring(idxEnd);
			s = trimmed;
			return true;
		}

	}
}
