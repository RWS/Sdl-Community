using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.AdvancedDisplayFilter.Content
{
    public class ContentProcessor : IMarkupDataVisitor
    {

        public ContentProcessor()
        {
            Initialize();
        }

        private void Initialize()
        {
            PlainText = new StringBuilder("");
            Comments  = new List<IComment>();
        }
        public List<IComment> Comments { get; set; }
        public StringBuilder PlainText { get; set; }

        public bool IncludeTagContent { get; set; }

        public bool IgnoreComments { get; set; }

        public string GetPlainText(ISegment segment, bool includeTagContent)
        {
            IgnoreComments = true;
            IncludeTagContent = includeTagContent;
            Initialize(); 
            VisitChildren(segment);
            return PlainText.ToString();
        }

        public List<IComment> GetSegmentComments(ISegment segment)
        {
            IgnoreComments = false;
            Initialize();
            VisitChildren(segment);
            return Comments;
        }

        public void ProcessSegment(ISegment segment)
        {
            Initialize();
            VisitChildren(segment);
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

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            if (!IgnoreComments)
            {
                foreach (var commentProperty in commentMarker.Comments.Comments)
                {
                    Comments.Add(commentProperty);
                }
            }
            VisitChildren(commentMarker);
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            // Not required for this implementation
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {            
            PlainText.Append(lockedContent.Content.ToString());
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            VisitChildren(marker);
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            if (IncludeTagContent)
                PlainText.Append("<" + tag.Properties.TagContent + "/>");
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
            if (IncludeTagContent)
                PlainText.Append("<" + tagPair.StartTagProperties.TagContent + ">");
            VisitChildren(tagPair);
            if (IncludeTagContent)
                PlainText.Append("</" + tagPair.EndTagProperties.TagContent + ">");
        }

        public void VisitText(IText text)
        {
            PlainText.Append(text.Properties.Text);
        }
    }
}
