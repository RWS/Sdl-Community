using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.SdlTmAnonymizer.Model.Log
{
	[Serializable]
	public class Detail
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public string Previous { get; set; }
		public string Value { get; set; }
	}
}
