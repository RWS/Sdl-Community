using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.StudioViews.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.StudioViews.Services
{
	public class ParagraphUnitProvider
	{
		private readonly SegmentVisitor _segmentVisitor;

		public ParagraphUnitProvider(SegmentVisitor segmentVisitor)
		{
			_segmentVisitor = segmentVisitor;
		}

		public IParagraphUnit GetUpdatedParagraphUnit(IParagraphUnit paragraphUnitLeft, IParagraphUnit paragraphUnitRight)
		{
			if (!(paragraphUnitLeft.Clone() is IParagraphUnit paragraphUnit))
			{
				return null;
			}

			var sourceLeft = GetSourceContent(paragraphUnitLeft);
			var sourceRight = GetSourceContent(paragraphUnitRight);

			// TODO identify when the source content/text has been updated (other than structural changes)
			//if (sourceLeft != sourceRight)
			//{
			//	throw new Exception("Source content does not match!");
			//}

			paragraphUnit.Properties = paragraphUnitRight.Properties;

			paragraphUnit.Source.Clear();
			paragraphUnit.Target.Clear();

			var alignments = GetSegmentPairAlignment(paragraphUnitLeft, paragraphUnitRight);
			foreach (var alignmentInfo in alignments)
			{
				switch (alignmentInfo.Alignment)
				{
					case AlignmentInfo.AlignmentType.Added:
					case AlignmentInfo.AlignmentType.Matched:
						foreach (var segment in alignmentInfo.SegmentPairRight.Source)
						{
							paragraphUnit.Source.Add(segment.Clone() as IAbstractMarkupData);
						}
						foreach (var segment in alignmentInfo.SegmentPairRight.Target)
						{
							paragraphUnit.Target.Add(segment.Clone() as IAbstractMarkupData);
						}
						break;
					case AlignmentInfo.AlignmentType.LeftOnly:
						foreach (var segment in alignmentInfo.SegmentPairLeft.Source)
						{
							paragraphUnit.Source.Add(segment.Clone() as IAbstractMarkupData);
						}
						foreach (var segment in alignmentInfo.SegmentPairLeft.Target)
						{
							paragraphUnit.Target.Add(segment.Clone() as IAbstractMarkupData);
						}
						break;
				}
			}

			return paragraphUnit;
		}

		private string GetSourceContent(IParagraphUnit paragraphUnitLeft)
		{
			var sourceLeft = string.Empty;
			foreach (var segmentPair in paragraphUnitLeft.SegmentPairs)
			{
				_segmentVisitor.VisitSegment(segmentPair.Source);
				sourceLeft += _segmentVisitor.Text;
			}

			return sourceLeft;
		}

		public List<AlignmentInfo> GetSegmentPairAlignment(IParagraphUnit paragraphUnitLeft, IParagraphUnit paragraphUnitRight)
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

			alignments = alignments.OrderBy(a => a.SortId).ToList();

			NormalizeAlignment(alignments);

			return alignments;
		}

		private void NormalizeAlignment(List<AlignmentInfo> alignments)
		{
			if (!alignments.Exists(a => a.Alignment == AlignmentInfo.AlignmentType.None))
			{
				return;
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
					sourceLeft += _segmentVisitor.Text;

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
					sourceRight += _segmentVisitor.Text;
				}

				if (alignmentInfo.Alignment == AlignmentInfo.AlignmentType.LeftOnly)
				{
					sourceRight += _segmentVisitor.Text;
				}
			}
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

		public List<ParagraphUnitInfo> GroupSegmentPairs(IEnumerable<SegmentPairInfo> updatedSegmentPairs)
		{
			var paragraphUnits = new List<ParagraphUnitInfo>();
			foreach (var segmentPair in updatedSegmentPairs)
			{
				var paragraphUnit = paragraphUnits.FirstOrDefault(
					a => a.ParagraphUnit.Properties.ParagraphUnitId.Id == segmentPair.ParagraphUnitId);

				if (paragraphUnit != null)
				{
					paragraphUnit.SegmentPairs.Add(segmentPair);
				}
				else
				{
					var paragraphUnitInfo = new ParagraphUnitInfo
					{
						ParagraphUnit = segmentPair.ParagraphUnit,
						SegmentPairs = new List<SegmentPairInfo> { segmentPair },
						FileId = segmentPair.FileId
					};
					paragraphUnits.Add(paragraphUnitInfo);
				}
			}

			return paragraphUnits;
		}

	}
}
