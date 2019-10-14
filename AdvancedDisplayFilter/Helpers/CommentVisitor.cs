using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.AdvancedDisplayFilter.Helpers
{
	public class CommentDataVisitor : IMarkupDataVisitor
	{
		private readonly List<IComment> _comments;

		public CommentDataVisitor()
		{
			_comments = new List<IComment>();
		}
		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			foreach (var commentProperty in commentMarker.Comments.Comments)
			{
				_comments.Add(commentProperty);
			}
			VisitChildren(commentMarker);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			// Not required for this implementation
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			// Not required for this implementation
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			// Not required for this implementation
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
			// Not required for this implementation
			VisitChildren(tagPair);
		}

		public void VisitText(IText text)
		{
			// Not required for this implementation
		}

		public List<IComment> GetComments(ISegment segment)
		{
			_comments.Clear();
			VisitChildren(segment);
			return _comments;
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			if (container == null)
				return;
			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}
	}
}
