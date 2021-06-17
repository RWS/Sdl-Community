﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.StudioViews.Extensions;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.StudioViews.Providers
{
	public class ParagraphUnitProvider
	{
		private readonly SegmentVisitor _segmentVisitor;
		private readonly FilterItemService _filterItemService;
		private readonly SegmentBuilder _segmentBuilder;

		public ParagraphUnitProvider(SegmentVisitor segmentVisitor, FilterItemService filterItemService)
		{
			_segmentVisitor = segmentVisitor;
			_filterItemService = filterItemService;
			_segmentBuilder = new SegmentBuilder();
		}

		public ParagraphUnitResult GetUpdatedParagraphUnit(IParagraphUnit paragraphUnitLeft, IParagraphUnit paragraphUnitRight,
			List<string> excludeFilterIds)
		{
			if (paragraphUnitLeft == null)
			{
				return null;
			}

			var result = new ParagraphUnitResult
			{
				Paragraph = paragraphUnitLeft.Clone() as IParagraphUnit
			};
			if (result.Paragraph == null)
			{
				return null;
			}
			
			result.Paragraph.Properties = paragraphUnitRight.Properties.Clone() as IParagraphUnitProperties;
			result.Paragraph.Source.Clear();
			result.Paragraph.Target.Clear();

			var alignments = GetSegmentPairAlignments(paragraphUnitLeft, paragraphUnitRight);
			foreach (var alignmentInfo in alignments)
			{
				switch (alignmentInfo.Alignment)
				{
					case AlignmentInfo.AlignmentType.Added:
					case AlignmentInfo.AlignmentType.Matched:
						var isExcluded = SegmentIsExcluded(excludeFilterIds, alignmentInfo.SegmentPairLeft);
						if (isExcluded)
						{
							result.ExcludedSegments++;
							result.Paragraph.Source.AddSegment(alignmentInfo.SegmentPairLeft.Source, _segmentBuilder);
							result.Paragraph.Target.AddSegment(alignmentInfo.SegmentPairLeft.Target, _segmentBuilder);
						}
						else
						{
							if (!IsSame(alignmentInfo.SegmentPairLeft?.Target, alignmentInfo.SegmentPairRight.Target))
							{
								result.UpdatedSegments++;
							}

							result.Paragraph.Source.AddSegment(alignmentInfo.SegmentPairRight.Source, _segmentBuilder);
							result.Paragraph.Target.AddSegment(alignmentInfo.SegmentPairRight.Target, _segmentBuilder);
						}
						break;
					case AlignmentInfo.AlignmentType.LeftOnly:
						result.Paragraph.Source.AddSegment(alignmentInfo.SegmentPairLeft.Source, _segmentBuilder);
						result.Paragraph.Target.AddSegment(alignmentInfo.SegmentPairLeft.Target, _segmentBuilder);
						break;
				}
			}

			return result;
		}

		public List<AlignmentInfo> GetSegmentPairAlignments(IParagraphUnit paragraphUnitLeft, IParagraphUnit paragraphUnitRight)
		{
			var alignments = new List<AlignmentInfo>();

			foreach (var segmentPairLeft in paragraphUnitLeft.SegmentPairs)
			{
				var segmentIdLeft = segmentPairLeft.Properties.Id.Id;
				var segmentPairRight = paragraphUnitRight.SegmentPairs.FirstOrDefault(
					a => a.Properties.Id.Id == segmentIdLeft);

				if (segmentPairRight == null)
				{
					segmentPairRight = paragraphUnitRight.SegmentPairs.FirstOrDefault(
						a => a.Properties.Id.Id.StartsWith(segmentIdLeft));

					if (segmentPairRight == null)
					{
						// The alignment of segments merged from the left can only be determined after 
						// identifying relative segments from the right; make reference to NormalizeAlignment
						alignments.Add(new AlignmentInfo
						{
							SortId = GetSortId(segmentIdLeft),
							SegmentId = segmentIdLeft,
							Alignment = AlignmentInfo.AlignmentType.None,
							SegmentPairLeft = segmentPairLeft,
							SegmentPairRight = null
						});
					}
					else
					{
						alignments.Add(new AlignmentInfo
						{
							SortId = GetSortId(segmentIdLeft),
							SegmentId = segmentIdLeft,
							Alignment = AlignmentInfo.AlignmentType.Removed,
							SegmentPairLeft = segmentPairLeft,
							SegmentPairRight = segmentPairRight
						});
					}
				}
				else
				{
					alignments.Add(new AlignmentInfo
					{
						SortId = GetSortId(segmentIdLeft),
						SegmentId = segmentIdLeft,
						Alignment = AlignmentInfo.AlignmentType.Matched,
						SegmentPairLeft = segmentPairLeft,
						SegmentPairRight = segmentPairRight
					});
				}
			}

			foreach (var segmentPairRight in paragraphUnitRight.SegmentPairs)
			{
				var segmentId = segmentPairRight.Properties.Id.Id;
				var segmentPairLeft = paragraphUnitLeft.SegmentPairs.FirstOrDefault(
					a => a.Properties.Id.Id == segmentPairRight.Properties.Id.Id);

				if (segmentPairLeft == null)
				{
					alignments.Add(new AlignmentInfo
					{
						SortId = GetSortId(segmentId),
						SegmentId = segmentId,
						Alignment = AlignmentInfo.AlignmentType.Added,
						SegmentPairLeft = null,
						SegmentPairRight = segmentPairRight
					});
				}
			}

			return NormalizeAlignment(alignments.OrderBy(a => a.SortId).ToList());
		}

		private List<AlignmentInfo> NormalizeAlignment(List<AlignmentInfo> alignments)
		{
			if (!alignments.Exists(a => a.Alignment == AlignmentInfo.AlignmentType.None))
			{
				return alignments;
			}

			var sourceRight = string.Empty;
			var sourceLeft = string.Empty;

			foreach (var alignmentInfo in alignments)
			{
				if (alignmentInfo.Alignment == AlignmentInfo.AlignmentType.Matched ||
					alignmentInfo.Alignment == AlignmentInfo.AlignmentType.Removed ||
					alignmentInfo.Alignment == AlignmentInfo.AlignmentType.LeftOnly ||
					alignmentInfo.Alignment == AlignmentInfo.AlignmentType.None)
				{
					_segmentVisitor.VisitSegment(alignmentInfo.SegmentPairLeft.Source);
					sourceLeft += RemoveWhiteSpaces(_segmentVisitor.Text);

					if (alignmentInfo.Alignment == AlignmentInfo.AlignmentType.None)
					{
						alignmentInfo.Alignment = sourceLeft.Length <= sourceRight.Length
							? AlignmentInfo.AlignmentType.Removed
							: AlignmentInfo.AlignmentType.LeftOnly;
					}
				}

				if (alignmentInfo.Alignment == AlignmentInfo.AlignmentType.Added ||
					alignmentInfo.Alignment == AlignmentInfo.AlignmentType.Matched)
				{
					_segmentVisitor.VisitSegment(alignmentInfo.SegmentPairRight.Source);
					sourceRight += RemoveWhiteSpaces(_segmentVisitor.Text);
				}

				if (alignmentInfo.Alignment == AlignmentInfo.AlignmentType.LeftOnly)
				{
					sourceRight += RemoveWhiteSpaces(_segmentVisitor.Text);
				}
			}

			return alignments;
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

		private static string RemoveWhiteSpaces(string input)
		{
			var output = Regex.Replace(input, @"\s+", string.Empty);
			return output;
		}

		private static string GetSortId(string segmentId)
		{
			if (segmentId.Contains(" "))
			{
				var segmentNumber = segmentId.Substring(0, segmentId.IndexOf(" ", StringComparison.Ordinal));
				var segmentSplitInfo = segmentId.Substring(segmentId.IndexOf(" ", StringComparison.Ordinal));
				return segmentNumber.PadLeft(6, '0') + segmentSplitInfo;
			}

			return segmentId.PadLeft(6, '0');
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

		private static bool IsEmpty(ISegment segment)
		{
			return segment == null || segment.ToString().Trim() == string.Empty;
		}
	}
}
