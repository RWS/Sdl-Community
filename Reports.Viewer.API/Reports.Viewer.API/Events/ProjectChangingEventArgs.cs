using System;

namespace Sdl.Reports.Viewer.API.Events
{
	public class ProjectChangingEventArgs: EventArgs
	{
		public string PreviousProjectId { get; internal set; }

		public string ProjectId { get; internal set; }
	}
}
