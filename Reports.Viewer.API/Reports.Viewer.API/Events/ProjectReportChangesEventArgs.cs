using System;
using System.Collections.Generic;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Reports.Viewer.API.Events
{
	public class ProjectReportChangesEventArgs : EventArgs
	{
		public string ClientId { get; internal set; }

		public string ProjectId { get; internal set; }

		public List<Report> AddedReports { get; internal set; }

		public List<Report> RemovedReports { get; internal set; }
	}
}
