using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_TranslationProvider.Utils
{
	internal static class Extensions
	{
		public static string IsoLanguageName(this CultureInfo ci)
		{
			return ci.Name;
		}
	}
}
