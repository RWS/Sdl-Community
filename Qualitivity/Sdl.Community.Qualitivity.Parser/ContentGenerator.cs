using System;
using System.Collections.Generic;
using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.Parser
{
    public class ContentGenerator : IMarkupDataVisitor
    {
        #region settings


        public Segment Segment { get; set; }

        internal StringBuilder PlainText { get; set; }       
        internal List<ITagPair> TagPairs { get; set; }
        internal List<Tag> TagUnits { get; set; }
        internal List<ILockedContent> LockedContentTags { get; set; }
        internal List<IPlaceholderTag> PlaceholderTags { get; set; }
        

        public List<Comment> Comments { get; set; }
        public List<SegmentSection> SegmentSections { get; set; }     
        
        public List<string> RevisionMarkersUniqueIds { get; set; }        
       

        private bool IncludeTagText { get; set; }
        private bool IsRevisionMarker { get; set; }
        private RevisionMarker RevisionMarker { get; set; }
        private int TagCounter { get; set; }


       
        #endregion
   
        #region "process segment"
        
        public void ProcessSegment(ISegment segment, bool includeTagText, List<string> revisionMarkersUniqueIds)
        {
            IsRevisionMarker = false;
            RevisionMarkersUniqueIds = revisionMarkersUniqueIds;
            Segment = new Segment();
           
            PlainText = new StringBuilder("");
            SegmentSections = new List<SegmentSection>();
            Comments = new List<Comment>();
            TagPairs = new List<ITagPair>();
            LockedContentTags = new List<ILockedContent>();
            PlaceholderTags = new List<IPlaceholderTag>();
            TagUnits = new List<Tag>();

            IncludeTagText = includeTagText;

            
            VisitChildren(segment);
        }

   
        #endregion

        
        #region "iterate sub-items"
       
        private void VisitChildren(IAbstractMarkupDataContainer container)
        {
            if (container == null)
                return;

            foreach (var item in container)                           
                item.AcceptVisitor(this);            
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
            LockedContentTags.Add((ILockedContent)lockedContent.Clone());

            var objTag = new Tag
            {
                AlignmentAnchor = "",
                Anchor = "",
                TagId = lockedContent.UniqueId.ToString(),
                TextEquivalent = lockedContent.Content.ToString(),
                SectionType = Tag.Type.LockedContent
            };
            SegmentSections.Add(objTag);

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
                var objTag = new Tag
                {
                    AlignmentAnchor = "",
                    Anchor = TagCounter.ToString(),
                    TagId = tag.TagProperties.TagId.Id,
                    TextEquivalent = tag.Properties.TextEquivalent,
                    SectionType = Tag.Type.Standalone
                };


                TagUnits.Add(objTag);

                objTag.Revision = IsRevisionMarker ? RevisionMarker : null;
                SegmentSections.Add(objTag);

                PlainText.Append(tag.Properties.TextEquivalent);
            }
            else if (IncludeTagText)
            {
                var objTag = new Tag
                {
                    AlignmentAnchor = "",
                    Anchor = TagCounter.ToString(),
                    TagId = tag.TagProperties.TagId.Id,
                    TextEquivalent = tag.Properties.TagContent,
                    SectionType = Tag.Type.Standalone
                };

                TagUnits.Add(objTag);

                objTag.Revision = IsRevisionMarker ? RevisionMarker : null;
                SegmentSections.Add(objTag);

                PlainText.Append(tag.TagProperties.TagContent);


                var placeHolderTag = new Sdl.LanguagePlatform.Core.Tag
                {
                    Type = TagType.TextPlaceholder,
                    TagID = tag.TagProperties.TagId.Id,
                    TextEquivalent = tag.Properties.TextEquivalent,
                    Anchor = TagCounter
                };
                Segment.Add(placeHolderTag);
            }
            TagCounter++;
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            if (revisionMarker.Properties.Date != null)
            {
                var uniqueId = revisionMarker.Properties.Date.Value.Ticks + "." + revisionMarker.Properties.RevisionType;

                if (!RevisionMarkersUniqueIds.Contains(uniqueId))
                {
                    RevisionMarkersUniqueIds.Add(uniqueId);
                    IsRevisionMarker = true;
                }
                else
                    IsRevisionMarker = false;


                switch (revisionMarker.Properties.RevisionType)
                {
                    case RevisionType.Delete:
                    {
                        if (IsRevisionMarker)
                        {
                            RevisionMarker = new RevisionMarker(uniqueId, revisionMarker.Properties.Author, (DateTime)revisionMarker.Properties.Date, RevisionMarker.RevisionType.Delete);
                            VisitChildren(revisionMarker);
                        }
                        else
                            RevisionMarker = null;
                    } break;
                    case RevisionType.Insert:
                    {
                        RevisionMarker = IsRevisionMarker ? new RevisionMarker(uniqueId, revisionMarker.Properties.Author, (DateTime)revisionMarker.Properties.Date, RevisionMarker.RevisionType.Insert) : null;

                        VisitChildren(revisionMarker);
                    } break;
                    case RevisionType.Unchanged:
                    {
                        RevisionMarker = IsRevisionMarker ? new RevisionMarker(uniqueId, revisionMarker.Properties.Author, (DateTime)revisionMarker.Properties.Date, RevisionMarker.RevisionType.Unchanged) : null;

                        VisitChildren(revisionMarker);
                    } break;
          
                }
            }

            IsRevisionMarker = false;
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
                var objTag = new Tag
                {
                    AlignmentAnchor = "",
                    Anchor = TagCounter.ToString(),
                    TagId = tagPair.TagProperties.TagId.Id,
                    TextEquivalent = tagPair.StartTagProperties.TagContent,
                    SectionType = Tag.Type.Start
                };

                TagUnits.Add(objTag);
                

                objTag.Revision = IsRevisionMarker ? RevisionMarker : null;
                SegmentSections.Add(objTag);

                PlainText.Append(tagPair.StartTagProperties.TagContent);




                var tgStart = new Sdl.LanguagePlatform.Core.Tag
                {
                    Type = TagType.Start,
                    Anchor = TagCounter,
                    TagID = tagPair.StartTagProperties.TagId.Id,
                    TextEquivalent = tagPair.StartTagProperties.DisplayText
                };
                Segment.Add(tgStart);
            }

            VisitChildren(tagPair);

            if (IncludeTagText)
            {
                var objTag = new Tag
                {
                    AlignmentAnchor = "",
                    Anchor = TagCounter.ToString(),
                    TagId = tagPair.TagProperties.TagId.Id,
                    TextEquivalent = tagPair.EndTagProperties.TagContent,
                    SectionType = Tag.Type.End
                };

                TagUnits.Add(objTag);

                objTag.Revision = IsRevisionMarker ? RevisionMarker : null;
                SegmentSections.Add(objTag);

                PlainText.Append(tagPair.EndTagProperties.TagContent);


                var tgEnd = new Sdl.LanguagePlatform.Core.Tag
                {
                    Type = TagType.End,
                    TagID = tagPair.StartTagProperties.TagId.Id,
                    Anchor = TagCounter,
                    TextEquivalent = tagPair.EndTagProperties.DisplayText
                };
                Segment.Add(tgEnd);
            }
            TagCounter++;
        }

        public void VisitText(IText text)
        {

            if (SegmentSections.Count > 0 && SegmentSections[SegmentSections.Count - 1].GetType() == typeof(Text))
            {
                var objTextPrevious = SegmentSections[SegmentSections.Count - 1] as Text;

  
                if (IsRevisionMarker)
                {
                    if (objTextPrevious != null && objTextPrevious.Revision != null && objTextPrevious.Revision.RevType == RevisionMarker.RevType)
                    {
                        objTextPrevious.Value += text.Properties.Text;
                        objTextPrevious.Revision = RevisionMarker;
                        SegmentSections[SegmentSections.Count - 1] = objTextPrevious;
                    }
                    else
                    {
                        var objText = new Text
                        {
                            Value = text.Properties.Text,
                            Revision = RevisionMarker
                        };
                        SegmentSections.Add(objText);                         
                    }                        
                }
                else
                {
                    if (objTextPrevious != null && objTextPrevious.Revision == null)
                    {
                        objTextPrevious.Value += text.Properties.Text;
                        objTextPrevious.Revision = null;
                        SegmentSections[SegmentSections.Count - 1] = objTextPrevious;
                    }
                    else
                    {
                        var objText = new Text
                        {
                            Value = text.Properties.Text,
                            Revision = null
                        };
                        SegmentSections.Add(objText);
                    }
                }
            }
            else
            {
                var objText = new Text
                {
                    Value = text.Properties.Text,
                    Revision = IsRevisionMarker ? RevisionMarker : null
                };
                SegmentSections.Add(objText);  
            }
    
            if ((!IsRevisionMarker || RevisionMarker.RevType == RevisionMarker.RevisionType.Delete) &&
                IsRevisionMarker)
                return;


            PlainText.Append(text.Properties.Text);
            Segment.Add(text.Properties.Text);
        }

        #endregion
    }
}
