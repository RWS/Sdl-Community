using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.SdlTmAnonymizer.Model.Log
{
	public class ReportFile
	{
		public string FullPath { get; set; }

		public string Name { get; set; }

		public DateTime Created { get; set; }

		public Report.ReportType Type { get; set; }
	}
}
