using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Multilingual.Excel.FileType.BatchTasks.Settings;
using Multilingual.Excel.FileType.Extensions;
using Multilingual.Excel.FileType.Models;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Services
{
	public class ParagraphUnitProvider
	{
		private readonly SegmentVisitor _segmentVisitor;
		private readonly FilterItemService _filterItemService;
		private readonly SegmentBuilder _segmentBuilder;

		public ParagraphUnitProvider(SegmentVisitor segmentVisitor, FilterItemService filterItemService, SegmentBuilder segmentBuilder)
		{
			_segmentVisitor = segmentVisitor;
			_filterItemService = filterItemService;
			_segmentBuilder = segmentBuilder;
		}

		public ParagraphUnitResult GetUpdatedParagraphUnit(IParagraphUnit paragraphUnitLeft, IParagraphUnit paragraphUnitRight,
			MultilingualExcelImportSettings settings)
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

			var sourceContainer = GetContainer(paragraphUnitLeft.Source, result.Paragraph.Source);
			var targetContainer = GetContainer(paragraphUnitLeft.Target, result.Paragraph.Target);

			foreach (var alignmentInfo in alignments)
			{
				switch (alignmentInfo.Alignment)
				{
					case AlignmentInfo.AlignmentType.Added:
					case AlignmentInfo.AlignmentType.Matched:
						var isExcluded = SegmentIsExcluded(settings, alignmentInfo.SegmentPairLeft, alignmentInfo.SegmentPairRight);
						if (isExcluded)
						{
							result.ExcludedSegments++;
							sourceContainer.AddSegment(alignmentInfo.SegmentPairLeft.Source, _segmentBuilder);
							targetContainer.AddSegment(alignmentInfo.SegmentPairLeft.Target, _segmentBuilder);
						}
						else
						{
							if (!IsSame(alignmentInfo.SegmentPairLeft?.Target, alignmentInfo.SegmentPairRight.Target))
							{
								result.UpdatedSegments++;

								// update the TranslationOrigin
								if (alignmentInfo.SegmentPairRight.Properties.TranslationOrigin != null)
								{
									var currentTranslationOrigin = (ITranslationOrigin)alignmentInfo.SegmentPairRight.Properties.TranslationOrigin.Clone();
									alignmentInfo.SegmentPairRight.Properties.TranslationOrigin.OriginBeforeAdaptation = currentTranslationOrigin;
								}
								else
								{
									alignmentInfo.SegmentPairRight.Properties.TranslationOrigin = _segmentBuilder.CreateTranslationOrigin();
								}
								
								SetTranslationOrigin(alignmentInfo.SegmentPairRight.Target, settings.OriginSystem);

								if (!string.IsNullOrEmpty(settings.StatusTranslationUpdatedId))
								{
									var success = Enum.TryParse<ConfirmationLevel>(settings.StatusTranslationUpdatedId, true, out var confirmationLevel);
									var statusTranslationUpdated = success
										? confirmationLevel
										: alignmentInfo.SegmentPairRight.Properties.ConfirmationLevel;

									alignmentInfo.SegmentPairRight.Target.Properties.ConfirmationLevel = statusTranslationUpdated;
								}

								alignmentInfo.SegmentPairRight.Target.Properties.IsLocked = alignmentInfo.SegmentPairRight.Properties.IsLocked;
							}

							sourceContainer.AddSegment(alignmentInfo.SegmentPairRight.Source, _segmentBuilder);
							targetContainer.AddSegment(alignmentInfo.SegmentPairRight.Target, _segmentBuilder);
						}
						break;
					case AlignmentInfo.AlignmentType.LeftOnly:
						sourceContainer.AddSegment(alignmentInfo.SegmentPairLeft.Source, _segmentBuilder);
						targetContainer.AddSegment(alignmentInfo.SegmentPairLeft.Target, _segmentBuilder);
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

		private bool SegmentIsExcluded(MultilingualExcelImportSettings settings, ISegmentPair segmentPair, ISegmentPair updatedSegmentPair)
		{
			if (segmentPair != null)
			{
				if (settings.ExcludeFilterIds?.Count > 0)
				{
					var status = segmentPair.Properties.ConfirmationLevel.ToString();
					var match = _filterItemService.GetTranslationOriginType(
						segmentPair.Target.Properties.TranslationOrigin);

					if ((segmentPair.Properties.IsLocked && settings.ExcludeFilterIds.Exists(a => a == "Locked"))
					    || settings.ExcludeFilterIds.Exists(a => a == status)
					    || settings.ExcludeFilterIds.Exists(a => a == match))
					{
						return true;
					}
				}

				if (!settings.OverwriteTranslations && !IsSame(segmentPair.Target, updatedSegmentPair?.Target))
				{
					_segmentVisitor.VisitSegment(segmentPair.Target);
					if (!string.IsNullOrEmpty(_segmentVisitor.Text))
					{
						return true;
					}
				}
			}

			return false;
		}

		private void SetTranslationOrigin(ISegment targetSegment, string originSystem)
		{
			targetSegment.Properties.TranslationOrigin.MatchPercent = byte.Parse("0");
			targetSegment.Properties.TranslationOrigin.OriginSystem = originSystem;
			targetSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.Interactive;
			targetSegment.Properties.TranslationOrigin.IsStructureContextMatch = false;
			targetSegment.Properties.TranslationOrigin.TextContextMatchLevel = TextContextMatchLevel.None;

			targetSegment.Properties.TranslationOrigin.SetMetaData("last_modified_by", originSystem);
			targetSegment.Properties.TranslationOrigin.SetMetaData("modified_on", FormatAsInvariantDateTime(DateTime.UtcNow));
		}

		private string FormatAsInvariantDateTime(DateTime date)
		{
			return date.ToString(DateTimeFormatInfo.InvariantInfo);
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

		private bool IsSame(ISegment segment, ISegment updatedSegment)
		{
			var originalTarget = segment?.ToString();
			var updatedTarget = updatedSegment?.ToString();

			var isSame = (originalTarget == updatedTarget) &&
						 (segment?.Properties.IsLocked == updatedSegment?.Properties.IsLocked) &&
						 (segment?.Properties.ConfirmationLevel == updatedSegment?.Properties.ConfirmationLevel);

			return isSame;
		}

		private static bool IsEmpty(IEnumerable segment)
		{
			return segment == null || segment.ToString().Trim() == string.Empty;
		}

		private static IAbstractMarkupDataContainer GetContainer(IAbstractMarkupDataContainer paragraph, IAbstractMarkupDataContainer container)
		{
			var result = container;
			foreach (var abstractMarkupData in paragraph.AllSubItems)
			{
				if (abstractMarkupData is ISegment)
				{
					break;
				}

				if (abstractMarkupData.Clone() is ITagPair tagPair)
				{
					tagPair.Clear();
					container.Add(tagPair);

					result = tagPair;
				}
			}

			return result;
		}
	}
}
