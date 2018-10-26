using System;
using System.Collections.Generic;

namespace Sdl.Community.SdlTmAnonymizer.Model.Log
{
	[Serializable]
	public class Report
	{
		public enum ReportScope
		{
			All = 0,
			Content = 1,
			SystemFields = 2,
			CustomFields = 3
		}

		public Report()
		{
		}

		public Report(TmFile tmFile)
		{
			TmFile = tmFile;
			Actions = new List<Model.Log.Action>();
			ElapsedSeconds = 0;
			Created = DateTime.Now;
			Type = "Update TM";
		}

		public string ReportFullPath { get; set; }

		public int UpdatedCount { get; set; }

		public TmFile TmFile { get; set; }

		public DateTime Created { get; set; }

		public double ElapsedSeconds { get; set; }

		public List<Action> Actions { get; set; }
		
		public ReportScope Scope { get; set; }

		public string Type { get; set; }
	}
}
