﻿using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Batch_Task;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Process_Xliff
{
	public class DecryptSegmentVisitor : IMarkupDataVisitor
	{
		private readonly AnonymizerSettings _decryptSettings;
		private readonly IDocumentItemFactory _documentItemFactory;
		private readonly IPropertiesFactory _propertiesFactory;

		public DecryptSegmentVisitor(IDocumentItemFactory documentItemFactory, IPropertiesFactory propertiesFactory, AnonymizerSettings decryptSettings)
		{
			_documentItemFactory = documentItemFactory;
			_propertiesFactory = propertiesFactory;
			_decryptSettings = decryptSettings;
		}

		public void DecryptText(IParagraph paragraph)
		{
			VisitChildren(paragraph);
		}

		public void DecryptText(ISegment segment)
		{			
			VisitChildren(segment);
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
						var decryptedText = _documentItemFactory.CreateText(
							_propertiesFactory.CreateTextProperties(
								DecryptText(tag.Properties.TagContent)));

						if (!_decryptSettings.IgnoreEncrypted)
						{
							var elementContainer = abstractMarkupData.Parent;
							elementContainer.Insert(tag.IndexInParent, decryptedText);
							elementContainer.RemoveAt(tag.IndexInParent);
						}
					}
					catch
					{
						// take the text from tag and insert it back as IText
						InsertTextBack(abstractMarkupData, tag);
					}
				}
			}
		}

		public void VisitText(IText text)
		{
			// ignored for this implementation
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{
			// ignored for this implementation
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			// ignored for this implementation
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			// ignored for this implementation
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			// ignored for this implementation
		}

		private string Decrypt(string text)
		{
			var regex = new Regex("{.*?}", RegexOptions.IgnoreCase);
			var result = regex.Replace(text, Process);

			return result;
		}

		private string Process(Match match)
		{
			if (match.Success && (match.ToString().Contains("{") && match.ToString().Contains("}")))
			{
				var encryptedText = match.ToString().Substring(1, match.ToString().Length - 2);
				try
				{
					var decryptedText = DecryptText(encryptedText);
					return decryptedText;
				}
				catch
				{
					return encryptedText;
				}
			}

			return match.ToString();
		}

		private void InsertTextBack(IAbstractMarkupData abstractMarkupData, IPlaceholderTag tag)
		{
			var elementContainer = abstractMarkupData.Parent;
			var untagedText = _documentItemFactory.CreateText(_propertiesFactory.CreateTextProperties(tag.Properties.TagContent));

			elementContainer.Insert(tag.IndexInParent, untagedText);
			elementContainer.RemoveAt(tag.IndexInParent);
		}

		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			if (container == null)
			{
				return;
			}

			foreach (var item in container.ToList())
			{
				item.AcceptVisitor(this);
			}
		}

		private string DecryptText(string encryptedText)
		{
			var isOldVersion = _decryptSettings.IsOldVersion ?? false;
			var encryptedKey = _decryptSettings.EncryptionKey;
			var decryptedKey = AnonymizeData.DecryptData(encryptedKey, Constants.Key);

			var key = isOldVersion ? encryptedKey : decryptedKey;

			_decryptSettings.IsOldVersion = false;

			return AnonymizeData.DecryptData(encryptedText, key);
		}
	}
}