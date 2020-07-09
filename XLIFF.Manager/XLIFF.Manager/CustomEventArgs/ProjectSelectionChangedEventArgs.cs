using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.CustomEventArgs
{
	public class ProjectSelectionChangedEventArgs: System.EventArgs
	{
		public Project SelectedProject { get; set; }
	}
}
