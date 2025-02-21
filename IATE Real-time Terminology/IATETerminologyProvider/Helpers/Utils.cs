using System.Globalization;

namespace Sdl.Community.IATETerminologyProvider.Helpers
{
	public static class Utils
	{
		public static string UppercaseFirstLetter(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}

			var textInfo = CultureInfo.CurrentCulture.TextInfo;
			return textInfo.ToTitleCase(s);
		}
	}
}