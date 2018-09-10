using System;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Wordprocessing;
using Sdl.Community.projectAnonymizer.Batch_Task;
using Sdl.Community.projectAnonymizer.Helpers;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.projectAnonymizer.Process_Xliff
{
	public class DecryptSegmentVisitor : IMarkupDataVisitor
	{
		private IDocumentItemFactory _factory;
		private IPropertiesFactory _propertiesFactory;
		private readonly DecryptSettings _decryptSettings;

		public DecryptSegmentVisitor(DecryptSettings decryptSettings)
		{
			_decryptSettings = decryptSettings;
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
			var result = regex.Replace(text, Process);
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
						//var key = AnonymizeData.EncryptData(_decryptSettings.EncryptionKey, Constants.Key);
						var decryptedText = DecryptText(encryptedText);
						if (!_decryptSettings.IgnoreEncrypted)
						{
							return decryptedText;
						}
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
						//var key = AnonymizeData.EncryptData(_decryptSettings.EncryptionKey, Constants.Key);
						var decryptedText = _factory.CreateText(
							_propertiesFactory.CreateTextProperties(
								DecryptText(tag.Properties.TagContent)));

						if (!_decryptSettings.IgnoreEncrypted)
						{
							var elementContainer = abstractMarkupData.Parent;
							elementContainer.Insert(tag.IndexInParent, decryptedText);
							elementContainer.RemoveAt(tag.IndexInParent);
						}
					}
					catch (Exception e)
					{
						// take the text from tag and insert it back as IText
						InsertTextBack(abstractMarkupData, tag);
					}
				}
			}
		}

		private void InsertTextBack(IAbstractMarkupData abstractMarkupData, IPlaceholderTag tag)
		{
			var elementContainer = abstractMarkupData.Parent;
			var untagedText = _factory.CreateText(
				_propertiesFactory.CreateTextProperties(tag.Properties.TagContent));
			elementContainer.Insert(tag.IndexInParent, untagedText);

			elementContainer.RemoveAt(tag.IndexInParent);
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

		private string DecryptText(string encryptedText)
		{
			var isOldVersion = !_decryptSettings.SettingsBundle.GetSettingsGroup<AnonymizerSettings>("AnonymizerSettings").IsEncrypted;
			var encryptedKey = _decryptSettings.EncryptionKey;
			var decryptedKey = AnonymizeData.DecryptData(encryptedKey, Constants.Key);

			var key = _isOldVersion ? encryptedKey : decryptedKey;

			return AnonymizeData.DecryptData(encryptedText, key);
		}
	}
}
