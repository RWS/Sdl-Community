using System.Collections.Generic;
using Reports.Viewer.Api.Model;

namespace Sdl.Community.Reports.Viewer.CustomEventArgs
{
	public class ReportSelectionChangedEventArgs: System.EventArgs
	{
		public Report SelectedReport { get; set; }

		public List<Report> SelectedReports { get; set; }
	}
}
