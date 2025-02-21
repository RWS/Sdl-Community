using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.Excel.FileType.Services
{
	public class SegmentVisitorReplaceText : IMarkupDataVisitor
	{

		private string _from;
		private string _to;

		public void ReplaceText(string from, string to, ISegment segment)
		{
			_from = from;
			_to = to;

			VisitChilderen(segment);
		}


		public void VisitSegment(ISegment segment)
		{
			VisitChilderen(segment);
		}

		private void VisitChilderen(IAbstractMarkupDataContainer container)
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

		public void VisitTagPair(ITagPair tagPair)
		{
			VisitChilderen(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag placeholderTag)
		{
		}

		public void VisitText(IText text)
		{
			text.Properties.Text = text.Properties.Text.Replace(_from, _to);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			// not used in this implementation
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitChilderen(commentMarker);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChilderen(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			VisitChilderen(lockedContent.Content);
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			if (revisionMarker.Properties.RevisionType == RevisionType.Insert
			    || revisionMarker.Properties.RevisionType == RevisionType.FeedbackAdded)
			{
				VisitChilderen(revisionMarker);
			}
		}
	}
}
