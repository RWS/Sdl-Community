using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.WordCloud.Plugin
{
    class PlainTextExtractor : IMarkupDataVisitor
    {
        public static string GetPlainText(IParagraph paragraph)
        {
            PlainTextExtractor e = new PlainTextExtractor();
            e.VisitChildren(paragraph);
            return e.PlainText;
        }

        public static string GetPlainText(ISegment segment)
        {
            PlainTextExtractor e = new PlainTextExtractor();
            e.VisitChildren(segment);
            return e.PlainText;
        }

        public PlainTextExtractor()
        {
            PlainText = "";
        }

        public string PlainText
        {
            get;
            set;
        }


        /// <summary>
        /// Iterates all sub items of container (IAbstractMarkupDataContainer)
        /// </summary>
        /// <param name="container"></param>
        private void VisitChildren(IEnumerable<IAbstractMarkupData> container)
        {
            foreach (var item in container)
            {
                if (item is IStructureTag)
                {
                    // skip structure tags
                    continue;
                }

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
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            VisitChildren(lockedContent.Content);
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            VisitChildren(marker);
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            //Ignore the placeholder
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            switch (revisionMarker.Properties.RevisionType)
            {
                case RevisionType.Delete:
                    //Deleted content is ignored
                    break;
                case RevisionType.Insert:
                    VisitChildren(revisionMarker);
                    break;
                case RevisionType.Unchanged:
                    VisitChildren(revisionMarker);
                    break;
            }
        }

        public void VisitSegment(ISegment segment)
        {
            VisitChildren(segment);
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            //Tag content is ignored only visit children
            VisitChildren(tagPair);
        }

        public void VisitText(IText text)
        {
            PlainText += text.ToString();
        }

        #endregion
    }
}
