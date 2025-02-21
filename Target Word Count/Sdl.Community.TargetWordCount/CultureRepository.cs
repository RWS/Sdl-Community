using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Community.TargetWordCount
{
	public static class CultureRepository
	{
		private static Dictionary<string, CultureInfo> cultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
															.GroupBy(c => c.EnglishName)
															.ToDictionary(g => g.Key, g => g.First());

		public static Dictionary<string, CultureInfo> Cultures { get { return cultures; } }
	}
}