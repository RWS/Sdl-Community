using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Transcreate.FileTypeSupport.SDLXLIFF
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
				item.AcceptVisitor(this);
			}
		}


		
		public void VisitCommentMarker(ICommentMarker commentMarker)
		{
			VisitChildren(commentMarker);
		}

		public void VisitLocationMarker(ILocationMarker location)
		{

		}

		public void VisitLockedContent(ILockedContent lockedContent)
		{
			if (_element is ElementLocked && _currentLockedContentId.ToString() == _elementIdToSearch)
			{
				FoundElement = lockedContent;
			}

			_currentLockedContentId++;
		}

		public void VisitOtherMarker(IOtherMarker marker)
		{
			VisitChildren(marker);
		}

		public void VisitPlaceholderTag(IPlaceholderTag tag)
		{
			if (_element is ElementPlaceholder && tag.TagProperties.TagId.Id == _elementIdToSearch)
			{
				FoundElement = tag;
			}
		}

		public void VisitRevisionMarker(IRevisionMarker revisionMarker)
		{
			VisitChildren(revisionMarker);
		}

		public void VisitSegment(ISegment segment)
		{
			VisitChildren(segment);
		}

		public void VisitTagPair(ITagPair tagPair)
		{
			if (_element is ElementTagPair elementTagPair)
			{
				if (elementTagPair.Type == Element.TagType.OpeningTag || elementTagPair.Type== Element.TagType.ClosingTag
				    && tagPair.TagProperties.TagId.Id == _elementIdToSearch)
				{
					FoundElement = tagPair;
				}

				VisitChildren(tagPair);
			}
		}

		public void VisitText(IText text)
		{

		}		
	}
}
