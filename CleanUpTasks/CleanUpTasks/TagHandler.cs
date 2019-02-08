using System.Collections.Generic;
using System.Linq;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.CleanUpTasks
{
	public class TagHandler : SegmentHandlerBase, ISegmentHandler
    {
        private readonly bool fmtAllUnchecked = false;
        private readonly Stack<ITagPair> fmtTagsToRemove = new Stack<ITagPair>();
        private readonly IVerifyingFormattingVisitor fmtVisitor = null;
        private readonly bool phAllUnchecked = false;
        private readonly Stack<IPlaceholderTag> phTagsToRemove = new Stack<IPlaceholderTag>();
        private readonly IXmlReportGenerator reportGenerator = null;
        private readonly ICleanUpSourceSettings settings = null;
        public TagHandler(ICleanUpSourceSettings settings,
                          IVerifyingFormattingVisitor fmtVisitor,
                          IDocumentItemFactory itemFactory,
                          ICleanUpMessageReporter reporter,
                          IXmlReportGenerator reportGenerator)
            : base(itemFactory, reporter)
        {
            this.settings = settings;
            this.fmtVisitor = fmtVisitor;
            this.reportGenerator = reportGenerator;

            // Check whether all checkboxes are unchecked and cache the result
            fmtAllUnchecked = settings.FormatTagList.Values.All(isChecked => isChecked == false);
            phAllUnchecked = settings.PlaceholderTagList.Values.All(isChecked => isChecked == false);
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            if (!phAllUnchecked)
            {
                var content = tag.Properties.TagContent;

                if (!string.IsNullOrEmpty(content))
                {
                    bool isChecked;
                    if (settings.PlaceholderTagList.TryGetValue(content, out isChecked))
                    {
                        if (isChecked)
                        {
                            phTagsToRemove.Push(tag);
                        }
                    }
                }
            }
        }

        public void VisitSegment(ISegment segment)
        {
            if (ShouldSkip(segment)) { return; }

            VisitChildren(segment);

            ProcessTags();

            // Merge all adjacent IText
            ITextMerger merger = new ITextMerger();
            merger.VisitSegment(segment);
            merger.Merge();
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            if (!fmtAllUnchecked)
            {
                var formatting = tagPair.StartTagProperties?.Formatting;

                if (formatting != null)
                {
                    foreach (var fmtItem in formatting.Values)
                    {
                        fmtItem.AcceptVisitor(fmtVisitor);
                    }

                    if (fmtVisitor.ShouldRemoveTag())
                    {
                        fmtTagsToRemove.Push(tagPair);
                    }

                    // Reset the verifier for each TagPair!!
                    // Otherwise, the verifier keeps the settings from the
                    // previous one
                    fmtVisitor.ResetVerifier();
                }
            }

            VisitChildren(tagPair);
        }

        private static void ReAddSubItemsToParent(List<IAbstractMarkupData> subItemList, IAbstractMarkupDataContainer parent, int index)
        {
            if (subItemList != null)
            {
                // Reinsert at same index if not at end of container
                if (parent.Count > index)
                {
                    foreach (var subItem in subItemList)
                    {
                        subItem.RemoveFromParent();
                        parent.Insert(index, subItem);
                        index++;
                    }
                }
                else
                {
                    foreach (var subItem in subItemList)
                    {
                        subItem.RemoveFromParent();
                        parent.Add(subItem);
                    }
                }
            }
        }

        private void Log(IAbstractMarkupData data, string tagContent)
        {
            IAbstractMarkupDataContainer parent = data.Parent;
            while (!(parent is ISegment))
            {
                parent = ((IAbstractMarkupData)parent).Parent;
            }

            var segment = (ISegment)parent;
            reportGenerator.AddTagItem(segment.Properties.Id.Id, tagContent);
        }

        private void ProcessTags()
        {
            // Formatting tags
            while (fmtTagsToRemove.Count > 0)
            {
                var tagPair = fmtTagsToRemove.Pop();

                List<IAbstractMarkupData> subItemList = null;
                if (tagPair.AllSubItems != null)
                {
                    subItemList = tagPair.AllSubItems.ToList();
                }

                var parent = tagPair.Parent;
                var index = tagPair.IndexInParent;

                Log(tagPair, tagPair.TagProperties.TagContent);

                tagPair.RemoveFromParent();

                ReAddSubItemsToParent(subItemList, parent, index);
            }

            // Placeholder tags
            while (phTagsToRemove.Count > 0)
            {
                var phTag = phTagsToRemove.Pop();

                Log(phTag, phTag.Properties.TagContent);

                phTag.RemoveFromParent();
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

        public void VisitText(IText text)
        {
        }

        #endregion Not Used
    }
}