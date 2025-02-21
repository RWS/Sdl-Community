using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace SDLTM.Import.FileService
{
	internal class TextCollector : IMarkupDataVisitor
	{
		private readonly StringBuilder _result = new StringBuilder();

		public static string CollectText(IAbstractMarkupData markupData)
		{
			var collector = new TextCollector();
			markupData.AcceptVisitor(collector);
			return collector.Result;
		}

		public string Result => _result.ToString();

		public void VisitChildren(IAbstractMarkupDataContainer container)
		{
			foreach (var item in container)
			{
				item.AcceptVisitor(this);
			}
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitChildren(commentMarker);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			// include any text inside locked content
			VisitChildren(lockedContent.Content);
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			// NOTE: not using the text equivalent
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			// don't include deleted content
			if (revisionMarker.Properties.RevisionType != RevisionType.Delete)
			{
				VisitChildren(revisionMarker);
			}
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
			_result.Append(text.Properties.Text);
		}
	}
}
