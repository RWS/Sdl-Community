using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class CleanUpHandler : IMarkupDataVisitor
	{
		private StringBuilder _textBuilder = new StringBuilder();
		public void VisitTagPair(ITagPair tagPair)
		{

		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{

		}

		public void VisitText(IText text)
		{
			_textBuilder.Append(text.Properties.Text);
		}
		public string GetText(ISegment segment)
		{
			_textBuilder.Clear();
			VisitChildren(segment);

			return _textBuilder.ToString();
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

		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{

		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
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
