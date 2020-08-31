using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class ImprovedTarget
	{
		public ImprovedTarget(string originalTarget, string originalSource)
		{
			OriginalTarget = originalTarget;
			OriginalSource = originalSource;
		}

		public List<string> Comments { get; set; }

		public string Improvement { get; set; }

		public string OriginalTarget { get; }

		public string OriginalSource { get; }

		public int Score { get; set; }
	}
}