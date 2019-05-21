using System.Collections.Generic;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class ProjectResponse
	{
		public int Count { get; set; }
		public List<ProjectDetailsResponse> Items { get; set; }
	}
}
