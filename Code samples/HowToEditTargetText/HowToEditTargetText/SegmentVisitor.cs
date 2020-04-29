using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace HowToEditTargetText
{
	public class SegmentVisitor : IMarkupDataVisitor
	{  
		public void AddText(ISegment segment)
		{
			VisitChildren(segment);	
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
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
		}

		public void VisitText(IText text)
		{
			text.Properties.Text = text + " added from code";
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
	}
}
