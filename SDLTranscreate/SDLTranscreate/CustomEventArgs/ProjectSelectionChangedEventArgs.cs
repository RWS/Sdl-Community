using Sdl.Community.Transcreate.Model;

namespace Sdl.Community.Transcreate.CustomEventArgs
{
	public class ProjectSelectionChangedEventArgs: System.EventArgs
	{
		public Project SelectedProject { get; set; }
	}
}
