using System.Collections.Generic;

namespace Reports.Viewer.Api.Model
{
	public class ActionResult
	{
		public ActionResult() : this(true) { }

		public ActionResult(bool success)
		{
			Success = success;
		}

		public bool Success { get; internal set; }

		public string Message { get; internal set; }

		public List<Report> Reports { get; internal set; }
	}
}
