using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Community.StudioViews.Common;
using Sdl.Community.StudioViews.Interfaces;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Providers;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.TranslationStudioAutomation.IntegrationApi.Extensions;
using SegmentPairInfo = Sdl.Community.StudioViews.Model.SegmentPairInfo;

namespace Sdl.Community.StudioViews.Services
{
	public class ContentImporter : AbstractBilingualContentProcessor
	{
		private readonly List<SegmentPairInfo> _updatedSegmentPairs;
		private readonly List<string> _excludeFilterIds;
		private readonly FilterItemService _filterItemService;
		private readonly ParagraphUnitProvider _paragraphUnitProvider;
		private readonly SegmentBuilder _segmentBuilder;

		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;

		public ContentImporter(List<SegmentPairInfo> updatedSegmentPairs, List<string> excludeFilterIds,
			FilterItemService filterItemService, ParagraphUnitProvider paragraphUnitProvider, SegmentBuilder segmentBuilder)
		{
			_updatedSegmentPairs = updatedSegmentPairs;
			_excludeFilterIds = excludeFilterIds;
			_filterItemService = filterItemService;
			_paragraphUnitProvider = paragraphUnitProvider;
			_segmentBuilder = segmentBuilder;

			UpdatedSegments = 0;
			ExcludedSegments = 0;
			TotalSegments = 0;
		}

		public int UpdatedSegments { get; private set; }

		public int ExcludedSegments { get; private set; }

