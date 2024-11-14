using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Compare.Core.Helper
{
	public static class Convert
	{
		public static decimal ToDecimal(string value)
		{
			return System.Convert.ToDecimal(value, CultureInfo.InvariantCulture);
		}
		
		public static decimal ToDecimal(int value)
		{
			return System.Convert.ToDecimal(value, CultureInfo.InvariantCulture);
		}
		
		public static int ToInt32(decimal value)
		{
			return System.Convert.ToInt32(value, CultureInfo.InvariantCulture);
		}
	}
}