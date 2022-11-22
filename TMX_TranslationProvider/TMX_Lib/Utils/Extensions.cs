using System.Globalization;

namespace TMX_Lib.Utils
{
	public static class Extensions
	{
		public static string IsoLanguageName(this CultureInfo ci)
		{
			return ci.Name;
		}
	}
}
