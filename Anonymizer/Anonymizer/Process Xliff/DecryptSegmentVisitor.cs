using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sdl.Community.Anonymizer.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class DecryptSegmentVisitor : IMarkupDataVisitor
	{
		private IDocumentItemFactory _factory;
		private IPropertiesFactory _propertiesFactory;
		public void DecryptText(ISegment segment, IDocumentItemFactory factory, IPropertiesFactory propertiesFactory)
		{
			_factory = factory;
			_propertiesFactory = propertiesFactory;
			VisitChildren(segment);
		}

		//private string Decrypt(string text)
		//{
		//	var regex = new Regex("{.*?}", RegexOptions.IgnoreCase);
		//	var result = regex.Replace(text, new MatchEvaluator(Process));

		//	return result;
		//}

		//private string Process(Match match)
		//{
		//	if (match.Success)
		//	{
		//		if (match.ToString().Contains("{") && match.ToString().Contains("}"))
		//		{
		//			var encryptedText = match.ToString().Substring(1, match.ToString().Length - 2);
		//			var decryptedText = AnonymizeData.DecryptData(encryptedText, "Andrea");
		//			return decryptedText;
		//		}
		//	}
		//	return match.ToString();
		//}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (tagPair.StartTagProperties != null)
			{
				if (tagPair.StartTagProperties.MetaDataContainsKey("Anonymizer"))
				{
					var decryptedText = AnonymizeData.DecryptData(tagPair.StartTagProperties.TagContent, "Andrea");
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
					var text = _factory.CreateText(
						_propertiesFactory.CreateTextProperties(AnonymizeData.DecryptData(tag.Properties.TagContent, "Andrea")));
					var elementContainer = abstractMarkupData.Parent;
					elementContainer.Add(text);
					elementContainer.RemoveAt(tag.IndexInParent);
				}
				//decryptedData.IndexInParent = tag.IndexInParent;

				//tag.Properties.TagContent = AnonymizeData.DecryptData(tag.Properties.TagContent,"Andrea");

			}
		}

		public void VisitText(IText text)
		{
			//var decryptedText = Decrypt(text.Properties.Text);
			//text.Properties.Text =  decryptedText;
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
