using System;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.Log
{
	public class ReportFile
	{
		public string FullPath { get; set; }

		public string Name { get; set; }

		public DateTime Created { get; set; }

		public Report.ReportScope Scope { get; set; }
	}
}
