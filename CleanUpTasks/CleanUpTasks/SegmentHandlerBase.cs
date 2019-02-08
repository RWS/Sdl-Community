using System;
using System.Text.RegularExpressions;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.CleanUpTasks
{
	public class SegmentHandlerBase
    {
        public SegmentHandlerBase(IDocumentItemFactory itemFactory, ICleanUpMessageReporter reporter)
        {
            ItemFactory = itemFactory;
            Reporter = reporter;
        }

        protected IDocumentItemFactory ItemFactory { get; }

        protected ICleanUpMessageReporter Reporter { get; }

        protected static bool ShouldSkip(ISegment segment)
        {  
            var props = segment.Properties;

            if (props != null)
            {
                return props.IsLocked;
            }

            return false;
        }

        protected IEndTagProperties CreateEndTag(string endTag)
        {
            var endTagProps = ItemFactory.PropertiesFactory.CreateEndTagProperties(endTag);
            endTagProps.TagContent = endTag;
            endTagProps.DisplayText = endTag;

            return endTagProps;
        }

        protected IFormattingGroup CreateFormattingGroup()
        {
            return ItemFactory.PropertiesFactory.FormattingItemFactory.CreateFormatting();
        }

        protected IFormattingItem CreateFormattingItem(string name, string value)
        {
            return ItemFactory.PropertiesFactory.FormattingItemFactory.CreateFormattingItem(name, value);
        }

        protected IAbstractMarkupData CreateIText(string text)
        {
			   var props = ItemFactory.PropertiesFactory.CreateTextProperties(text);
            var itext = ItemFactory.CreateText(props);

            return itext;
        }

        protected IPlaceholderTag CreatePlaceHolderTag(Match match)
        {	   
            var placeHolderTagProps = ItemFactory.PropertiesFactory.CreatePlaceholderTagProperties(match.Value);
            placeHolderTagProps.TagContent = match.Value;

            if (!string.IsNullOrEmpty(match.Groups[1].Value))
            {
                placeHolderTagProps.DisplayText = match.Groups[1].Value;
            }

            return CreatePlaceHolderTagInternal(match.Value, placeHolderTagProps);
        }

        protected IPlaceholderTag CreatePlaceHolderTag(string text)
        {
            var placeHolderTagProps = ItemFactory.PropertiesFactory.CreatePlaceholderTagProperties(text);
            placeHolderTagProps.TagContent = text;
            placeHolderTagProps.TagId = new TagId(Guid.NewGuid().ToString());

            // Only show attribute value
            var m = Regex.Match(text, @"<\w+\s+\w+?=""(.+)""[\s\/]*>");
            if (m.Success)
            {
                placeHolderTagProps.DisplayText = $"{m.Groups[1].Value}";
            }
            else
            {
                placeHolderTagProps.DisplayText = text;
            }

            return CreatePlaceHolderTagInternal(text, placeHolderTagProps);
        }

        protected IStartTagProperties CreateStartTag(string startTag)
        {
            var startTagProps = ItemFactory.PropertiesFactory.CreateStartTagProperties(startTag);
            startTagProps.TagContent = startTag;
            startTagProps.TagId = new TagId(Guid.NewGuid().ToString());

            var m = Regex.Match(startTag, @"<(\w+)\s+\w+.*?>");
            if (m.Success)
            {
                startTagProps.DisplayText = $"<{m.Groups[1].Value}>";
            }
            else
            {
                startTagProps.DisplayText = startTag;
            }

            return startTagProps;
        }

        protected ITagPair CreateTagPair(IStartTagProperties startTag, IEndTagProperties endTag)
        {
            return ItemFactory.CreateTagPair(startTag, endTag);
        }

        protected ISegment CreateTargetSegment()
        {
            var segPairProps = ItemFactory.CreateSegmentPairProperties();
            var target = ItemFactory.CreateSegment(segPairProps);
            return target;
        }

        private IPlaceholderTag CreatePlaceHolderTagInternal(string text, IPlaceholderTagProperties placeHolderTagProps)
        {
            return ItemFactory.CreatePlaceholderTag(placeHolderTagProps);
        }

        private bool HasAttribute(string value)
        {
            if (Regex.IsMatch(value, "<[^\\=]+?=\"[^\\=]+?\"\\s*\\/>"))
            {
                return true;
            }
            else
            {
                Reporter.Report((ISegmentHandler)this, ErrorLevel.Error, "Invalid tag", value);
                return false;
            }
        }
    }
}