using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExportAnalysisReports.Service;

namespace Sdl.Community.ExportAnalysisReports.UnitTests.Helpers
{
	public class CustomPathInfo: PathInfo
	{
		public override string ApplicationFullPath { get; set; }
	}
}
