using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class SegmentVisitor: AbstractBilingualContentProcessor,IMarkupDataVisitor
	{
		public void ReplaceText(ISegment segment)
		{
			
			segment.Insert(0, ItemFactory.CreateText(
				PropertiesFactory.CreateTextProperties("Andrea")));
			//segment.
			//VisitChildren(segment);
		}
		public void VisitTagPair(ITagPair tagPair)
		{
			
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			
		}

		public void VisitText(IText text)
		{
			text.Properties.Text = "test";
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
