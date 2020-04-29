using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.AdvancedDisplayFilter.DisplayFilters;
using Sdl.Community.AdvancedDisplayFilter.Helpers;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.AdvancedDisplayFilter.Services
{
	public class QualitySamplingService
	{		
		public List<string> GetSamplingSegmentPairsIds(Document document, CustomFilterSettings customFilterSettings)
		{
			if (!customFilterSettings.QualitySamplingSegmentSelection &&
			    !customFilterSettings.QualitySamplingMinMaxCharacters)
			{
				return null;
			}

			var segmentPairs = document.SegmentPairs.ToList();

			if (segmentPairs.Count <= 0)
			{
				return null;
			}

			var segmentPairIds = new List<string>();

			if (customFilterSettings.QualitySamplingSegmentSelection)
			{
				if (customFilterSettings.QualitySamplingRandomlySelect)
				{
					// first get a qualified list, taking into consideration the min/max char range
					if (customFilterSettings.QualitySamplingMinMaxCharacters)
					{
						segmentPairs = GetQualifiedSegmentPairs(segmentPairs, customFilterSettings);
					}

					// get total number of segment to work with.
					var percentage = ((decimal)customFilterSettings.QualitySamplingRandomlySelectValue / 100);
					var segmentsCount = (int)Math.Round(segmentPairs.Count * percentage, 0);

					// select the segments randomly
					var segmentPairIndexes = new List<int>();
					if (segmentsCount == segmentPairs.Count)
					{
						for (var i = 0; i < segmentsCount; i++)
						{
							segmentPairIndexes.Add(i);
						}
					}

					for (var i = 0; i < segmentsCount; i++)
					{
						segmentPairIndexes.Add(RandomNumber(1, segmentPairs.Count + 1, segmentPairIndexes));
					}

					foreach (var segmentPairIndex in segmentPairIndexes)
					{
						var segmentPair = segmentPairs[segmentPairIndex - 1];
						var segmentPairId = GetSegmentPairId(segmentPair);
						segmentPairIds.Add(segmentPairId);
					}
				}
				else
				{
					var segmentPairGroups = new List<List<ISegmentPair>>();
					while (segmentPairs.Any())
					{
						segmentPairGroups.Add(segmentPairs.Take(customFilterSettings.QualitySamplingSelectOneInEveryValue).ToList());
						segmentPairs = segmentPairs.Skip(customFilterSettings.QualitySamplingSelectOneInEveryValue).ToList();
					}

					foreach (var segmentPairGroup in segmentPairGroups)
					{
						var qualifiedSegmentPairs = customFilterSettings.QualitySamplingMinMaxCharacters
							? GetQualifiedSegmentPairs(segmentPairGroup, customFilterSettings)
							: segmentPairGroup;

						if (!(qualifiedSegmentPairs?.Count > 0))
						{
							continue;
						}

						var segmentPairIndex = RandomNumber(1, qualifiedSegmentPairs.Count + 1);
						var segmentPair = qualifiedSegmentPairs[segmentPairIndex - 1];
						var segmentPairId = GetSegmentPairId(segmentPair);
						segmentPairIds.Add(segmentPairId);
					}
				}
			}
			else if (customFilterSettings.QualitySamplingMinMaxCharacters)
			{
				segmentPairs = GetQualifiedSegmentPairs(segmentPairs, customFilterSettings);

				foreach (var segmentPair in segmentPairs)
				{
					var segmentPairId = GetSegmentPairId(segmentPair);
					segmentPairIds.Add(segmentPairId);
				}
			}

			return segmentPairIds;

		}

		public string GetSegmentPairId(ISegmentPair segmentPair)
		{
			if (segmentPair == null)
			{
				return null;
			}

			var paragraphId = segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id;
			var segmentId = segmentPair.Properties.Id.Id;
			var segmentPairId = paragraphId + ";" + segmentId;
			return segmentPairId;
		}

		private static int RandomNumber(int min, int max, ICollection<int> currentValues = null)
		{
			var random = new Random();
			var value = random.Next(min, max);
			if (currentValues != null)
			{
				while (currentValues.Contains(value))
				{
					value = random.Next(min, max);
				}
			}

			return value;
		}

		private static List<ISegmentPair> GetQualifiedSegmentPairs(IEnumerable<ISegmentPair> segmentPairs, CustomFilterSettings customFilterSettings)
		{
			var availableSegmentPairs = new List<ISegmentPair>();
			var segmentVisitor = new SegmentTextVisitor();

			foreach (var segmentPair in segmentPairs)
			{
				var sourceText = segmentVisitor.GetText(segmentPair.Source);
				var targetText = segmentVisitor.GetText(segmentPair.Target);

				if (string.IsNullOrEmpty(targetText))
				{
					targetText = sourceText;
				}

				if (targetText.Length >= customFilterSettings.QualitySamplingMinCharsValue &&
					targetText.Length <= customFilterSettings.QualitySamplingMaxCharsValue)
				{
					availableSegmentPairs.Add(segmentPair);
				}
			}

			return availableSegmentPairs;
		}
	}
}
