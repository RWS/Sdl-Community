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
		public Allow(bool access) 
		{ 
			Access = access;
		}

		public Allow() { }

		public bool Allowaccess { get { return Access; } set {  Access = value; } }

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
