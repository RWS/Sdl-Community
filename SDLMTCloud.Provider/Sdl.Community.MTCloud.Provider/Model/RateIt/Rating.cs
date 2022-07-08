using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Model.RateIt
{
	public class Rating
	{
		// Corresponds to the options (checkboxes) set on the UI
		public List<string> Comments { get; set; }

		public int Score { get; set; }

		public void Empty()
		{
			Score = 0;
			Comments = new List<string>();
		}
	}
}