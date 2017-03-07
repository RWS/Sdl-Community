using System;
using System.Collections.Generic;
using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.PostEdit.Compare.Core.SDLXLIFF
{
    /// <summary>
    /// This class is used to traverse all elements that can occur inside a segment, e.g.
    /// text, tags, comments, placeholders, etc. In our implementation it is used to
    /// retrieve the plain text information (if required).
    /// </summary>
    public class ContentGenerator : IMarkupDataVisitor
    {

       
        #region settings

        public Sdl.LanguagePlatform.Core.Segment Segment { get; set; }

        private int TagCounter { get; set; }

        public List<Comment> Comments
        {
            get;
            set;
        }

        public List<SegmentSection> SegmentSections
        {
            get;
            set;
        }
        public StringBuilder PlainText
        {
            get;
            set;
        }

        public bool IncludeTagText
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




        private bool IsRevisionMarker { get; set; }
        public List< RevisionMarker> RevisionMarkers { get; set; }
        public RevisionMarker RevisionMarker { get; set; }
        
        
        
        
        
        
        
        
        #endregion


        #region "process segment"
        // process the segment; individuating the content text, tags & comments
        public void ProcessSegment(ISegment segment, bool includeTagText)
        {
            TagCounter = 0;
            Segment = new Sdl.LanguagePlatform.Core.Segment();
            PlainText = new StringBuilder(string.Empty);
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
            TagCounter = 0;
            Segment = new Sdl.LanguagePlatform.Core.Segment();

            PlainText = new StringBuilder(string.Empty);
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
            var lockedTag = new Sdl.LanguagePlatform.Core.Tag
            {
                Type = Sdl.LanguagePlatform.Core.TagType.LockedContent,
                TextEquivalent = lockedContent.Content.ToString(),
                Anchor = TagCounter
            };
            Segment.Add(lockedTag);

            TagCounter++;

            SegmentSections.Add(IsRevisionMarker
                ? new SegmentSection(SegmentSection.ContentType.LockedContent, string.Empty,
                    lockedContent.Content.ToString(), RevisionMarker)
                : new SegmentSection(SegmentSection.ContentType.LockedContent, string.Empty,
                    lockedContent.Content.ToString()));

            PlainText.Append(lockedContent.Content);
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            VisitChildren(marker);
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {            
            var placeHolderTag = new Sdl.LanguagePlatform.Core.Tag
            {
                Type = Sdl.LanguagePlatform.Core.TagType.TextPlaceholder,
                TagID = tag.TagProperties.TagId.Id,
                TextEquivalent = tag.Properties.TextEquivalent,
                Anchor = TagCounter
            };
            Segment.Add(placeHolderTag);

            TagCounter++;

            if (tag.Properties.HasTextEquivalent && !IncludeTagText)
            {
                SegmentSections.Add(IsRevisionMarker
                    ? new SegmentSection(SegmentSection.ContentType.Placeholder, tag.TagProperties.TagId.Id,
                        tag.Properties.TextEquivalent, RevisionMarker)
                    : new SegmentSection(SegmentSection.ContentType.Placeholder, tag.TagProperties.TagId.Id,
                        tag.Properties.TextEquivalent));


                PlainText.Append(tag.Properties.TextEquivalent);
            }
            else if (IncludeTagText)
            {
                SegmentSections.Add(IsRevisionMarker
                    ? new SegmentSection(SegmentSection.ContentType.Placeholder, tag.TagProperties.TagId.Id,
                        tag.Properties.TagContent, RevisionMarker)
                    : new SegmentSection(SegmentSection.ContentType.Placeholder, tag.TagProperties.TagId.Id,
                        tag.Properties.TagContent));

                PlainText.Append(tag.TagProperties.TagContent);
            }
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            IsRevisionMarker = true;

           
            switch (revisionMarker.Properties.RevisionType)
            {
                case RevisionType.Delete:
                {
                    if (revisionMarker.Properties.Date != null)
                        RevisionMarker = new RevisionMarker(revisionMarker.Properties.Author, (DateTime)revisionMarker.Properties.Date, SDLXLIFF.RevisionMarker.RevisionType.Delete);
                    VisitChildren(revisionMarker);
                } break;
                case RevisionType.Insert:
                {
                    if (revisionMarker.Properties.Date != null)
                        RevisionMarker = new RevisionMarker(revisionMarker.Properties.Author, (DateTime)revisionMarker.Properties.Date, RevisionMarker.RevisionType.Insert);
                    VisitChildren(revisionMarker);
                } break;
                case RevisionType.Unchanged:
                {
                    if (revisionMarker.Properties.Date != null)
                        RevisionMarker = new RevisionMarker(revisionMarker.Properties.Author, (DateTime)revisionMarker.Properties.Date, SDLXLIFF.RevisionMarker.RevisionType.Unchanged);
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
            IsRevisionMarker = false;
        }

        public void VisitSegment(ISegment segment)
        {
            VisitChildren(segment);
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            if (IncludeTagText)
            {
                TagUnits.Add(new TagUnit(tagPair.TagProperties.TagId.Id, tagPair.TagProperties.DisplayText, tagPair.StartTagProperties.TagContent, TagUnit.TagUnitState.IsOpening, TagUnit.TagUnitType.IsTag));

                SegmentSections.Add(IsRevisionMarker
                    ? new SegmentSection(SegmentSection.ContentType.Tag, tagPair.TagProperties.TagId.Id,
                        tagPair.StartTagProperties.TagContent, RevisionMarker)
                    : new SegmentSection(SegmentSection.ContentType.Tag, tagPair.TagProperties.TagId.Id,
                        tagPair.StartTagProperties.TagContent));


                PlainText.Append(tagPair.StartTagProperties.TagContent);
            }

            var tgStart = new Sdl.LanguagePlatform.Core.Tag
            {
                Type = Sdl.LanguagePlatform.Core.TagType.Start,
                Anchor = TagCounter,
                TagID = tagPair.StartTagProperties.TagId.Id,
                TextEquivalent = tagPair.StartTagProperties.DisplayText
            };


            Segment.Add(tgStart);
            



            VisitChildren(tagPair);




            var tgEnd = new Sdl.LanguagePlatform.Core.Tag
            {
                Type = Sdl.LanguagePlatform.Core.TagType.End,
                TagID = tagPair.StartTagProperties.TagId.Id,
                Anchor = TagCounter,
                TextEquivalent = tagPair.EndTagProperties.DisplayText
            };
            Segment.Add(tgEnd);

            if (IncludeTagText)
            {
                TagUnits.Add(new TagUnit(tagPair.TagProperties.TagId.Id, string.Empty, tagPair.EndTagProperties.TagContent, TagUnit.TagUnitState.IsClosing, TagUnit.TagUnitType.IsTag));

                SegmentSections.Add(IsRevisionMarker
                    ? new SegmentSection(SegmentSection.ContentType.TagClosing,
                        tagPair.TagProperties.TagId.Id, tagPair.EndTagProperties.TagContent, RevisionMarker)
                    : new SegmentSection(SegmentSection.ContentType.TagClosing,
                        tagPair.TagProperties.TagId.Id, tagPair.EndTagProperties.TagContent));

                PlainText.Append(tagPair.EndTagProperties.TagContent);
            }
            TagCounter++;
        }

        public void VisitText(IText text)
        {
            if (SegmentSections.Count > 0 && SegmentSections[(SegmentSections.Count - 1)].Type == SegmentSection.ContentType.Text)
            {
                if (IsRevisionMarker)
                {
                    if (SegmentSections[(SegmentSections.Count - 1)].RevisionMarker != null && SegmentSections[(SegmentSections.Count - 1)].RevisionMarker.Type == RevisionMarker.Type)
                        SegmentSections[(SegmentSections.Count - 1)].Content += text.Properties.Text;
                    else
                        SegmentSections.Add(new SegmentSection(SegmentSection.ContentType.Text, string.Empty, text.Properties.Text, RevisionMarker));
                }
                else
                {
                    if (SegmentSections[(SegmentSections.Count - 1)].RevisionMarker == null)
                        SegmentSections[(SegmentSections.Count - 1)].Content += text.Properties.Text;
                    else
                        SegmentSections.Add(new SegmentSection(SegmentSection.ContentType.Text, string.Empty, text.Properties.Text));
                }
            }
            else
            {
                SegmentSections.Add(IsRevisionMarker
                    ? new SegmentSection(SegmentSection.ContentType.Text, string.Empty, text.Properties.Text,
                        RevisionMarker)
                    : new SegmentSection(SegmentSection.ContentType.Text, string.Empty, text.Properties.Text));
            }

            if ((!IsRevisionMarker || RevisionMarker.Type == RevisionMarker.RevisionType.Delete) &&
                IsRevisionMarker) 
                return;

            Segment.Add(text.Properties.Text);
            PlainText.Append(text.Properties.Text);
        }

        #endregion
    }
}
