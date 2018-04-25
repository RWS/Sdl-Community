using System;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.projectAnonymizer.Process_Xliff
{
	public class DecryptSegmentVisitor : IMarkupDataVisitor
	{
		private IDocumentItemFactory _factory;
		private IPropertiesFactory _propertiesFactory;
		private readonly string _encryptionKey;

		public DecryptSegmentVisitor(string encryptionKey)
		{
			_encryptionKey = encryptionKey;
		}
		public void DecryptText(ISegment segment, IDocumentItemFactory factory, IPropertiesFactory propertiesFactory)
		{
			_factory = factory;
			_propertiesFactory = propertiesFactory;
			VisitChildren(segment);
		}

		private string Decrypt(string text)
		{
			var regex = new Regex("{.*?}", RegexOptions.IgnoreCase);
			var result = regex.Replace(text, new MatchEvaluator(Process));

			return result;
		}

		private string Process(Match match)
		{
			if (match.Success)
			{
				if (match.ToString().Contains("{") && match.ToString().Contains("}"))
				{
					var encryptedText = match.ToString().Substring(1, match.ToString().Length - 2);
					try
					{
						var decryptedText = AnonymizeData.DecryptData(encryptedText, _encryptionKey);
						return decryptedText;
					}
					catch (Exception e)
					{
						return encryptedText;
					}
					
				}
			}
			return match.ToString();
		}

		//tag pair comes as text with '{ }' symbol we need to match the text and remove the symbols
		public void VisitTagPair(ITagPair tagPair)
		{
			if (tagPair.StartTagProperties != null)
			{
				if (tagPair.StartTagProperties.MetaDataContainsKey("Anonymizer"))
				{
					var decryptedText = Decrypt(tagPair.StartTagProperties.TagContent);
					tagPair.StartTagProperties.TagContent = decryptedText;
				}
				
			}
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			if (tag.Properties.MetaDataContainsKey("Anonymizer"))
			{
				var abstractMarkupData = tag.Parent.AllSubItems.FirstOrDefault(i => i.IndexInParent.Equals(tag.IndexInParent));
				if (abstractMarkupData != null)
				{
					//if we catch an exception that means is only a taged text is not encrypted
					try
					{
						var decryptedText = _factory.CreateText(
							_propertiesFactory.CreateTextProperties(AnonymizeData.DecryptData(tag.Properties.TagContent, _encryptionKey)));

						var elementContainer = abstractMarkupData.Parent;

						elementContainer.Insert(tag.IndexInParent, decryptedText);

						elementContainer.RemoveAt(tag.IndexInParent);
					}
					catch (Exception e)
					{
						// take the text from tag and insert it back as IText
						var elementContainer = abstractMarkupData.Parent;
						var untagedText = _factory.CreateText(
							_propertiesFactory.CreateTextProperties(tag.Properties.TagContent));
						elementContainer.Insert(tag.IndexInParent, untagedText);

						elementContainer.RemoveAt(tag.IndexInParent);
					}
				}
			}
		}

		public void VisitText(IText text)
		{

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
