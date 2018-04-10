using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class SegmentVisitor: AbstractBilingualContentProcessor,IMarkupDataVisitor
	{
		private IDocumentItemFactory _factory;
		//	private List<string> _patterns = new List<string>{ @"([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+)", @"\b(?:\d[ -]*?){13,16}\b" };
		private List<string> _patterns = new List<string> { @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b", @"\b(?:\d[ -]*?){13,16}\b" };
		public void ReplaceText(ISegment segment, IDocumentItemFactory factory)
		{
			_factory = factory;
			VisitChildren(segment);
		}

		private void CheckForPersonalData(string text)
		{
			foreach (var pattern in _patterns)
			{
				var regex = new Regex(pattern,RegexOptions.IgnoreCase);
				var result = regex.Replace(text, new MatchEvaluator(Anonymize));
			}
		}
		private string Anonymize(Match m)
		{
			var x = AnonymizeData.EncryptData(m.ToString(), "Andrea");
			return x;
		}
		public void VisitTagPair(ITagPair tagPair)
		{
			if (tagPair.StartTagProperties != null)
			{
				CheckForPersonalData(tagPair.StartTagProperties.TagContent);
				
			}
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			
		}

		public void VisitText(IText text)
		{
			//text.Properties.Text = "test";
			CheckForPersonalData(text.Properties.Text);
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
