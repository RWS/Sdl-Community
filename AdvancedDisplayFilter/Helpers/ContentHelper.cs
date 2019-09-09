using System.Text.RegularExpressions;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public static class ContentHelper
	{
		public static Match SearchContentRegularExpression(string text, string regexRule, RegexOptions regexOptions)
		{
			try
			{
				var regex = new Regex(regexRule, regexOptions);
				return regex.Match(text);
			}
			catch
			{
				// catch all; ignore
			}
			return null;
		}
	}
}
