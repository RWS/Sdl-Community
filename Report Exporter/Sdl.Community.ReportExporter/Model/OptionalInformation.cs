using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ReportExporter.Model
{
	public class OptionalInformation
	{
		public bool IncludeAdaptiveBaseline { get; set; }
		public bool IncludeAdaptiveLearnings { get; set; }
		public bool IncludeInternalFuzzies { get; set; }
	}
}
