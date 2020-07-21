using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Rating
	{
		public int Score { get; set; }
		// Corresponds to the options (checkboxes) set on the UI
		public List<string> Comments { get; set; }
	}
}
