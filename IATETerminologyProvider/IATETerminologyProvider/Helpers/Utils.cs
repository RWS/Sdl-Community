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
	}
}