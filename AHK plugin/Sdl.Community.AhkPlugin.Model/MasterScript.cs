using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AhkPlugin.Model
{
	public class MasterScript
	{
		public string Location { get; set; }
		public List<Script> Scripts { get; set; }

		public string ScriptId { get; set; }
		public string Name { get; set; }
	}
}
