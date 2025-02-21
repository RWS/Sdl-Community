using System.Collections.Generic;
using Reports.Viewer.Api.Model;

namespace Reports.Viewer.Plus.CustomEventArgs
{
	public class ReportSelectionChangedEventArgs: System.EventArgs
	{
		public Report SelectedReport { get; set; }

		public List<Report> SelectedReports { get; set; }
	}
}
