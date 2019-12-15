using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.Community.CleanUpTasks.Models;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.CleanUpTasks
{
	public class NonTranslatableHandler : ISegmentHandler
    {
        private readonly List<ConversionItemList> conversionItemLists = null;
        private readonly IXmlReportGenerator reportGenerator = null;
        private readonly ISettings settings = null;
        private readonly List<IText> textList = new List<IText>();

        public NonTranslatableHandler(ISettings settings,
                                      List<ConversionItemList> conversionItems,
                                      IXmlReportGenerator reportGenerator)
        {
            this.settings = settings;
            conversionItemLists = conversionItems;
            this.reportGenerator = reportGenerator;
        }

        public void ProcessText()
        {
            if (textList.Count == 0)
            {
                return;
            }

            foreach (var itemList in conversionItemLists)
            {
                foreach (var item in itemList.Items)
                {
                    ProcessInternal(item);
                }
            }

            textList.Clear();
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
            textList.Add(text);
        }

        private void ProcessInternal(ConversionItem item)
        {
            if (ShouldSkip(item)) { return; }

            var search = item.Search;
            var replacement = item.Replacement;

            foreach (var text in textList)
            {
                var original = text.Properties.Text;

                if (!string.IsNullOrEmpty(original))
                {
                    string updatedText;
                    var replaceSuccessful = TryReplaceString(text.Properties.Text, item, search, out updatedText);

                    if (replaceSuccessful)
                    {
                        reportGenerator.AddConversionItem("Non translatable", original, updatedText, item.Search.Text, item.Replacement.Text);

                        text.Properties.Text = updatedText;
                    }
                }
            }
        }

        private bool ShouldSkip(ConversionItem item)
        {
            var search = item.Search;
            var replacement = item.Replacement;

            return search.TagPair || search.EmbeddedTags || replacement.Placeholder;
        }

        private bool TryNormalStringUpdate(string original, ConversionItem item, out string updatedText)
        {
            var search = item.Search;
            var replacement = item.Replacement;

            updatedText = null;

            if (original.NormalStringCompare(search.Text, search.CaseSensitive, settings.SourceCulture))
            {
                updatedText = original.NormalStringReplace(search.Text, replacement.Text, search.CaseSensitive);
            }

            return updatedText != null;
        }

        private bool TryRegexUpdate(string original, ConversionItem item, out string updatedText, bool wholeWord = false)
        {
            var search = item.Search;
            var replacement = item.Replacement;

            string searchText;
            if (wholeWord)
            {
                searchText = "\\b" + Regex.Escape(search.Text) + "\\b";
            }
            else
            {
                searchText = search.Text;
            }

            updatedText = null;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                if (original.RegexCompare(searchText, search.CaseSensitive))
                {
                    updatedText = original.RegexReplace(searchText, replacement, search.CaseSensitive);
                }
            }

            // Because the method returns input unchanged if there is no match, you can use
            // the Object.ReferenceEquals method to determine whether the method has made any
            // replacements to the input string.
            return (updatedText != null) && !object.ReferenceEquals(original, updatedText);
        }

        private bool TryReplaceString(string original, ConversionItem item, SearchText search, out string updatedText)
        {
            var result = false;

            if (search.UseRegex)
            {
                result = TryRegexUpdate(original, item, out updatedText);
            }
            else if (search.WholeWord)
            {
                result = TryRegexUpdate(original, item, out updatedText, true);
            }
            else
            {
                result = TryNormalStringUpdate(original, item, out updatedText);
            }

            return result;
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