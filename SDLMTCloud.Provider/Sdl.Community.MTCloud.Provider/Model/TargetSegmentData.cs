using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class TargetSegmentData
	{
		public List<string> TargetSegments { get; set; }
		public string Model { get; set; }
		public string[] QualityEstimation { get; set; } 
	}
}
