using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AhkPlugin.Model
{
	public class Script
	{
		public Guid ScriptId { get; set; }
		public bool Active { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Text { get; set; }


	}
}
