using System;
using System.Text.RegularExpressions;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public static class ContentHelper
	{
		public static bool ReverseSearch(string text,string regexRule)
		{
			try
			{
				var regex = new Regex(regexRule,RegexOptions.None);
				var match = regex.Match(text);

				//if the words matching the rule is not found return the segment
				if (!match.Success)
				{
					return true;
				}
			}
			catch (Exception e)
			{
				// ignored
			}
			return false;
		}
	}
}
