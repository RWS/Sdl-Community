using System.Collections.Generic;
using System.Linq;
using Sdl.Community.TargetWordCount.Models;
using Sdl.Core.Globalization;

namespace Sdl.Community.TargetWordCount
{
	public static class ReportGenerator
	{
		private const string Characters = "Characters";
		private const string Words = "Words";

		public static string Generate(List<ISegmentWordCounter> counters, IWordCountBatchTaskSettings settings)
		{
			var grandTotal = new CountTotal();
			var fileData = new List<CountTotal>();

			CollectFileData(counters, settings, grandTotal, fileData);

			grandTotal.CountMethod = fileData.First().CountMethod;
			grandTotal.FileName = "Total";

			return CreateReport(grandTotal, fileData, settings);
		}

		private static void AccumulateCountData(IWordCountBatchTaskSettings settings, ISegmentWordCounter counter, CountTotal info)
		{
			info.FileName = counter.FileName;

			SetCountMethod(settings, counter, info);

			foreach (var segInfo in counter.FileCountInfo.SegmentCounts)
			{
				var origin = segInfo.TranslationOrigin;

				if (origin == null)
				{
					info.Increment(CountTotal.New, segInfo.CountData);
				}
				else
				{
					if (settings.ReportLockedSeperately && segInfo.IsLocked)
					{
						info.Increment(CountTotal.Locked, segInfo.CountData);
					}
					else if (origin.OriginType == "document-match")
					{
						info.Increment(CountTotal.PerfectMatch, segInfo.CountData);
					}
					else if (origin.IsRepeated)
					{
						info.Increment(CountTotal.Repetitions, segInfo.CountData);
					}
					else if (origin.MatchPercent == 100)
					{
						if (origin.TextContextMatchLevel == Sdl.FileTypeSupport.Framework.NativeApi.TextContextMatchLevel.SourceAndTarget)
						{
							info.Increment(CountTotal.ContextMatch, segInfo.CountData);
						}
						else
						{
							info.Increment(CountTotal.OneHundredPercent, segInfo.CountData);
						}
					}
					else if (origin.MatchPercent >= 95)
					{
						info.Increment(CountTotal.NinetyFivePercent, segInfo.CountData);
					}
					else if (origin.MatchPercent >= 85)
					{
						info.Increment(CountTotal.EightyFivePercent, segInfo.CountData);
					}
					else if (origin.MatchPercent >= 75)
					{
						info.Increment(CountTotal.SeventyFivePercent, segInfo.CountData);
					}
					else if (origin.MatchPercent >= 50)
					{
						info.Increment(CountTotal.FiftyPercent, segInfo.CountData);
					}
					else
					{
						info.Increment(CountTotal.New, segInfo.CountData);
					}
				}

				if (!(settings.ReportLockedSeperately && segInfo.IsLocked))
				{
					info.Increment(CountTotal.Total, segInfo.CountData);
				}

				if (segInfo.IsLocked)
				{
					info.LockedSpaceCountTotal += segInfo.SpaceCount;
				}
				else
				{
					info.UnlockedSpaceCountTotal += segInfo.SpaceCount;
				}
			}
		}

		private static void CollectFileData(List<ISegmentWordCounter> counters, IWordCountBatchTaskSettings settings, CountTotal grandTotal, List<CountTotal> fileData)
		{
			foreach (var counter in counters)
			{
				var info = new CountTotal();

				AccumulateCountData(settings, counter, info);

				fileData.Add(info);
				grandTotal.Increment(info);
				grandTotal.UnlockedSpaceCountTotal += info.UnlockedSpaceCountTotal;
				grandTotal.LockedSpaceCountTotal += info.LockedSpaceCountTotal;
			}
		}

		private static string CreateReport(CountTotal grandTotal, List<CountTotal> fileData, IWordCountBatchTaskSettings settings)
		{
			var builder = new ReportBuilder();

			// Build grand total table
			builder.BuildTotalTable(grandTotal, settings);

			// Build individual file tables
			foreach (var data in fileData)
			{
				builder.BuildFileTable(data, settings);
			}

			return builder.GetReport();
		}

		private static void SetCountMethod(IWordCountBatchTaskSettings settings, ISegmentWordCounter counter, CountTotal info)
		{
			Language language = null;

			if (settings.UseSource)
			{
				language = counter.FileCountInfo.SourceInfo;
			}
			else
			{
				language = counter.FileCountInfo.TargetInfo;
			}

			if (language.UsesCharacterCounts)
			{
				info.CountMethod = CountUnit.Character;
			}
			else
			{
				info.CountMethod = CountUnit.Word;
			}
		}
	}
}