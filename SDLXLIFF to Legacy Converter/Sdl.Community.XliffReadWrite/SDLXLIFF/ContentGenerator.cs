using System;
using System.Collections.Generic;
using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.XliffReadWrite.SDLXLIFF
{
    /// <summary>
    /// This class is used to traverse all elements that can occur inside a segment, e.g.
    /// text, tags, comments, placeholders, etc. In our implementation it is used to
    /// retrieve the plain text information (if required).
    /// </summary>
    internal class ContentGenerator : IMarkupDataVisitor
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

        internal List<ITagPair> TagPairs
        {
            get;
            set;
        }
        internal List<ILockedContent> LockedContentTags
        {
            get;
            set;
        }
        internal List<IPlaceholderTag> PlaceholderTags
        {
            get;
            set;
        }
        internal List<TagUnit> TagUnits
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
            TagPairs = new List<ITagPair>();
            LockedContentTags = new List<ILockedContent>();
            PlaceholderTags = new List<IPlaceholderTag>();
            TagUnits = new List<TagUnit>();

            IncludeTagText = includeTagText;
            VisitChildren(segment);
          
        }

        public void ProcessParagraphSegment(IParagraph segment, bool includeTagText)
        {


            PlainText = new StringBuilder("");
            SegmentSections = new List<SegmentSection>();
            Comments = new List<Comment>();
            TagPairs = new List<ITagPair>();
            LockedContentTags = new List<ILockedContent>();
            PlaceholderTags = new List<IPlaceholderTag>();
            TagUnits = new List<TagUnit>();

            IncludeTagText = includeTagText;
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
                var xComment = new Comment
                {
                    Author = commentMarker.Comments.GetItem(i).Author,
                    Date = commentMarker.Comments.GetItem(i).Date,
                    DateSpecified = commentMarker.Comments.GetItem(i).DateSpecified,
                    Severity = commentMarker.Comments.GetItem(i).Severity.ToString(),
                    Text = commentMarker.Comments.GetItem(i).Text,
                    Version = commentMarker.Comments.GetItem(i).Version
                };

                Comments.Add(xComment);               
            }

            VisitChildren(commentMarker);
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            // Not required for this implementation.
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            LockedContentTags.Add((ILockedContent)lockedContent);

            SegmentSections.Add(new SegmentSection(SegmentSection.ContentType.LockedContent, "", lockedContent.Content.ToString()));
 
            PlainText.Append(lockedContent.Content);
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            VisitChildren(marker);
        }
        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            PlaceholderTags.Add((IPlaceholderTag)tag.Clone());
        

            if (tag.Properties.HasTextEquivalent && !IncludeTagText)
            {
                TagUnits.Add(new TagUnit(tag.TagProperties.TagId.Id, tag.Properties.DisplayText, tag.Properties.TextEquivalent, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsPlaceholder));

                SegmentSections.Add(new SegmentSection(SegmentSection.ContentType.Placeholder, tag.TagProperties.TagId.Id, tag.Properties.TextEquivalent));

                PlainText.Append(tag.Properties.TextEquivalent);
            }

            if (!IncludeTagText) return;
            TagUnits.Add(new TagUnit(tag.TagProperties.TagId.Id, tag.Properties.DisplayText, tag.Properties.TagContent, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsPlaceholder));

            SegmentSections.Add(new SegmentSection(SegmentSection.ContentType.Placeholder, tag.TagProperties.TagId.Id,  tag.Properties.TagContent));

            PlainText.Append(tag.TagProperties.TagContent);
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
                case RevisionType.FeedbackComment:
                    break;
                case RevisionType.FeedbackAdded:
                    break;
                case RevisionType.FeedbackDeleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void VisitSegment(ISegment segment)
        {
            VisitChildren(segment);
        }

        public void VisitTagPair(ITagPair tagPair)
        {

            TagPairs.Add((ITagPair)tagPair.Clone());
    
            
            if (IncludeTagText)
            {
                TagUnits.Add(new TagUnit(tagPair.TagProperties.TagId.Id, tagPair.TagProperties.DisplayText, tagPair.StartTagProperties.TagContent, TagUnit.TagUnitState.IsOpening, TagUnit.TagUnitType.IsTag));

                SegmentSections.Add(new SegmentSection(SegmentSection.ContentType.Tag,  tagPair.TagProperties.TagId.Id, tagPair.StartTagProperties.TagContent));
           
                PlainText.Append(tagPair.StartTagProperties.TagContent);
            }

            VisitChildren(tagPair);

            if (!IncludeTagText) return;
            TagUnits.Add(new TagUnit(tagPair.TagProperties.TagId.Id, string.Empty, tagPair.EndTagProperties.TagContent, TagUnit.TagUnitState.IsClosing, TagUnit.TagUnitType.IsTag));

            SegmentSections.Add(new SegmentSection(SegmentSection.ContentType.TagClosing, tagPair.TagProperties.TagId.Id,  tagPair.EndTagProperties.TagContent));

            PlainText.Append(tagPair.EndTagProperties.TagContent);
        }

        public void VisitText(IText text)
        {                        
            if (SegmentSections.Count > 0 && SegmentSections[(SegmentSections.Count - 1)].Type == SegmentSection.ContentType.Text)
                SegmentSections[SegmentSections.Count - 1].Content += text.Properties.Text;
            else
                SegmentSections.Add(new SegmentSection(SegmentSection.ContentType.Text, "", text.Properties.Text));

            PlainText.Append(text.Properties.Text);
        }

        #endregion
    }
}
