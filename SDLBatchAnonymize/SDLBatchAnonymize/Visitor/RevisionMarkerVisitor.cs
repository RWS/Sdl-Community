using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SDLBatchAnonymize.Visitor
{
	public class RevisionMarkerVisitor : IMarkupDataVisitor
	{
		private string _revisionAuthor;

		public void AnonymizeRevisionMarker(ISegment segment, string revisionAuthor)
		{
			_revisionAuthor = revisionAuthor;
			VisitChildren(segment);
		}
		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			revisionMarker.Properties.Author = _revisionAuthor;
		}

		public void VisitTagPair(ITagPair tagPair)
		{
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
		}

		public void VisitText(IText text)
		{
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitChildren(commentMarker);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			if (container == null)
			{
				return;
			}

			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}
	}
}
