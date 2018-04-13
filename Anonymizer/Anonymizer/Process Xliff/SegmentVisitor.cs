using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		public void ReplaceText(ISegment segment, IDocumentItemFactory factory, IPropertiesFactory propertiesFactory)
		{
			_factory = factory;
			_segment = segment;
			_propertiesFactory = propertiesFactory;
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

		private List<AnonymizedData> GetAnonymizedData(string segmentText)
		{
			var anonymizedData = new List<AnonymizedData>();
			foreach (var pattern in _patterns)
			{
				var regex = new Regex(pattern, RegexOptions.IgnoreCase);
				var matches = regex.Matches(segmentText);
				foreach (Match match in matches)
				{
					var data = new AnonymizedData
					{
						MatchText = match.Value,
						PositionInOriginalText = match.Index,
						EncryptedText = AnonymizeData.EncryptData(match.ToString(), "Andrea")
					};
					anonymizedData.Add(data);
				}
			}
			return anonymizedData;
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (tagPair.StartTagProperties != null)
			{
				var anonymizedText = Anonymizer(tagPair.StartTagProperties.TagContent);
				tagPair.StartTagProperties.TagContent = anonymizedText;
			}
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			
		}

		public void VisitText(IText text)
		{
			var markUp = new List<IAbstractMarkupData>();
			var segmentText = new List<IText>();
			//var test = text.Split(1);
			var shouldAnonymize = ShouldAnonymize(text.Properties.Text);
			var indexInParent = text.IndexInParent;
			if (shouldAnonymize)
			{
				//var anonymizedData = GetAnonymizedData(text.Properties.Text);

				//for (var i = 0; i < anonymizedData.Count; i++)
				//{
				//	//that means is first time we split the original text
				//	if (segmentText.Count.Equals(0))
				//	{
				//		if (anonymizedData[i].PositionInOriginalText.Equals(0))
				//		{
				//			var subText = text;
				//			//var subText = text.Split(anonymizedData[i].MatchText.Length);
				//			var shouldAnonymizeFirstPosition = ShouldAnonymize(text.Properties.Text);
				//			if (shouldAnonymizeFirstPosition)
				//			{
				//				subText.Properties.Text = Anonymizer(text.Properties.Text);
				//				//text.Properties.Text = Anonymizer(text.Properties.Text);
				//			}
				//			segmentText.Add(subText);
				//			var tag = _factory.CreatePlaceholderTag(_propertiesFactory.CreatePlaceholderTagProperties(subText.Properties.Text));
				//			markUp.Add(tag);
				//		}
				//		else
				//		{
				//			var subText = text.Split(anonymizedData[i].PositionInOriginalText);
				//			segmentText.Add(subText);
				//			var tag = _factory.CreatePlaceholderTag(_propertiesFactory.CreatePlaceholderTagProperties(subText.Properties.Text));
				//			markUp.Add(tag);
				//		}
						
				//	}
				//	else
				//	{
				//		var subText = segmentText[i-1].Split(anonymizedData[i-1].PositionInOriginalText);
				//		subText.Properties.Text = anonymizedData[i - 1].EncryptedText;
				//		segmentText.Add(subText);
				//		var tag = _factory.CreatePlaceholderTag(_propertiesFactory.CreatePlaceholderTagProperties(subText.Properties.Text));
				//		markUp.Add(tag);
				//	}
				//}
				var abstractMarkupData = text.Parent.AllSubItems.FirstOrDefault(n => n.Equals(text));
				if (abstractMarkupData != null)
				{
					//creed un tag de proba
					var tag = _factory.CreatePlaceholderTag(
						_propertiesFactory.CreatePlaceholderTagProperties(Anonymizer(text.Properties.Text)));
					var elementContainer = abstractMarkupData.Parent;
					elementContainer.Add(tag);
					elementContainer.RemoveAt(indexInParent);
					//foreach (var mark in markUp)
					//{
					//	elementContainer.Add(mark);
					//}
				}
				//	element = markUp;
				
				//var anonymizedText = Anonymizer(text.Properties.Text);

				//_segment.Add(_factory.CreatePlaceholderTag(_propertiesFactory.CreatePlaceholderTagProperties(anonymizedText)));
			}
			//foreach (var subSegment in segmentText)
			//{
			//	text.Properties.Text = text.Properties.Text + subSegment.Properties.Text;
			//}
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
