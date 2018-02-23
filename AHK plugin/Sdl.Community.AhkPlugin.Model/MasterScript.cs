using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AhkPlugin.Model
{
	public class MasterScript
	{
		public List<Script> ScriptList { get; set; }
		public string Location { get; set; }
		public string Text { get; set; }
	}
}
