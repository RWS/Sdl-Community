using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TermExcelerator
{
	public class Allow
	{
		private static bool Access;

		public Allow() { }

		public static bool GetAccess()
		{
			return Access;
		}

		public static void SetAccess(bool x)
		{
		      Access = x;
		}
	}
}
