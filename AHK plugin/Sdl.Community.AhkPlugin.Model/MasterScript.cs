using System.Collections.Generic;

namespace Sdl.Community.AhkPlugin.Model
{
	public class MasterScript
	{
		public string Location { get; set; }
		public List<Script> Scripts { get; set; }

		public int Id { get; set; }
		public string Name { get; set; }
	}
}