		public int TotalSegments { get; private set; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		public override void Initialize(IDocumentProperties documentInfo)
		{
			_documentProperties = documentInfo;

			SourceLanguage = documentInfo.SourceLanguage.CultureInfo;
			TargetLanguage = documentInfo.TargetLanguage?.CultureInfo ?? SourceLanguage;

			base.Initialize(documentInfo);
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			_fileProperties = fileInfo;
			base.SetFileProperties(fileInfo);
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			var updatedSegmentPairs = _updatedSegmentPairs.Where(a => a.ParagraphUnitId == paragraphUnit.Properties.ParagraphUnitId.Id).ToList();

			TotalSegments += paragraphUnit.SegmentPairs.Count();

			if (updatedSegmentPairs.Any())
			{
				var mergeStatus = string.Empty;
				var mergedSegmentContent = string.Empty;
				var updatedMergedSegmentContent = string.Empty;
				var removeMergedIndexes = new List<int>();
				foreach (var segmentPair in paragraphUnit.SegmentPairs)
				{
					var updatedSegmentPair = updatedSegmentPairs.FirstOrDefault(a => a.SegmentId == segmentPair.Properties.Id.Id);
					if (updatedSegmentPair != null)
					{
						// if the entire paragraph was merged, then simply clear the source/target containers + update properties
						if (updatedSegmentPair.SegmentPair?.Properties?.TranslationOrigin?.OriginSystem == Constants.MergedParagraph)
						{
							segmentPair.Source.Clear();
							segmentPair.Target.Clear();

							segmentPair.Properties = updatedSegmentPair.SegmentPair?.Properties?.Clone() as ISegmentPairProperties;
							mergeStatus = Constants.MergedParagraph;
							continue;
						}

						if (updatedSegmentPair.SegmentPair?.Properties?.TranslationOrigin != null &&
							updatedSegmentPair.SegmentPair.Properties.TranslationOrigin.MetaDataContainsKey(Constants.MergeStatus))
						{
							mergeStatus = updatedSegmentPair.SegmentPair.Properties.TranslationOrigin.GetMetaData(Constants.MergeStatus);
							// track content from the segment that was merged, to understand what indexes should be removed
							// from the updated paragraph container...
							if (mergeStatus == Constants.MergedSegment)
							{
								mergedSegmentContent += segmentPair.Source.AsSimpleText().Replace(" ", string.Empty);
								updatedMergedSegmentContent = updatedSegmentPair.SegmentPair?.Source.AsSimpleText().Replace(" ", string.Empty);
							}
						}


						if (IsEmpty(updatedSegmentPair.SegmentPair?.Source) && !IsEmpty(segmentPair.Source) &&
							updatedSegmentPair.SegmentPair?.Properties?.TranslationOrigin?.OriginSystem == Constants.MergedParagraph)
						{
							UpdateSegmentPair(segmentPair, updatedSegmentPair);
							UpdatedSegments++;

							updatedSegmentPairs.Remove(updatedSegmentPair);
							continue;
						}

						var isExcluded = SegmentIsExcluded(_excludeFilterIds, segmentPair);
						if (isExcluded)
						{
							ExcludedSegments++;
							continue;
						}

						if (IsSame(segmentPair.Source, updatedSegmentPair.SegmentPair?.Source) &&
							IsSame(segmentPair.Target, updatedSegmentPair.SegmentPair?.Target))
						{
							continue;
						}

						UpdateSegmentPair(segmentPair, updatedSegmentPair);
						UpdatedSegments++;

						updatedSegmentPairs.Remove(updatedSegmentPair);
					}
					else if (mergeStatus == Constants.MergedSegment)
					{
						// identify if the segment index should be removed from the updated paragraph container
						// by comparing the length of the content that was merged
						mergedSegmentContent += segmentPair.Source.AsSimpleText().Replace(" ", string.Empty);
						if (mergedSegmentContent.Length <= updatedMergedSegmentContent.Length)
						{
							removeMergedIndexes.Add(segmentPair.Source.IndexInParent);
							segmentPair.Source.Clear();
							segmentPair.Target.Clear();
						}
					}
					else if (mergeStatus == Constants.MergedParagraph)
					{
						// remove all segments in the current paragraph container
						removeMergedIndexes.Add(segmentPair.Source.IndexInParent);
						segmentPair.Source.Clear();
						segmentPair.Target.Clear();
					}
				}

				// remove merged segment indexes from the paragraph container
				removeMergedIndexes.Reverse();
				foreach (var index in removeMergedIndexes)
				{
					paragraphUnit.Source.RemoveAt(index);
					paragraphUnit.Target.RemoveAt(index);
				}

				// identify the segment pairs that have been split and add them to the container, starting from the original segment pair index
				if (updatedSegmentPairs.Count > 0 && HasSplitSegments(updatedSegmentPairs.Select(a => a.SegmentPair)))
				{
					var segmentPairInfoGroups = GetSegmentPairInfoGroupsFromSpitSegments(updatedSegmentPairs).Reverse().ToList();
					foreach (var segmentPairInfoGroup in segmentPairInfoGroups)
					{
						var indexInParent = GetIndexInParent(paragraphUnit, segmentPairInfoGroup.Key, out var sourceParent, out var targetParent);
						if (sourceParent == null || targetParent == null)
						{
							continue;
						}

						sourceParent.RemoveAt(indexInParent);
						targetParent.RemoveAt(indexInParent);

						foreach (var segmentData in segmentPairInfoGroup.Value)
						{
							if (segmentData is SegmentPairInfo segmentPairInfo)
							{
								sourceParent.Insert(indexInParent, segmentPairInfo.SegmentPair.Source.Clone() as IAbstractMarkupData);
								targetParent.Insert(indexInParent, segmentPairInfo.SegmentPair.Target.Clone() as IAbstractMarkupData);

								indexInParent++;

								UpdatedSegments++;
							}

							
							if (!(segmentData is SegmentMarkupInfo segmentMarkupInfo))
							{
								continue;
							}

							// Add markup found between the split segment pairs
							for (var index = 0; index < segmentMarkupInfo.SourceMarkupData.Count; index++)
							{
								var sourceMarkupData = segmentMarkupInfo.SourceMarkupData[index];
								sourceParent.Insert(indexInParent, sourceMarkupData.Clone() as IAbstractMarkupData);

								// the markup 'should' be identical in the target container, therefore we can use the same index from the source
								var targetMarkupData = segmentMarkupInfo.TargetMarkupData[index];
								targetParent.Insert(indexInParent, targetMarkupData.Clone() as IAbstractMarkupData);

								indexInParent++;
							}
						}
					}
				}
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}

		private static int GetIndexInParent(IParagraphUnit paragraphUnit, string segmentId,
			out IAbstractMarkupDataContainer sourceParent, out IAbstractMarkupDataContainer targetParent)
		{
			sourceParent = null;
			targetParent = null;

			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				if (segmentId != segmentPair.Properties.Id.Id)
				{
					continue;
				}

				sourceParent = segmentPair.Source.Parent;
				targetParent = segmentPair.Target.Parent;

				return segmentPair.Source.IndexInParent;
			}

			return -1;
		}

		private static Dictionary<string, List<ISegmentInfo>> GetSegmentPairInfoGroupsFromSpitSegments(List<SegmentPairInfo> updatedSegmentPairs)
		{
			var groups = new Dictionary<string, List<ISegmentInfo>>();

			foreach (var segmentPairInfo in updatedSegmentPairs)
			{
				var parentSegmentId = segmentPairInfo.SegmentPair.Properties.Id.Id;
				if (parentSegmentId.Contains(" "))
				{
					parentSegmentId = parentSegmentId.Substring(0, parentSegmentId.LastIndexOf(" ", StringComparison.Ordinal));
				}


				var testSegmentId = parentSegmentId;
				while (testSegmentId.Contains(" "))
				{
					testSegmentId = testSegmentId.Substring(0, testSegmentId.LastIndexOf(" ", StringComparison.Ordinal));
					if (groups.ContainsKey(testSegmentId))
					{
						parentSegmentId = testSegmentId;
					}
				}


				if (groups.ContainsKey(parentSegmentId))
				{
					if (groups[parentSegmentId].Last() is SegmentPairInfo previousSegmentPairInfo)
					{
						var sourceParent = previousSegmentPairInfo.SegmentPair.Source.Parent;
						var targetParent = previousSegmentPairInfo.SegmentPair.Target.Parent;

						var sourceAbstractMarkupData = GetNonSegmentMarkupDataAfterSegment(sourceParent, previousSegmentPairInfo.SegmentPair.Source);
						var targetAbstractMarkupData = GetNonSegmentMarkupDataAfterSegment(targetParent, previousSegmentPairInfo.SegmentPair.Target);

						if (sourceAbstractMarkupData.Count > 0)
						{
							// Add the content between the split segments
							var segmentMarkupInfo = new SegmentMarkupInfo
							{
								SegmentId = previousSegmentPairInfo.SegmentId,
								FileId = previousSegmentPairInfo.FileId,
								ParagraphUnit = previousSegmentPairInfo.ParagraphUnit,
								ParagraphUnitId = previousSegmentPairInfo.ParagraphUnitId,
								SourceWordCounts = new WordCounts(),
								SourceMarkupData = sourceAbstractMarkupData,
								TargetMarkupData = targetAbstractMarkupData
							};

							groups[parentSegmentId].Add(segmentMarkupInfo);
						}
					}

					groups[parentSegmentId].Add(segmentPairInfo);
				}
				else
				{
					groups.Add(parentSegmentId, new List<ISegmentInfo> { segmentPairInfo });
				}
			}

			return groups;
		}

		private static List<IAbstractMarkupData> GetNonSegmentMarkupDataAfterSegment(IAbstractMarkupDataContainer segmentParent, ISegment segmentContainer)
		{
			var markupDataList = new List<IAbstractMarkupData>();
			var foundContainer = false;
			foreach (var markupData in segmentParent.AllSubItems)
			{
				if (markupData is ISegment segment)
				{
					if (foundContainer)
					{
						break;
					}

					if (segmentContainer.Properties.Id.Id == segment.Properties.Id.Id)
					{
						foundContainer = true;
					}
				}
				else if (foundContainer)
				{
					if (segmentContainer.Contains(markupData))
					{
						continue;
					}

					if (markupData is ITagPair)
					{
						break;
					}

					// Add to the list
					markupDataList.Add(markupData);
				}
			}

			return markupDataList;
		}

		private bool HasSplitSegments(IEnumerable<ISegmentPair> segmentPairs)
		{
			return segmentPairs.Select(segmentPair => segmentPair.Properties.Id.Id).Any(segmentId => segmentId.Contains(" "));
		}

		private static void UpdateSegmentPair(ISegmentPair segmentPair, SegmentPairInfo updatedSegmentPair)
		{
			segmentPair.Properties = updatedSegmentPair.SegmentPair.Properties.Clone() as ISegmentPairProperties;

			// Update the segment pair
			if (updatedSegmentPair.SegmentPair.Source != null)
			{
				segmentPair.Source.Clear();
				foreach (var markupDataItem in updatedSegmentPair.SegmentPair.Source)
				{
					segmentPair.Source.Add(markupDataItem.Clone() as IAbstractMarkupData);
				}
			}

			if (updatedSegmentPair.SegmentPair.Target != null)
			{
				segmentPair.Target.Clear();
				foreach (var markupDataItem in updatedSegmentPair.SegmentPair.Target)
				{
					segmentPair.Target.Add(markupDataItem.Clone() as IAbstractMarkupData);
				}
			}
		}

		private static bool IsEmpty(ISegment segment)
		{
			return segment.ToString().Trim() == string.Empty;
		}

		private bool SegmentIsExcluded(List<string> excludeFilterIds, ISegmentPair segmentPair)
		{
			var segmentIsExcluded = false;
			if (segmentPair != null && excludeFilterIds?.Count > 0)
			{
				var status = segmentPair.Properties.ConfirmationLevel.ToString();
				var match = _filterItemService.GetTranslationOriginType(
					segmentPair.Target.Properties.TranslationOrigin);

				if ((segmentPair.Properties.IsLocked && excludeFilterIds.Exists(a => a == "Locked"))
					|| excludeFilterIds.Exists(a => a == status)
					|| excludeFilterIds.Exists(a => a == match))
				{
					segmentIsExcluded = true;
				}
			}

			return segmentIsExcluded;
		}

		private static bool IsSame(ISegment segment, ISegment updatedSegment)
		{
			var originalTarget = segment?.ToString();
			var updatedTarget = updatedSegment?.ToString();

			var isSame = (originalTarget == updatedTarget) &&
						 (segment?.Properties.IsLocked == updatedSegment?.Properties.IsLocked) &&
						 (segment?.Properties.ConfirmationLevel == updatedSegment?.Properties.ConfirmationLevel);

			return isSame;
		}

	}
}
