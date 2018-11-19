using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff
{
	public class SegmentVisitor : IMarkupDataVisitor
	{
		private readonly List<RegexPattern> _patterns;
		private readonly string _encryptionKey;
		private readonly bool _arePatternsEcrypted;
		private IDocumentItemFactory _factory;
		private IPropertiesFactory _propertiesFactory;
		private string _id = null;

		public SegmentVisitor(List<RegexPattern> patterns, string encryptionKey, bool arePatternsEcnrypted)
		{
			_arePatternsEcrypted = arePatternsEcnrypted;
			_patterns = patterns;
			_encryptionKey = encryptionKey;
		}

		public void ReplaceText(ISegment segment, IDocumentItemFactory factory, IPropertiesFactory propertiesFactory)
		{
			_factory = factory;
			_propertiesFactory = propertiesFactory;
			VisitChildren(segment);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (tagPair.StartTagProperties != null)
			{
				tagPair.StartTagProperties.TagContent = Anonymizer(tagPair.StartTagProperties.TagContent, true);
				tagPair.TagProperties.SetMetaData("Anonymizer", "Anonymizer");
			}
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			//if (!string.IsNullOrEmpty(_id))
			//{
			//	if (_id == tag.TagProperties.ta)
			//}
		}

		public void VisitText(IText text)
		{
			var markUpCollection = new List<IAbstractMarkupData>();
			var shouldAnonymize = ShouldAnonymize(text.Properties.Text);
			var originalSegmentClone = text.Clone();
			var count = 0;
			if (shouldAnonymize)
			{
				try
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
								//in the case we have only PI in the segment
								//remove the text -> add the anonymized data in the same position
								
								if (elementContainer.AllSubItems.ToList().Count.Equals(1))
								{
									if (elementContainer.AllSubItems.ToList().ElementAtOrDefault(count) == null)
									{
										RandomizeTag(markupData);
										elementContainer.Insert(count, markupData);
									}
									else
									{
										elementContainer.AllSubItems.ToList()[0].RemoveFromParent();
										RandomizeTag(markupData);
										elementContainer.Insert(0, markupData);
									}
								}
								else
								{
									RandomizeTag(markupData);
									elementContainer.Insert(count, markupData);
								}
								count++;
							}
						}
					}
				}
				catch (Exception e) { }
			}
		}

		private static void RandomizeTag(IAbstractMarkupData markupData)
		{
			if (markupData is IPlaceholderTag tag)
			{
				tag.Properties.TagId = new TagId(Guid.NewGuid().ToString());
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

		private string Anonymizer(string text, bool isTagContent)
		{
			foreach (var pattern in _patterns)
			{
				var regex = DecryptIfEncrypted(pattern);

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
				var regex = DecryptIfEncrypted(pattern);
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
				var regex = DecryptIfEncrypted(pattern);
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

		private void GetSubsegmentPi(IText segmentText, List<IAbstractMarkupData> segmentContent, List<AnonymizedData> anonymizedDataList)
		{
			//this means we have PI data + text
			if (segmentText.Properties.Text.Length > anonymizedDataList[0].MatchText.Length)
			{
				//check if PI data is on first position split the segment after the PI
				if (anonymizedDataList[0].PositionInOriginalText.Equals(0))
				{
					var remainingSegmentText = segmentText.Split(anonymizedDataList[0].MatchText.Length);
								
					AddPlaceholderTag(segmentContent, segmentText);
					AddPlaceholderTag(segmentContent, remainingSegmentText);
					AnonymizeContent(remainingSegmentText, segmentContent);				
				}
				else
				{
					var remainingSegmentText = segmentText.Split(anonymizedDataList[0].PositionInOriginalText);
					AnonymizeContent(segmentText, segmentContent);

					if (ShouldAnonymize(remainingSegmentText.Properties.Text))
					{
						var remainingData = GetAnonymizedData(remainingSegmentText.Properties.Text);
						GetSubsegmentPi(remainingSegmentText, segmentContent, remainingData);
					}
					else
					{
						AddPlaceholderTag(segmentContent, remainingSegmentText);
					}
				}
			}
			else
			{				
				AddPlaceholderTag(segmentContent, segmentText);
			}
		}

		private void AddPlaceholderTag(ICollection<IAbstractMarkupData> segmentContent, IText text)
		{
			var processedData = Anonymizer(text.Properties.Text, false);
			var tag = _factory.CreatePlaceholderTag(_propertiesFactory.CreatePlaceholderTagProperties(processedData));
			tag.Properties.SetMetaData("Anonymizer", "Anonymizer");

			segmentContent.Add(tag);		
		}

		private void AnonymizeContent(IText text, List<IAbstractMarkupData> segmentContent)
		{
			if (ShouldAnonymize(text.Properties.Text))
			{
				var remainingData = GetAnonymizedData(text.Properties.Text);
				GetSubsegmentPi(text, segmentContent, remainingData);
			}
			else
			{
				segmentContent.Add(text);
			}
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

		private Regex DecryptIfEncrypted(RegexPattern pattern)
		{
			return new Regex(!_arePatternsEcrypted ? pattern.Pattern : AnonymizeData.DecryptData(pattern.Pattern, _encryptionKey), RegexOptions.IgnoreCase);
		}
	}
}