using System.Collections.Generic;
using System.Linq;
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
			var ids = segmentsIds.Split(',').ToList();
			var idMachesRange = false;
			var valueMatches = false;
			var rangeList = GetRangeList(ids);
			var exactValuesList = GetExactValueList(ids);
			if (rangeList.Count > 0)
			{
				idMachesRange = IsInRangeList(segmentId, rangeList);
			}

			if (exactValuesList.Count > 0)
			{
				valueMatches = exactValuesList.Contains(Parse(segmentId));
			}

			return idMachesRange || valueMatches;
		}

		private static bool IsInRangeList(string segmentId, List<SegmentRange> rangeList)
		{
			var isInRange = false;
			foreach (var range in rangeList)
			{
				if (Enumerable.Range(range.Min, range.Max - range.Min + 1).Contains(Parse(segmentId)))
				{
					isInRange = true;
				}
			}
			return isInRange;
		}

		private static List<int> GetExactValueList(List<string> idsList)
		{
			var exactValuesList = new List<int>();
			foreach (var id in idsList)
			{
				if (!id.Contains("-"))
				{
					exactValuesList.Add(Parse(id));
				}
			}
			return exactValuesList;
		}
	

		private static List<SegmentRange> GetRangeList(List<string> idsRange)
		{
			var segmentsRangeList = new List<SegmentRange>();

			foreach (var idRange in idsRange)
			{
				if (idRange.Contains("-"))
				{
					var minValue = idRange.Substring(0, idRange.IndexOf('-'));
					var maxValue = idRange.Substring(idRange.IndexOf('-') + 1);
					var segmentRange = new SegmentRange
					{
						Min = Parse(minValue),
						Max = Parse(maxValue)
					};
					segmentsRangeList.Add(segmentRange);
				}
			}
			return segmentsRangeList;
		}
	}
}
