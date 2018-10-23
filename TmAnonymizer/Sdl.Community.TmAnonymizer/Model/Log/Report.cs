using System;
using System.Collections.Generic;

namespace Sdl.Community.SdlTmAnonymizer.Model.Log
{
	[Serializable]
	public class Report
	{
		public string ReportFullPath { get; set; }

		public int UpdatedCount { get; set; }

		public TmFile TmFile { get; set; }

		public DateTime Created { get; set; }

		public TimeSpan ElapsedTime { get; set; }

		public List<Action> Actions { get; set; }		
	}
}
