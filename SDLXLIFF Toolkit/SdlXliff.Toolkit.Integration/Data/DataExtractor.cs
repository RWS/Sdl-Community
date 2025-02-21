using System.Collections.Generic;
using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace SdlXliff.Toolkit.Integration.Data
{
    class DataExtractor : IMarkupDataVisitor
    {
        public DataExtractor()
        {
            PlainText = new StringBuilder();
            Tags = new List<TagData>();
            LockedContent = new List<IndexData>();
        }

        /// <summary>
        /// extracted text
        /// </summary>
        public StringBuilder PlainText
        {
            get;
            private set;
        }

        /// <summary>
        /// tags inside segment text
        /// </summary>
        public List<TagData> Tags
        {
            get;
            private set;
        }

        /// <summary>
        /// locked content indexes inside segment text
        /// </summary>
        public List<IndexData> LockedContent
        {
            get;
            private set;
        }

        /// <summary>
        /// extract plain text
        /// </summary>
        /// <param name="segment">ISegment to process</param>
        /// <param name="isSearchLocked">true - search in locked elements</param>
        public void Process(ISegment segment)
        {
            PlainText = new StringBuilder();
            Tags = new List<TagData>();
            LockedContent = new List<IndexData>();

            VisitChildren(segment);
        }

        private void VisitChildren(IAbstractMarkupDataContainer container)
        {
            foreach (var item in container)
            {
                item.AcceptVisitor(this);
            }
        }

        #region IMarkupDataVisitor Members
        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            VisitChildren(commentMarker);
        }
        public void VisitLocationMarker(ILocationMarker location)
        {
            // not needed
        }
        public void VisitLockedContent(ILockedContent lockedContent)
        {
            LockedContent.Add(new IndexData(PlainText.Length, 0));

            VisitChildren((IAbstractMarkupDataContainer)lockedContent.Content);

            LockedContent[LockedContent.Count - 1].Length = PlainText.Length -
                LockedContent[LockedContent.Count - 1].IndexStart;
        }
        public void VisitOtherMarker(IOtherMarker marker)
        {
            // not needed
        }
        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            Tags.Add(new TagData(tag.Properties.TagId.Id,
                PlainText.Length,
                tag.Properties.TagContent));
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
            Tags.Add(new TagData(tagPair.StartTagProperties.TagId.Id,
                PlainText.Length,
                tagPair.StartTagProperties.TagContent));

            VisitChildren(tagPair);

            Tags.Add(new TagData(string.Empty,
                PlainText.Length,
                tagPair.EndTagProperties.TagContent));
        }
        public void VisitText(IText text)
        {
            PlainText.Append(text.Properties.Text);
        }
        #endregion
    }
}
