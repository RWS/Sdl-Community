using System.Text.RegularExpressions;

namespace Sdl.Community.AdvancedDisplayFilter.Helpers
{
	public static class ContentHelper
	{
		public static Match SearchContentRegularExpression(string text, string searchString, RegexOptions regexOptions, out Regex regex)
		{
			try
			{
				regex = new Regex(searchString, regexOptions);
				return regex.Match(text);
			}
			catch
			{
				// catch all; ignore
			}

			regex = null;
			return null;
		}
	}
}
