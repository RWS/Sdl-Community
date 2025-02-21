using Sdl.FileTypeSupport.Framework.BilingualApi;
using Trados.Transcreate.FileTypeSupport.MSOffice.Model;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Visitors
{
	internal class ElementExtractor : IMarkupDataVisitor
	{
		private string ElementIdToSearch
		{
			get;
			set;
		}

		private Token.TokenType ElementTypeToSearch
		{
			get;
			set;
		}

		public IAbstractMarkupData FoundElement
		{
			get;
			set;
		}

		private int _currentLockedContentId;

		public IAbstractMarkupData GetTag(string elementID, IAbstractMarkupDataContainer objectToSearch, Token.TokenType tokenType)
		{
			ElementIdToSearch = elementID;
			ElementTypeToSearch = tokenType;
			FoundElement = null;
			_currentLockedContentId = 0;
			VisitChildren(objectToSearch);
			return FoundElement;
		}

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
			if (ElementTypeToSearch == Token.TokenType.LockedContent && _currentLockedContentId.ToString() == ElementIdToSearch)
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
			if (ElementTypeToSearch == Token.TokenType.TagPlaceholder && tag.TagProperties.TagId.Id == ElementIdToSearch)
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
			if ((ElementTypeToSearch == Token.TokenType.TagOpen || ElementTypeToSearch == Token.TokenType.TagClose) && tagPair.TagProperties.TagId.Id == ElementIdToSearch)
			{
				FoundElement = tagPair;
			}

			VisitChildren(tagPair);
		}

		public void VisitText(IText text)
		{

		}
	}
}
