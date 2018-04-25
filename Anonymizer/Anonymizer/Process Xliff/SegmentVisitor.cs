using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.projectAnonymizer.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.projectAnonymizer.Process_Xliff
{
	public class SegmentVisitor: IMarkupDataVisitor
	{
		private IDocumentItemFactory _factory;
		private IPropertiesFactory _propertiesFactory;
		private readonly List<RegexPattern> _patterns;
		private readonly string _encryptionKey;

		public SegmentVisitor(List<RegexPattern> patterns,string encryptionKey)
		{
			_patterns = patterns;
			_encryptionKey = encryptionKey;
		}
		public void ReplaceText(ISegment segment, IDocumentItemFactory factory, IPropertiesFactory propertiesFactory)
		{
			_factory = factory;
			_propertiesFactory = propertiesFactory;
			VisitChildren(segment);
		}

		private string Anonymizer(string text,bool isTagContent)
		{
			foreach (var pattern in _patterns)
			{
				var regex = new Regex(pattern.Pattern, RegexOptions.IgnoreCase);

				var match = regex.Match(text);
				if (match.Success)
				{
					var result = regex.Replace(text, matchText => ProcessMatchData(matchText, pattern, isTagContent));
					return result;
				}
			}
			return text;
		}

		private string ProcessMatchData(Match match, RegexPattern pattern, bool isTagContent)
		{
			string encryptedText;
			//Check if the match should be encrypted
			encryptedText = pattern.ShouldEncrypt ? AnonymizeData.EncryptData(match.ToString(), _encryptionKey) : match.ToString();
			//For tag content we need to add {} for decrypting the data
			if (isTagContent)
			{
				return string.Concat("{", encryptedText, "}");
			}
			return encryptedText;
		}

		private bool ShouldAnonymize(string text)
		{
			foreach (var pattern in _patterns)
			{
				var regex = new Regex(pattern.Pattern, RegexOptions.IgnoreCase);
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
				var regex = new Regex(pattern.Pattern, RegexOptions.IgnoreCase);
				var matches = regex.Matches(segmentText);
				foreach (Match match in matches)
				{
					var data = new AnonymizedData
					{
						MatchText = match.Value,
						PositionInOriginalText = match.Index,
						EncryptedText = AnonymizeData.EncryptData(match.ToString(), _encryptionKey)
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
				tagPair.StartTagProperties.TagContent = Anonymizer(tagPair.StartTagProperties.TagContent,true);
				tagPair.TagProperties.SetMetaData("Anonymizer", "Anonymizer");
			}
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			
		}

		public void VisitText(IText text)
		{
			var markUpCollection = new List<IAbstractMarkupData>();
			var shouldAnonymize = ShouldAnonymize(text.Properties.Text);
			var originalSegmentClone = text.Clone();
			var count = 0;
			if (shouldAnonymize)
			{
				var anonymizedData = GetAnonymizedData(text.Properties.Text);

				GetSubsegmentPi(text, markUpCollection, anonymizedData);
				
				var abstractMarkupData = text.Parent.AllSubItems.FirstOrDefault(n => n.Equals(originalSegmentClone));
				if (abstractMarkupData == null)
				{
					abstractMarkupData = text.Parent.AllSubItems.FirstOrDefault(n => n.Equals(text));
				}
				if (abstractMarkupData != null)
				{
					var elementContainer = abstractMarkupData.Parent;
					
					foreach (var markupData in markUpCollection)
					{
						//that means is a text we don't need to add it
						if (elementContainer.Contains(markupData))
						{
							count++;
						}
						else
						{
							//in the case we have only aghisa@sdl.com in the segment
							//remove the text -> add the anonymized data in the same position
							if (elementContainer.AllSubItems.Count().Equals(1) &&
							    ShouldAnonymize(elementContainer.AllSubItems.ToList()[0].ToString()))
							{
								if (elementContainer.AllSubItems.Any())
								{
									elementContainer.AllSubItems.ToList()[0].RemoveFromParent();
									elementContainer.Insert(count, markupData);
								}
							}
							else
							{
								elementContainer.Insert(count, markupData);
							}
							count++;
						}
					}
				}
			}
		}

		private void GetSubsegmentPi(IText segmentText,List<IAbstractMarkupData> segmentContent, List<AnonymizedData> anonymizedDataList)
		{
				//this means we have PI data + text
			if (segmentText.Properties.Text.Length > anonymizedDataList[0].MatchText.Length)
			{
				//check if PI data is on first position split the segment after the PI
				if (anonymizedDataList[0].PositionInOriginalText.Equals(0))
				{
					var remainingSegmentText = segmentText.Split(anonymizedDataList[0].MatchText.Length);

					//check if we should encrypt or only tag the data
					var processedData = Anonymizer(segmentText.Properties.Text, false);
					var tag = _factory.CreatePlaceholderTag(
						_propertiesFactory.CreatePlaceholderTagProperties(processedData));
					tag.Properties.SetMetaData("Anonymizer", "Anonymizer");
					//Add encrypted tag to collection
					segmentContent.Add(tag);

					if (ShouldAnonymize(remainingSegmentText.Properties.Text))
					{
						var remainingData = GetAnonymizedData(remainingSegmentText.Properties.Text);
						GetSubsegmentPi(remainingSegmentText, segmentContent, remainingData);
					}
					else
					{
						segmentContent.Add(remainingSegmentText);
					}
				}
				else
				{
					var remainingSegmentText = segmentText.Split(anonymizedDataList[0].PositionInOriginalText);
					if (ShouldAnonymize(segmentText.Properties.Text))
					{
						var remainingData = GetAnonymizedData(segmentText.Properties.Text);
						GetSubsegmentPi(segmentText, segmentContent, remainingData);
					}
					else
					{
						segmentContent.Add(segmentText);
					}
					if (ShouldAnonymize(remainingSegmentText.Properties.Text))
					{
						var remainingData = GetAnonymizedData(remainingSegmentText.Properties.Text);
						GetSubsegmentPi(remainingSegmentText, segmentContent, remainingData);
					}
					else
					{
						var processedData = Anonymizer(remainingSegmentText.Properties.Text, false);
						var tag = _factory.CreatePlaceholderTag(
							_propertiesFactory.CreatePlaceholderTagProperties(processedData));
						tag.Properties.SetMetaData("Anonymizer", "Anonymizer");
						segmentContent.Add(tag);
					}
				}
			} //segment contains only PI data
			else
			{
				var processedData = Anonymizer(segmentText.Properties.Text, false);
				var tag = _factory.CreatePlaceholderTag(
					_propertiesFactory.CreatePlaceholderTagProperties(processedData));

				tag.Properties.SetMetaData("Anonymizer", "Anonymizer");
				segmentContent.Add(tag);
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
