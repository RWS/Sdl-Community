using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.AdvancedDisplayFilter.Helpers
{
	public class TagVisitor : IMarkupDataVisitor
	{
		private bool _containsTag;
		public bool ContainsTag(ISegment segment)
		{
			VisitChildren(segment);
			return _containsTag;
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (tagPair.StartTagProperties != null)
			{
				_containsTag = true;
				 
			}
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			if (tag.TagProperties != null)
			{
				_containsTag = true;
			}
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
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
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
				return;
			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}
	}
}
