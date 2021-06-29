using System;

namespace Sdl.Community.MTCloud.Provider.Model.QELabelExtractorModel
{
	public class ReportSummary
	{
		public DateTime CreatedAt { get; set; }
		public DateTime? DueDate { get; set; }
		public int Files { get; set; }
		public string Language { get; set; }
		public string Location { get; set; }
		public string Project { get; set; }
		public string Task { get; set; }
	}
}