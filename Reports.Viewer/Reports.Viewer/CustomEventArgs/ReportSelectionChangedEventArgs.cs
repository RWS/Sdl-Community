using Sdl.Community.Reports.Viewer.Model;
using Sdl.Reports.Viewer.API;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Community.Reports.Viewer.CustomEventArgs
{
	public class ReportSelectionChangedEventArgs: System.EventArgs
	{
		public Report SelectedReport { get; set; }
	}
}
