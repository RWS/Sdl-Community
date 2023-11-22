using System.Collections.Generic;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.QuickTags.Styles;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Multilingual.Excel.FileType.QuickTags
{
    public class BilingualExcelQuickTagBuilder : AbstractQuickTagBuilder
    {
        private const string FiltersResourceAssemblyName = "Sdl.FileTypeSupport.Filters.Resources";

        private const string ContentFormattingStartTag = FormattingConstants.ContentFormattingStart;
        private const string ContentFormattingEndTag = FormattingConstants.ContentFormattingEnd;
        private const string ContentFormattingDisplayName = FormattingConstants.ContentFormattingTag;

        private static readonly Dictionary<string, string> TagContentNameFormats =
            FormattingConstants.TagContentNameFormats;

        public IList<IQuickTag> BuildQuickTags()
        {
            return new List<IQuickTag>
            {
                CreateDefaultTagPair(QuickTagDefaultId.Bold, string.Format(TagContentNameFormats[Bold.Name], bool.TrueString)),
                CreateDefaultTagPair(QuickTagDefaultId.Italic,  string.Format(TagContentNameFormats[Italic.Name], bool.TrueString)),
                CreateDefaultTagPair(QuickTagDefaultId.Underline,  string.Format(TagContentNameFormats[Underline.Name], UnderlineStyle.single)),
                CreateDefaultTagPair(QuickTagDefaultId.Subscript,  string.Format(TagContentNameFormats[TextPosition.Name], TextPosition.SuperSub.Subscript.ToString().ToLowerInvariant())),
                CreateDefaultTagPair(QuickTagDefaultId.Superscript,
                    string.Format(TagContentNameFormats[TextPosition.Name],
                        TextPosition.SuperSub.Superscript.ToString().ToLowerInvariant())),

                CreateTagPair(QuickTagConstants.Strike,
                    string.Format(ContentFormattingStartTag, string.Format(TagContentNameFormats[Strikethrough.Name], bool.TrueString)),
                    ContentFormattingEndTag,
                    ContentFormattingDisplayName,
                    StringResources.StrikethroughName,
                    StringResources.StrikethroughDescription,
                    FiltersResourceAssemblyName,
                    $"{FiltersResourceAssemblyName}.quickTagStrike.ico", true,
                    CreateFormatting(new Strikethrough(true)),
                    true, false, false),

                CreateTagPair(QuickTagConstants.NoBold,
                    string.Format(ContentFormattingStartTag, string.Format(TagContentNameFormats[Bold.Name], bool.FalseString)),
                    ContentFormattingEndTag,
                    ContentFormattingDisplayName,
                    StringResources.NoBoldName,
                    StringResources.NoBoldDescription,
                    FiltersResourceAssemblyName,
                    $"{FiltersResourceAssemblyName}.quickTagNoBold.ico", false,
                    CreateFormatting(new Bold(false)),
                    true, false, false),

                CreateTagPair(QuickTagConstants.NoItalic,
                    string.Format(ContentFormattingStartTag, string.Format(TagContentNameFormats[Italic.Name], bool.FalseString)),
                    ContentFormattingEndTag,
                    ContentFormattingDisplayName,
                    StringResources.NoItalicsName,
                    StringResources.NoItalicsDescription,
                    FiltersResourceAssemblyName,
                    $"{FiltersResourceAssemblyName}.quickTagNoItalic.ico", false,
                    CreateFormatting(new Italic(false)),
                    true, false, false),

                CreateTagPair(QuickTagConstants.NoUnderlining,
                    string.Format(ContentFormattingStartTag, string.Format(TagContentNameFormats[Underline.Name], UnderlineStyle.none)),
                    ContentFormattingEndTag,
                    ContentFormattingDisplayName,
                    StringResources.NoUnderlineName,
                    StringResources.NoUnderlineDescription,
                    FiltersResourceAssemblyName,
                    $"{FiltersResourceAssemblyName}.quickTagNoUnderline.ico", false,
                    CreateFormatting(new Underline(false)),
                    true, false, false),

                CreateTagPair(QuickTagConstants.NoSuperscript,
                    string.Format(ContentFormattingStartTag, $" {TextPosition.SuperSub.Superscript.ToString().ToLowerInvariant()}=\"False\""),
                    ContentFormattingEndTag,
                    ContentFormattingDisplayName,
                    StringResources.NoSuperscriptName,
                    StringResources.NoSuperscriptDescription,
                    FiltersResourceAssemblyName,
                    $"{FiltersResourceAssemblyName}.quickTagNoSuperscript.ico", false,
                    CreateFormatting(new TextPosition(TextPosition.SuperSub.Normal)),
                    true, false, false),

                CreateTagPair(QuickTagConstants.NoSubscript,
                    string.Format(ContentFormattingStartTag, $" {TextPosition.SuperSub.Subscript.ToString().ToLowerInvariant()}=\"False\""),
                    ContentFormattingEndTag,
                    ContentFormattingDisplayName,
                    StringResources.NoSubscriptName,
                    StringResources.NoSubscriptDescription,
                    FiltersResourceAssemblyName,
                    $"{FiltersResourceAssemblyName}.quickTagNoSubscript.ico", false,
                    CreateFormatting(new TextPosition(TextPosition.SuperSub.Normal)),
                    true, false, false),

                CreateTagPair(QuickTagConstants.NoStrike,
                    string.Format(ContentFormattingStartTag, string.Format(TagContentNameFormats[Strikethrough.Name], bool.FalseString)),
                    ContentFormattingEndTag,
                    ContentFormattingDisplayName,
                    StringResources.NoStrikethroughName,
                    StringResources.NoStrikethroughDescription,
                    FiltersResourceAssemblyName,
                    $"{FiltersResourceAssemblyName}.quickTagNoStrike.ico", false,
                    CreateFormatting(new Strikethrough(false)),
                    true, false, false),

                CreateTextPair(QuickTagConstants.GermanQuotes, 
                    "„ “", StringResources.GermanQuotesQuickTagDescription,
                    "„", "“", "„ “",
                    FiltersResourceAssemblyName, 
                    $"{FiltersResourceAssemblyName}.quickTagSpanishQuotes.ico"),

                CreateTextPair(QuickTagConstants.EnglishStraightQuotes, 
                    "“ ”", StringResources.EnglishStraightQuotesQuickTagDescription,
                    "“", "”", "“ ”",
                    FiltersResourceAssemblyName, 
                    $"{FiltersResourceAssemblyName}.quickTagDoubleQuotes.ico"),

                CreateTextPair(QuickTagConstants.FrenchQuotes, 
                    "« »", StringResources.FrenchQuotesQuickTagDescription,
                    "« ", " »", "« »",
                    FiltersResourceAssemblyName, 
                    $"{FiltersResourceAssemblyName}.quickTagFrenchQuotes.ico")
            };
        }

        private IQuickTag CreateDefaultTagPair(QuickTagDefaultId id, string formattingItem)
        {
            var startTagContent = string.Format(ContentFormattingStartTag, formattingItem);
            return CreateDefaultTagPair(id, startTagContent, ContentFormattingEndTag, ContentFormattingDisplayName);
        }
    }
}