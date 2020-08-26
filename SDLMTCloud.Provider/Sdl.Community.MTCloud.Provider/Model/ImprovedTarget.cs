using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class ImprovedTarget
	{
		public ImprovedTarget(string originalTarget)
		{
			OriginalTarget = originalTarget;
		}

		public List<string> Comments { get; set; }

		public string Improvement { get; set; }

		public string OriginalTarget { get; }

		public int Score { get; set; }
	}
}