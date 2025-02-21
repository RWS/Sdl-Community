using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SDLBatchAnonymize.Visitor
{
	public class CommentVisitor : IMarkupDataVisitor
	{
		private string _commentAuthor;

		public void AnonymizeCommentAuthor(ISegment segment,string commentAuthor)
		{
			_commentAuthor = commentAuthor;
			VisitChildren(segment);
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			foreach (var commentProperty in commentMarker.Comments.Comments)
			{
				commentProperty.Author = _commentAuthor;
			}
		}
		public void VisitTagPair(ITagPair tagPair)
		{
			VisitChildren(tagPair);
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

		public void VisitOtherMarker(IOtherMarker marker)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
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
