using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Anonymizer.Process_Xliff
{
	public class DecryptSegmentVisitor : IMarkupDataVisitor
	{
		public void DecryptText(ISegment segment)
		{
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
					var decryptedText = AnonymizeData.DecryptData(encryptedText, "Andrea");
					return decryptedText;
				}
			}
			return match.ToString();
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (tagPair.StartTagProperties != null)
			{
				var decryptedText = Decrypt(tagPair.StartTagProperties.TagContent);
				tagPair.StartTagProperties.TagContent = decryptedText;
			}
			VisitChildren(tagPair);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
		
		}

		public void VisitText(IText text)
		{
			var decryptedText = Decrypt(text.Properties.Text);
			text.Properties.Text =  decryptedText;
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
