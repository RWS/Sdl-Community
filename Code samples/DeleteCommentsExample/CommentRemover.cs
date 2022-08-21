using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace DeleteCommentsExample
{
	public class CommentRemover : AbstractBilingualContentProcessor
	{
		public CommentRemover(SegmentVisitor segmentVisitor, TextCoordinates startCoordinates, TextCoordinates endCoordinates)
		{
			SegmentVisitor = segmentVisitor;
			StartCoordinates = startCoordinates;
			EndCoordinates = endCoordinates;
		}

		private TextCoordinates EndCoordinates { get; }
		private SegmentVisitor SegmentVisitor { get; }
		private TextCoordinates StartCoordinates { get; }

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);

			if (paragraphUnit.IsStructure) return;

			var targetSegments = paragraphUnit.SegmentPairs
				.Select(sp => sp.Target)
				.Where
					(
						ts => int.Parse(ts.Properties.Id.ToString()) >= StartCoordinates.SegmentId && int.Parse(ts.Properties.Id.ToString()) <= EndCoordinates.SegmentId
					)
				.ToList();

			foreach (var segment in targetSegments)
			{
				foreach (var cm in SegmentVisitor.GetCommentMarkers(segment))
				{
					var segmentId = int.Parse(segment.Properties.Id.Id);

					var cmStartPositionInSegment = new TextCoordinates
					{
						CharNumber = segment.ToString().IndexOf(cm.ToString()),
						SegmentId = segmentId
					};

					var cmEndPositionInSegment = new TextCoordinates
					{
						CharNumber = cmStartPositionInSegment.CharNumber + cm.ToString().Length - 1,
						SegmentId = segmentId
					};

					if (IsElementInRange(cmStartPositionInSegment, cmEndPositionInSegment)) DeleteCommentMarker(cm);
				}
			}
		}

		private void DeleteCommentMarker(ICommentMarker cm)
		{
			var position = cm.IndexInParent;

			var parent = cm.Parent;
			parent.Remove(cm);

			foreach (var item in cm.ToList())
			{
				parent.Insert(position, (IAbstractMarkupData)item.Clone());
				position++;
			}
		}

		private bool IsElementInRange(TextCoordinates startPosition, TextCoordinates endPosition)
		{
			if (startPosition.SegmentId >= StartCoordinates.SegmentId && startPosition.CharNumber >= StartCoordinates.CharNumber) return true;
			if (endPosition.SegmentId <= EndCoordinates.SegmentId && endPosition.CharNumber <= EndCoordinates.CharNumber) return true;

			return true;
		}
	}
}