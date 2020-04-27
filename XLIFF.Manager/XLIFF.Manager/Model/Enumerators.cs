using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class Enumerators
	{
		public enum Action
		{
			None = 0,
			Export = 1,
			Import = 2
		}

		public enum Status
		{
			None = 0,
			Ready = 1,
			Exported = 2,
			Imported = 3,
			Error = 4
		}
	}
}
