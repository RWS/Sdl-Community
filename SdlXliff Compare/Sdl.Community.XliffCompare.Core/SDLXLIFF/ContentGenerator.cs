using System.Collections.Generic;
using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.XliffCompare.Core.SDLXLIFF
{
    /// <summary>
    /// This class is used to traverse all elements that can occur inside a segment, e.g.
    /// text, tags, comments, placeholders, etc. In our implementation it is used to
    /// retrieve the plain text information (if required).
    /// </summary>
    public class ContentGenerator : IMarkupDataVisitor
    {

       
        #region settings

        internal List<Comment> Comments
        {
            get;
            set;
        }

        internal List<SegmentSection> SegmentSections
        {
            get;
            set;
        }
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

        #region "process segment"
        // process the segment; individuating the content text, tags & comments
        public void ProcessSegment(ISegment segment, bool includeTagText)
        {
            
            PlainText = new StringBuilder("");
            SegmentSections = new List<SegmentSection>();
            Comments = new List<Comment>();

            IncludeTagText = includeTagText;
            VisitChildren(segment);
          
        }

        public void ProcessParagraphSegment(IParagraph segment)
        {


            PlainText = new StringBuilder("");
            SegmentSections = new List<SegmentSection>();
            Comments = new List<Comment>();


            IncludeTagText = true;
            VisitChildren(segment);

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
            for (var i = 0; i < commentMarker.Comments.Count; i++)
            {
                var comment = new Comment
                {
                    Author = commentMarker.Comments.GetItem(i).Author,
                    Date = commentMarker.Comments.GetItem(i).Date,
                    DateSpecified = commentMarker.Comments.GetItem(i).DateSpecified,
                    Severity = commentMarker.Comments.GetItem(i).Severity.ToString(),
                    Text = commentMarker.Comments.GetItem(i).Text,
                    Version = commentMarker.Comments.GetItem(i).Version
                };

                Comments.Add(comment);
            }

            VisitChildren(commentMarker);
        }

        public void VisitLocationMarker(ILocationMarker location)
        {

            // Not required for this implementation.
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            SegmentSections.Add(new SegmentSection(false, lockedContent.Content.ToString()));
 
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
                SegmentSections.Add(new SegmentSection(false, tag.Properties.TextEquivalent));

                PlainText.Append(tag.Properties.TextEquivalent);
            }

            if (IncludeTagText)
            {
                SegmentSections.Add(new SegmentSection(false, tag.Properties.TagContent));

                PlainText.Append(tag.TagProperties.TagContent);
            }
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            switch (revisionMarker.Properties.RevisionType)
            {
                case RevisionType.Delete:
                    {
                        //ignore
                    } break;
                case RevisionType.Insert:
                    {
                        VisitChildren(revisionMarker);
                    } break;
                case RevisionType.Unchanged:
                    {
                        VisitChildren(revisionMarker);
                    } break;
            }
        }

        public void VisitSegment(ISegment segment)
        {
            VisitChildren(segment);
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            if (IncludeTagText)
            {
                SegmentSections.Add(new SegmentSection(false, tagPair.StartTagProperties.TagContent));

                PlainText.Append(tagPair.StartTagProperties.TagContent);
            }

            VisitChildren(tagPair);

            if (!IncludeTagText) 
                return;

            SegmentSections.Add(new SegmentSection(false, tagPair.EndTagProperties.TagContent));

            PlainText.Append(tagPair.EndTagProperties.TagContent);
        }

        public void VisitText(IText text)
        {                
            if (SegmentSections.Count > 0 && SegmentSections[SegmentSections.Count - 1].IsText)
                SegmentSections[SegmentSections.Count - 1].Content += text.Properties.Text;
            else
                SegmentSections.Add(new SegmentSection(true, text.Properties.Text));

            PlainText.Append(text.Properties.Text);
        }
        #endregion
    }
}
