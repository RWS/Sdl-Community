using System.Text.RegularExpressions;

namespace Sdl.Community.AntidoteVerifier.Extensions
{
	public static class StringExtensions
	{
		public static string RemoveTags(this string input)
		{
			var pattern = "<.*?>"; // This regular expression matches any HTML or XML tag
			return Regex.Replace(input, pattern, string.Empty);
		}
	}
}