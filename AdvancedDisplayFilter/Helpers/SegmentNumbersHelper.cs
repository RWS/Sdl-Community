using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.Plugins.AdvancedDisplayFilter.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using static System.Int32;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public static class SegmentNumbersHelper
	{
		public static bool IsEven(string segmentId)
		{
			//in case of split segments the id looks like this: 1 a b 
			var id = 0;
			if (segmentId.Contains(" "))
			{
				var idSplit = segmentId.Split(' ');
				if (idSplit.Any())
				{
					id = Parse(idSplit[0]);
				}
			}
			else
			{
				id = Parse(segmentId);
			}
			return id % 2 == 0;
		}

		public static bool IsOdd(string segmentId)
		{
			var id = 0;
			if (segmentId.Contains(" "))
			{
				var idSplit = segmentId.Split(' ');
				if (idSplit.Any())
				{
					id = Parse(idSplit[0]);
				}
			}
			else
			{
				id = Parse(segmentId);
			}
			return id % 2 == 1;
		}
		
		public static bool IdInRange(string segmentId, string segmentsIds)
		{
			segmentsIds = segmentsIds.Replace(" ", string.Empty);
			var ids = segmentsIds.Split(',').ToList();
			var idMachesRange = false;
			var valueMatches = false;
			var rangeList = GetRangeList(ids);
			var exactValuesList = GetExactValueList(ids);

			//check if is split segment
			if (segmentId.Contains(" "))
			{
				var splitId = segmentId.Split(' ');
				if (splitId.Any())
				{
					segmentId = splitId[0];
				}
			}

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

		/// <summary>
		/// Sslit segments : 1 a , 1 a b, 1 b c
		/// </summary>
		/// <param name="rowId"></param>
		/// <param name="currentDocument"></param>
		/// <returns></returns>
		public static bool IsSplitSegment(string rowId, Document currentDocument)
		{
			var isCompex = IsComplexId(rowId);
			if (isCompex)
			{
				var isMergedSegment = IsMergedSegment(rowId, currentDocument);
				if (isMergedSegment)
				{
					//is not split segment
					return false;
				}
				return true;
			}
			return false;
		}

		private static bool IsComplexId(string id)
		{
			var regex = new Regex("^[0-9 ]*[a-z ]+$");
			var match = regex.Match(id);
			return match.Success;
		}
		/// <summary>
		/// Merged segments has ids like this 1a and appears only once
		/// </summary>
		/// <param name="rowId"></param>
		/// <param name="currentDocument"></param>
		/// <returns></returns>
		public static bool IsMergedSegment(string rowId, Document currentDocument)
		{
			var isCompexId = IsComplexId(rowId);
			var count = 0;
			if (isCompexId)
			{
				var match = Regex.Matches(rowId, "[1-9]*");
				if (match.Count > 0)
				{
					rowId = match[0].Value;
				}

				var segments = currentDocument.SegmentPairs.ToList();
				foreach (var segment in segments)
				{
					var currentSegmentId = segment.Source.Properties.Id.Id;
					//take only number value from id
					var currentSegmentMatch = Regex.Matches(currentSegmentId, "[1-9]*");
					if (currentSegmentMatch.Count > 0)
					{
						currentSegmentId = currentSegmentMatch[0].Value;
					}
					//count how many times same id appears in segments id
					if (rowId.Equals(currentSegmentId))
					{
						count++;
					}
					//this means is split segment
					if (count.Equals(2))
					{
						return false;
					}
				}
				if (count.Equals(1))
				{
					return true;
				}
			}
			
			return  false;
		}

		public static bool IsSourceEqualsToTarget(ISegmentPair segmentPair,bool caseSensitive)
		{
			if (segmentPair.Source.ToString() != string.Empty && segmentPair.Target.ToString() != string.Empty)
			{
				var textVisitor = new SegmentTextVisitor();
				var sourceText = textVisitor.GetText(segmentPair.Source);
				var targetText = textVisitor.GetText(segmentPair.Target);

				var isEqual = string.Compare(sourceText, targetText, !caseSensitive);
				return isEqual.Equals(0);
			}
			return false;
		}
	}
}
