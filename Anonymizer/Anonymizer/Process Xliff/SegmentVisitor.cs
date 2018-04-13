using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.Anonymizer.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class SegmentVisitor: IMarkupDataVisitor
	{
		private IDocumentItemFactory _factory;
		private ISegment _segment;
		private IPropertiesFactory _propertiesFactory;
		//	private List<string> _patterns = new List<string>{ @"([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+)", @"\b(?:\d[ -]*?){13,16}\b" };
		private List<string> _patterns = new List<string> { @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b", @"\b(?:\d[ -]*?){13,16}\b" };
		private StringBuilder _textBuilder = new StringBuilder();
		
		public void ReplaceText(ISegment segment, IDocumentItemFactory factory, IPropertiesFactory propertiesFactory)
		{
			_factory = factory;
			_segment = segment;
			_propertiesFactory = propertiesFactory;
			_textBuilder.Clear();
			VisitChildren(segment);
		}

		private string Anonymizer(string text)
		{
			foreach (var pattern in _patterns)
			{
				var regex = new Regex(pattern, RegexOptions.IgnoreCase);
				var result = regex.Replace(text, new MatchEvaluator(Process));
				if (!string.IsNullOrWhiteSpace(result))
				{
					return result;
				}
			}
			return string.Empty;
		}

		private static string Process(Match match)
		{
			var encryptedText = AnonymizeData.EncryptData(match.ToString(), "Andrea");
			return string.Concat("{", encryptedText, "}");

		}

		private bool ShouldAnonymize(string text)
		{
			foreach (var pattern in _patterns)
			{
				var regex = new Regex(pattern, RegexOptions.IgnoreCase);
				var match = regex.Match(text);
				if (match.Success)
				{
					return true;
				}
			}
			return false;
		}
		public void VisitTagPair(ITagPair tagPair)
		{
			if (tagPair.StartTagProperties != null)
			{
				var anonymizedText=Anonymizer(tagPair.StartTagProperties.TagContent);
				tagPair.StartTagProperties.TagContent = anonymizedText;
			}
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			
		}

		public void VisitText(IText text)
		{
			var shouldAnonymize = ShouldAnonymize(text.Properties.Text);
			if (shouldAnonymize)
			{
				var anonymizedText = Anonymizer(text.Properties.Text);

				_segment.Add(_factory.CreatePlaceholderTag(_propertiesFactory.CreatePlaceholderTagProperties(anonymizedText)));
			}
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
			foreach (var item in container.ToList())
			{
				item.AcceptVisitor(this);
			}
		}
	}
}
