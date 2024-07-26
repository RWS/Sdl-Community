﻿using System.Collections.Generic;
using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.FileTypeSupport.TXML
{
    class TxmlTextExtractor : IMarkupDataVisitor
    {

        private readonly List<string> _comments = new List<string>();

        public List<string> GetSegmentComment(ISegment segment)
        {
            _comments.Clear();
            VisitChildren(segment);
        
            return _comments;
        }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            for (int i = 0; i < commentMarker.Comments.Count; i++)
            {
                string commentInfo = commentMarker.Comments.GetItem(i).Text + ";" +
                    commentMarker.Comments.GetItem(i).Date + ";" +
                    commentMarker.Comments.GetItem(i).Author + ";" +
                    commentMarker.Comments.GetItem(i).Severity;

                _comments.Add(commentInfo);

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


        // loops through all sub items of the container (IMarkupDataContainer)
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
            segContent = segContent.Replace("&", "&amp;");
            segContent = segContent.Replace("<", "&lt;");
            segContent = segContent.Replace(">", "&gt;");
            segContent = segContent.Replace("'", "&apos;");
            PlainText.Append(segContent);
        }



        public void VisitTagPair(ITagPair tagPair)
        {
            PlainText.Append("<" + tagPair.StartTagProperties.TagContent + ">");
            VisitChildren(tagPair);
            PlainText.Append("</" + tagPair.EndTagProperties.TagContent + ">");
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
