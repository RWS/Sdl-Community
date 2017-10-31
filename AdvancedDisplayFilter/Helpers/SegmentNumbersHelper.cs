using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Int32;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public static class SegmentNumbersHelper
	{
		public static bool IsEven(string segmentId)
		{
			var id = Parse(segmentId);
			return id % 2 == 0;
		}

		public static bool IsOdd(string segmentId)
		{
			var id = Parse(segmentId);
			return id % 2 == 1;
		}
	}
}
