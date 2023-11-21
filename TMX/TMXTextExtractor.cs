using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.FileType.TMX
{
    class TMXTextExtractor : IMarkupDataVisitor
    {

        private List<string> _Comments = new List<string>();

        public List<string> GetSegmentComment(ISegment segment)
        {
            _Comments.Clear();
            VisitChildren(segment);
        
            return _Comments;
        }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            for (int i = 0; i < commentMarker.Comments.Count; i++)
            {
                string commentInfo = commentMarker.Comments.GetItem(i).Text + ";" +
                    commentMarker.Comments.GetItem(i).Date.ToString() + ";" +
                    commentMarker.Comments.GetItem(i).Author + ";" +
                    commentMarker.Comments.GetItem(i).Severity.ToString();

                _Comments.Add(commentInfo);

            }
            VisitChildren(commentMarker);
        }



        internal StringBuilder PlainText
        {
            get;
            set;
        }

        private string TryGetMetadata(ISegment segment, string metadataName)
        {
	        var meta = segment.Properties?.TranslationOrigin?.MetaData;
	        return meta?.FirstOrDefault(m => m.Key == metadataName).Value ?? null;
        }

        public string TryGetAuthor(ISegment segment)
        {
	        var author = TryGetMetadata(segment, "last_modified_by");
	        return author;
        }

        public DateTime? TryGetModifiedDate(ISegment segment)
        {
	        var modifiedDateStr = TryGetMetadata(segment, "modified_on");
			// IMPORTANT: the format date type is hardcoded ("MM/dd/yyyy HH:mm:ss"), because we need to properly parse on different machines as well
			if (modifiedDateStr != null && DateTime.TryParseExact(  modifiedDateStr, "MM/dd/yyyy HH:mm:ss", 
																	CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var modifiedDate))
		        return modifiedDate;
	        else
		        return null;
        }

        public string GetPlainText(ISegment segment)
        {
            PlainText = new StringBuilder("");
            VisitChildren(segment);
            
            string segContent = PlainText.ToString();
            return segContent;
        }


        // loops through all sub items of the container (IMarkupDataContainer)
        private void VisitChildren(IAbstractMarkupDataContainer container)
        {
            foreach (var item in container)
            {
                item.AcceptVisitor(this);
            }
        }


        public void VisitText(IText text)
        {
            string segContent = text.Properties.Text;
            segContent = segContent.Replace("&", "&amp;");
            segContent = segContent.Replace("<", "&lt;");
            segContent = segContent.Replace(">", "&gt;");
            segContent = segContent.Replace("'", "&apos;");
            PlainText.Append(segContent);
        }



        public void VisitTagPair(ITagPair tagPair)
        {
            PlainText.Append(tagPair.StartTagProperties.TagContent);
            VisitChildren(tagPair);
            PlainText.Append(tagPair.EndTagProperties.TagContent);
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
        #endregion


    }
}
