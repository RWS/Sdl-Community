using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_Lib.Utils
{
	public class CompareLanguages
	{
		public static bool Equivalent(string a, string b)
		{
			if (a == null || b == null || a.Length < 2 || b.Length < 2)
				// one of the strings is invalid
				return a.Equals(b, StringComparison.InvariantCultureIgnoreCase);

			// Syntax:
			// LL or LL-SS
			// LL - language, SS - dialect
			if ((a.Length == 2 && b.Length == 5) || (a.Length == 5 && b.Length == 2))
				// one without dialect, one with dialect -> ignore dialect altogether
				return a.Substring(0, 2).Equals(b.Substring(0, 2), StringComparison.InvariantCultureIgnoreCase) ;

			return a.Equals(b, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
