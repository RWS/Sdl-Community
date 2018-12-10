using System.Text.RegularExpressions;

namespace IATETerminologyProvider.Helpers
{
	public static class Utils
	{
		public static string RemoveUriForbiddenCharacters(this string uriString)
		{
			var regex = new Regex(@"[$%+!*'(), ]");
			return regex.Replace(uriString, "");
		}

		public static string UppercaseFirstLetter(string s)
		{
			// Check for empty string.
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			// Return char and concat substring.
			return char.ToUpper(s[0]) + s.Substring(1);
		}
	}
}