using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Collections.Generic;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public class PlainTextExtractor : IMarkupDataVisitor
    {
        public string PlainText { get; set; }

        public PlainTextExtractor() { PlainText = ""; }

        public static string GetPlainText(IParagraph paragraph)
        {
            PlainTextExtractor textExtractor = new PlainTextExtractor();
            textExtractor.VisitChildren(paragraph);
            return textExtractor.PlainText;
        }

        public static string GetPlainText(ISegment segment)
        {
            PlainTextExtractor textExtractor = new PlainTextExtractor();
            textExtractor.VisitChildren(segment);
            return null;
        }

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
            VisitChildren(tagPair);
        }

        public void VisitText(IText text)
        {
            PlainText += text.ToString();
        }
        #endregion
    }
}
