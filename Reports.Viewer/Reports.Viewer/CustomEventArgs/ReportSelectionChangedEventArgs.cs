using Sdl.Community.Reports.Viewer.Model;

namespace Sdl.Community.Reports.Viewer.CustomEventArgs
{
	public class ReportSelectionChangedEventArgs: System.EventArgs
	{
		public Report SelectedReport { get; set; }
	}
}
