using System.Collections.Generic;

namespace Sdl.Reports.Viewer.API.Model
{
	public class ReportChanges
	{		
		public string ClientId { get; internal set; }

		public string ProjectId { get; internal set; }

		public List<Report> AddedReports { get; internal set; }

		public List<Report> RemovedReports { get; internal set; }
	}
}
