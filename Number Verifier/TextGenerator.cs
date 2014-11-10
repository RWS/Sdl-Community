using System.Text;

using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.NumberVerifier
{
    /// <summary>
    /// This class is used to traverse all elements that can occur inside a segment, e.g.
    /// text, tags, comments, placeholders, etc. In our implementation it is used to
    /// retrieve the plain text information (if required).
    /// </summary>
    public class TextGenerator : IMarkupDataVisitor
    {
        #region settings
        internal StringBuilder PlainText
        {
            get;
            set;
        }

        internal bool IncludeTagText
        {
            get;
            set;
        }
        #endregion

        #region "get plain text"
        // Returns the plain text representation of a segment. 
        // If the includeTagText parameter is true, the returned string will 
        // also contain the tag content for each tag.
        public string GetPlainText(ISegment segment, bool includeTagText)
        {
            PlainText = new StringBuilder("");
            IncludeTagText = includeTagText;
            VisitChildren(segment);
            return PlainText.ToString();
        }
        #endregion

        #region "iterate sub-items"
        // Iterates all sub items of segment container (IMarkupDataContainer)
        private void VisitChildren(IAbstractMarkupDataContainer container)
        {
            foreach (var item in container)
            {
                item.AcceptVisitor(this);
            }
        }
        #endregion



        #region "IMarkupDataVisitor Members"
        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            VisitChildren(commentMarker);
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            // Not required for this implementation.
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            PlainText.Append(lockedContent.Content);
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            VisitChildren(marker);
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            if (tag.Properties.HasTextEquivalent && !IncludeTagText)
            {
                PlainText.Append(tag.Properties.TextEquivalent);
            }

            if (IncludeTagText)
            {
                PlainText.Append(tag.TagProperties.TagContent);
            }
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            // Not required for this implementation.
        }

        public void VisitSegment(ISegment segment)
        {
            VisitChildren(segment);
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            if (IncludeTagText)
            {
                PlainText.Append(tagPair.StartTagProperties.TagContent);
            }
            
            VisitChildren(tagPair);

            if (IncludeTagText)
            {
                PlainText.Append(tagPair.EndTagProperties.TagContent);
            }
        }

        public void VisitText(IText text)
        {
            PlainText.Append(text.Properties.Text);
        }
        #endregion
    }
 }

