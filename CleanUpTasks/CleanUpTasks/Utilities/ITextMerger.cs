using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	internal class ITextMerger : ISegmentHandler
    {
        private readonly List<List<IText>> textList = new List<List<IText>>();
        private bool createNewList = true;

        public void Merge()
        {
            foreach (var list in textList)
            {
                StringBuilder builder = new StringBuilder();

                // Collect all text together
                foreach (var txt in list)
                {
                    builder.Append(txt.Properties.Text);
                }

                // Put all text into first IText
                var first = list.First();
                first.Properties.Text = builder.ToString();

                // Remove other ITexts
                foreach (var txt in list.Skip(1))
                {
                    txt.RemoveFromParent();
                }
            }
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            // When we encounter a IPlaceholderTag, we set
            // createNewList to true
            createNewList = true;
        }

        public void VisitSegment(ISegment segment)
        {
            VisitChildren(segment);
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            // When we encounter a ITagPair, we set
            // createNewList to true
            createNewList = true;

            VisitChildren(tagPair);
        }

        public void VisitText(IText text)
        {
            if (createNewList)
            {
                // Create and add a new list
                var list = new List<IText>() { text };
                textList.Add(list);

                createNewList = false;
            }
            else
            {
                var last = textList.Last();
                last.Add(text);
            }
        }

        private void VisitChildren(IAbstractMarkupDataContainer container)
        {
            foreach (var item in container)
            {
                item.AcceptVisitor(this);
            }
        }

        #region Not Used

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
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

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
        }

        #endregion Not Used
    }
}