using System.Collections.Generic;

namespace Sdl.Community.StarTransit.Shared.Events
{
	public class ProjectCreationProgress
	{
		public List<string> BatchTaskIds { get; set; }
		public int Progress { get; set; }
	}
}
