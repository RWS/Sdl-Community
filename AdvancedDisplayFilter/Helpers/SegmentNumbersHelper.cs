using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Models;
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

		public static bool ContainsId(string segmentId, string segmentsIds)
		{
			segmentsIds= segmentsIds.Replace(" ",string.Empty);
			var ids = segmentsIds.Split(',').ToList();

			return ids.Contains(segmentId);
		}

		public static bool IdInRange(string segmentId, string segmentsIds)
		{
			segmentsIds = segmentsIds.Replace(" ", string.Empty);
			var rangeList = segmentsIds.Split(',').ToList();
			var segmentsRangeList = new List<SegmentRange>();

			foreach (var range in rangeList)
			{
				var minValue = range.Substring(0, range.IndexOf('-'));
				var maxValue = range.Substring(range.IndexOf('-') + 1);
				var segmentRange = new SegmentRange
				{
					Min = Parse(minValue),
					Max = Parse(maxValue)
				};
				segmentsRangeList.Add(segmentRange);
			}
			foreach (var segment in segmentsRangeList)
			{
				if (Enumerable.Range(segment.Min, segment.Max-segment.Min+1).Contains(Parse(segmentId)))
				{
					return true;
				}
			}
			return false;
		}
	}
}
