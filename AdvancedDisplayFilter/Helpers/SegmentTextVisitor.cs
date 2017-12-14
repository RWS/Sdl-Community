using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public class SegmentTextVisitor : IMarkupDataVisitor
	{
		private readonly StringBuilder _textBuilder = new StringBuilder();
	
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

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
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

		public void VisitTagPair(ITagPair tagPair)
		{
			//foreach (var subItem in tagPair.AllSubItems)
			//{
			//	var test = subItem;
			//}
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{

		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			
		}

		

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			
		}
		
	}
}
