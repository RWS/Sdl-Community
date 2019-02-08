using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.CleanUpTasks
{
	public class LockHandler : SegmentHandlerBase, ISegmentHandler
    {
        private readonly IXmlReportGenerator reportGenerator = null;
        private readonly ICleanUpSourceSettings settings = null;
        private readonly StringBuilder stringBuilder = new StringBuilder();

        public LockHandler(ICleanUpSourceSettings settings,
                           IDocumentItemFactory itemFactory,
                           ICleanUpMessageReporter reporter,
                           IXmlReportGenerator reportGenerator)
            : base(itemFactory, reporter)
        {
            this.settings = settings;
            this.reportGenerator = reportGenerator;
        }

        public void VisitSegment(ISegment segment)
        {
            if (ShouldSkip(segment)) { return; }

            if (ShouldLockStructure(segment)) { return; }

            if (settings.UseContentLocker)
            {
                VisitChildren(segment);

                LockContent(segment);
            }
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            VisitChildren(tagPair);
        }

        public void VisitText(IText text)
        {
            var txt = text.Properties?.Text;

            if (!string.IsNullOrEmpty(txt))
            {
                stringBuilder.Append(txt);
            }
        }

        private void LockContent(ISegment segment)
        {
            var txt = stringBuilder.ToString();

            string reason;
            if (ShouldLockSegment(txt, out reason))
            {
                if (segment != null)
                {
                    var props = segment.Properties;
                    if (props != null)
                    {
                        props.IsLocked = true;

                        reportGenerator.AddLockItem(props.Id.Id, txt, $"Matched: {reason}");
                    }
                }
            }

            stringBuilder.Clear();
        }

        private bool ShouldLockSegment(string txt, out string reason)
        {
            var segmentLockList = settings.SegmentLockList;
            var isMatch = false;
            reason = string.Empty;

            foreach (var lockItem in segmentLockList)
            {
                if (!string.IsNullOrEmpty(lockItem.SearchText)) // Ignore empty items!
                {
                    if (lockItem.IsRegex)
                    {
                        isMatch = txt.RegexCompare(lockItem.SearchText, lockItem.IsCaseSensitive);
                    }
                    else if (lockItem.WholeWord)
                    {
                        isMatch = txt.RegexCompare("\\b" + Regex.Escape(lockItem.SearchText) + "\\b", lockItem.IsCaseSensitive);
                    }
                    else
                    {
                        isMatch = txt.NormalStringCompare(lockItem.SearchText, lockItem.IsCaseSensitive, settings.SourceCulture);
                    }

                    if (isMatch)
                    {
                        reason = lockItem.SearchText;
                        break;
                    }
                }
                else
                {
                    Reporter.Report(this, ErrorLevel.Warning, "Search text is empty", "Content Locker");
                }
            }

            return isMatch;
        }

        private bool ShouldLockStructure(ISegment segment)
        {
            if (settings.UseStructureLocker)
            {
                var paragraphUnit = segment.ParentParagraphUnit;
                var contexts = paragraphUnit?.Properties?.Contexts?.Contexts;
                if (contexts != null)
                {
                    foreach (var ctxInfo in contexts)
                    {
                        if (settings.StructureLockList.Any(ctxDef => ctxDef.IsMatch(ctxInfo)))
                        {
                            var props = segment.Properties;

                            if (props != null)
                            {
                                props.IsLocked = true;
                                reportGenerator.AddLockItem(props.Id.Id, "Structure", $"{ctxInfo.ContextType}");
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
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

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
        }

        #endregion Not Used
    }
}