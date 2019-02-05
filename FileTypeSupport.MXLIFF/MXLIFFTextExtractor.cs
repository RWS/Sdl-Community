using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTypeSupport.MXLIFF
{
    internal class MXLIFFTextExtractor : IMarkupDataVisitor
    {
        private readonly List<IComment> comments = new List<IComment>();

        public List<IComment> GetSegmentComment(ISegment segment)
        {
            comments.Clear();
            VisitChildren(segment);

            return comments;
        }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            for (int i = 0; i < commentMarker.Comments.Count; i++)
            {
                comments.Add(commentMarker.Comments.GetItem(i));
            }

            VisitChildren(commentMarker);
        }

        internal StringBuilder PlainText
        {
            get;
            set;
        }

        public string GetPlainText(ISegment segment)
        {
            PlainText = new StringBuilder("");
            VisitChildren(segment);

            string segContent = PlainText.ToString();

            return segContent;
        }

        // Loops through all sub items of the container (IMarkupDataContainer)
        private void VisitChildren(IEnumerable<IAbstractMarkupData> container)
        {
            foreach (var item in container)
            {
                item.AcceptVisitor(this);
            }
        }

        public void VisitText(IText text)
        {
            string segContent = text.Properties.Text;

            PlainText.Append(segContent);
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            // Tag pairs should not be present
            throw new NotImplementedException();
        }

        #region "left empty"

        public void VisitSegment(ISegment segment)
        {
            VisitChildren(segment);
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            PlainText.Append(tag.TagProperties.TagContent);
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
        }

        #endregion "left empty"
    }
}