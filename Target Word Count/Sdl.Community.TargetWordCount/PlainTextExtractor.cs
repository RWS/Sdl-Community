using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Text;

namespace Sdl.Community.TargetWordCount
{
	internal class PlainTextExtractor : IMarkupDataVisitor
	{
		private StringBuilder plainText;

		public string GetPlainText(ISegment segment)
		{
			plainText = new StringBuilder("");

			VisitChildren(segment);

			return plainText.ToString();
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
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
			string segContent = text.Properties.Text;
			plainText.Append(text);
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