using System;
using System.Security;

namespace Multilingual.Excel.FileType.FileType.Preview
{
	public static class StringExtensions
	{
		public static string EscapeXml(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return s;

			var returnString = SecurityElement.Escape(s);

			return returnString;
		}

		public static string UnescapeXml(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return s;

			string returnString = s;
			returnString = returnString.Replace("&apos;", "'");
			returnString = returnString.Replace("&quot;", "\"");
			returnString = returnString.Replace("&gt;", ">");
			returnString = returnString.Replace("&lt;", "<");
			returnString = returnString.Replace("&amp;", "&");

			return returnString;
		}

		internal static string RemoveXmlTagDelimiter(this string tagContent)
		{
			if (!string.IsNullOrEmpty(tagContent) && tagContent[0] == '<' && tagContent.Length > 1)
			{
				tagContent = tagContent.TrimStart('<').TrimStart('/');
				return tagContent.TrimEnd('>');
			}

			return tagContent;
		}

		public static string GetXmlNodeContent(string s)
		{
			string result = string.Empty;

			var endStartTagIndex = s.IndexOf('>');

			if (endStartTagIndex != -1)
			{
				result = s.Substring(endStartTagIndex + 1);
				var startEndTagIndex = result.LastIndexOf("</", StringComparison.InvariantCultureIgnoreCase);
				if (startEndTagIndex != -1)
				{
					result = result.Substring(0, startEndTagIndex);
				}
			}

			return result;
		}
	}
}
