using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace DeleteCommentsExample
{
	public class SegmentVisitor : IMarkupDataVisitor
	{
		private List<ICommentMarker> CommentMarker { get; set; } = new();

		public List<ICommentMarker> GetCommentMarkers(ISegment segment)
		{
			VisitSegment(segment);

			var commentMarkers = CommentMarker;
			CommentMarker = new List<ICommentMarker>();

			return commentMarkers;
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			CommentMarker.Add(commentMarker);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
		{
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			VisitChildren(revisionMarker);
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			VisitChildren(tagPair);
		}

		public void VisitText(IText text)
		{
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}
	}
}