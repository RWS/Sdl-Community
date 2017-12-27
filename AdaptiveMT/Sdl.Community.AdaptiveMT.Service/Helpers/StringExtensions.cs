using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AdaptiveMT.Service.Helpers
{
	internal static class StringExtensions
	{
		public static Uri FormatUri(this string pattern, params object[] args)
		{
			return new Uri(string.Format(CultureInfo.InvariantCulture, pattern, args), UriKind.Absolute);
		}
	}
}
