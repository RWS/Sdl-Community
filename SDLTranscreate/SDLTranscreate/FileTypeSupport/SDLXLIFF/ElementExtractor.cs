using System;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;

namespace Trados.Transcreate.FileTypeSupport.SDLXLIFF
{
	public class ElementExtractor : IMarkupDataVisitor
	{
		private string _elementIdToSearch;

		private Element _element;

		private int _currentLockedContentId;

		public IAbstractMarkupData GetTag(string elementId, IAbstractMarkupDataContainer objectToSearch, Element element)
		{
			FoundElement = null;

			_elementIdToSearch = elementId;
			_element = element;
			_currentLockedContentId = 0;
			VisitChildren(objectToSearch);

			return FoundElement;
		}

		public IAbstractMarkupData FoundElement { get; private set; }

		/// <summary>
		/// Iterates all sub items of container (IAbstractMarkupDataContainer)
		/// </summary>
		/// <param name="container"></param>
		private void VisitChildren(IAbstractMarkupDataContainer container)
		{
			foreach (var item in container)
			{
				if (FoundElement != null)
				{
					return;
				}

				item.AcceptVisitor(this);
			}
		}

		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			if (FoundElement != null)
			{
				return;
			}

			VisitChildren(commentMarker);
		}

		public void VisitLocationMarker(ILocationMarker location) { }

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			if (FoundElement != null)
			{
				return;
			}
			
			if (_element is ElementLocked && _currentLockedContentId.ToString() == _elementIdToSearch)
			{
				FoundElement = lockedContent;
			}

			_currentLockedContentId++;
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			if (FoundElement != null)
			{
				return;
			}

			VisitChildren(marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			if (FoundElement != null)
			{
				return;
			}

			// Dev Notes: should handle the tagContentId differently; potentially have priority
			var tagContentId = GetTagContentId(tag.TagProperties.TagContent);

			if (_element is ElementPlaceholder && 
			    (tag.TagProperties.TagId.Id == _elementIdToSearch || tagContentId == _elementIdToSearch))
			{
				FoundElement = tag;
			}
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			if (FoundElement != null)
			{
				return;
			}

			VisitChildren(revisionMarker);
		}

		public void VisitSegment(ISegment segment)
		{
			if (FoundElement != null)
			{
				return;
			}

			VisitChildren(segment);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (FoundElement != null)
			{
				return;
			}

			if (_element is ElementTagPair elementTagPair)
			{
				// Dev Notes: should handle the tagContentId differently; potentially have priority
				var tagContentId = GetTagContentId(tagPair.TagProperties.TagContent);

				if ((elementTagPair.Type == Element.TagType.TagOpen || elementTagPair.Type == Element.TagType.TagClose) &&
				    (tagPair.TagProperties.TagId.Id == _elementIdToSearch || tagContentId == _elementIdToSearch))
				{
					FoundElement = tagPair;
				}

				VisitChildren(tagPair);
			}
		}

		public void VisitText(IText text) { }

		public string GetTagContentId(string text)
		{			
			var regexAttribute = new Regex(@"\s+(?<name>[^\s""]+)\=""(?<value>[^""]*)""", RegexOptions.Singleline | RegexOptions.IgnoreCase);		
			var mc = regexAttribute.Matches(text);
			if (mc.Count > 0)
			{
				foreach (Match match in mc)
				{
					var attValue = match.Groups["value"].Value;
					var attName = match.Groups["name"].Value;

					if (string.Compare(attName, "id", StringComparison.OrdinalIgnoreCase) == 0)
					{
						return attValue;
					}
				}
			}

			return null;
		}
	}
}
