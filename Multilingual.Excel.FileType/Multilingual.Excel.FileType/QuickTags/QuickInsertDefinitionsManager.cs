using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.QuickTags
{
    public class QuickInsertDefinitionsManager : IQuickInsertDefinitionsManager
    {
        private enum ResourceTypes
        {
            QuickInsertName = 0,
            QuickInsertDescription
        }

        public static IDocumentItemFactory ItemFactory { get; }

        public static IPropertiesFactory PropertiesFactory { get; }

        public static IFormattingItemFactory FormattingItemFactory { get; }

        static QuickInsertDefinitionsManager()
        {
	        ItemFactory = DefaultDocumentItemFactory.CreateInstance();
	        PropertiesFactory = DefaultPropertiesFactory.CreateInstance();
	        FormattingItemFactory = PropertiesFactory.FormattingItemFactory;
        }

        private static Dictionary<QuickInsertIds, IAbstractMarkupDataContainer> _markupData =
            new Dictionary<QuickInsertIds, IAbstractMarkupDataContainer>
            {
                {
                    QuickInsertIds.qBold,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<bold>", "</bold>", QuickInsertIds.qBold, true,
                            new FormattingGroup {{"Bold", FormattingItemFactory.CreateFormattingItem("Bold", "true")}})
                    }
                },
                {
                    QuickInsertIds.qItalic,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<italic>", "</italic>", QuickInsertIds.qItalic, true,
                            new FormattingGroup {{"Italic", FormattingItemFactory.CreateFormattingItem("Italic", "true")}})
                    }
                },
                {
                    QuickInsertIds.qUnderline,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<underline>", "</underline>", QuickInsertIds.qUnderline, true,
                            new FormattingGroup {{"Underline", FormattingItemFactory.CreateFormattingItem("Underline", "true")}})
                    }
                },
                {
                    QuickInsertIds.qNonBreakingHyphen,
                    new MarkupDataContainer {CreatePlaceholder("<non-breaking-hyphen/>", QuickInsertIds.qNonBreakingHyphen)}
                },
                {
                    QuickInsertIds.qOptionalHyphen,
                    new MarkupDataContainer {CreatePlaceholder("<optional-hyphen/>", QuickInsertIds.qOptionalHyphen)}
                },
                {
                    QuickInsertIds.qSmallCaps,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<small-caps>", "</small-caps>", QuickInsertIds.qSmallCaps, false,
                            new FormattingGroup {{"smallcaps", FormattingItemFactory.CreateFormattingItem("smallcaps", "true")}})
                    }
                },
                {
                    QuickInsertIds.qSubscript,
                    new MarkupDataContainer
                        {CreateTagPair("<subscript>", "</subscript>", QuickInsertIds.qSubscript, true,
                            new FormattingGroup {{"TextPosition", new TextPosition(TextPosition.SuperSub.Subscript)}})}
                },
                {
                    QuickInsertIds.qSuperscript,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<superscript>", "</superscript>", QuickInsertIds.qSuperscript, true,
                            new FormattingGroup {{"TextPosition", new TextPosition(TextPosition.SuperSub.Superscript)}})
                    }
                },
                {
                    QuickInsertIds.qNoBold,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<no-bold>", "</no-bold>", QuickInsertIds.qNoBold, true,
                            new FormattingGroup {{"Bold", FormattingItemFactory.CreateFormattingItem("Bold", "false")}})
                    }
                },
                {
                    QuickInsertIds.qNoItalic,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<no-italic>", "</no-italic>", QuickInsertIds.qNoItalic, true,
                            new FormattingGroup {{"Italic", FormattingItemFactory.CreateFormattingItem("Italic", "false")}})
                    }
                },
                {
                    QuickInsertIds.qNoUnderline,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<no-underline>", "</no-underline>", QuickInsertIds.qNoUnderline, true,
                            new FormattingGroup {{"Underline", FormattingItemFactory.CreateFormattingItem("Underline", "false")}})
                    }
                },
                {
                    QuickInsertIds.qNoSubscript,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<no-subscript>", "</no-subscript>", QuickInsertIds.qNoSubscript, true,
                            new FormattingGroup {{"TextPosition", new TextPosition(TextPosition.SuperSub.Normal)}})
                    }
                },
                {
                    QuickInsertIds.qNoSuperscript,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<no-superscript>", "</no-superscript>", QuickInsertIds.qNoSuperscript, true,
                            new FormattingGroup {{"TextPosition", new TextPosition(TextPosition.SuperSub.Normal)}})
                    }
                },
                {
                    QuickInsertIds.qStrikeThrough,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<strike-through>", "</strike-through>", QuickInsertIds.qStrikeThrough, true,
                            new FormattingGroup {{"Strikethrough", FormattingItemFactory.CreateFormattingItem("Strikethrough", "true")}})
                    }
                },
                {
                    QuickInsertIds.qNoStrikeThrough,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<no-strike-through>", "</no-strike-through>", QuickInsertIds.qNoStrikeThrough, true,
                            new FormattingGroup {{"Strikethrough", FormattingItemFactory.CreateFormattingItem("Strikethrough", "false")}})
                    }
                },
                {
                    QuickInsertIds.qNoSmallCaps,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<no-small-caps>", "</no-small-caps>", QuickInsertIds.qNoSmallCaps, false,
                            new FormattingGroup {{"smallcaps", FormattingItemFactory.CreateFormattingItem("smallcaps", "false")}})
                    }
                },
                {
                    QuickInsertIds.qDoubleUnderline,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<double-underline>", "</double-underline>", QuickInsertIds.qDoubleUnderline, true,
                            new FormattingGroup {{"Underline", FormattingItemFactory.CreateFormattingItem("Underline", "true")}})
                    }
                },
                {
                    QuickInsertIds.qNoDoubleUnderline,
                    new MarkupDataContainer
                    {
                        CreateTagPair("<no-double-underline>", "</no-double-underline>", QuickInsertIds.qNoDoubleUnderline, true,
                            new FormattingGroup {{"Underline", FormattingItemFactory.CreateFormattingItem("Underline", "false")}})
                    }
                },
                {
                    QuickInsertIds.qSortOrder,
                    new MarkupDataContainer {CreateTagPair("<sort-order>", "</sort-order>", QuickInsertIds.qSortOrder, false, null)}
                }
            };

        private static IAbstractMarkupData CreatePlaceholder(string tagContent, QuickInsertIds tagId)
        {
            var placeholder = ItemFactory.CreatePlaceholderTag(ItemFactory.PropertiesFactory.CreatePlaceholderTagProperties(tagContent));
            placeholder.TagProperties.TagId = new TagId(BuildQuickTagId(tagId));
            SetDisplayText(placeholder.TagProperties, tagId);

            return placeholder;
        }

        private static void SetDisplayText(IAbstractBasicTagProperties tagProperties, QuickInsertIds tagId)
        {
            tagProperties.DisplayText = QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, tagId);
        }

        private static IAbstractMarkupData CreateTagPair(
            string startTagContent,
            string endTagContent,
            QuickInsertIds tagId,
            bool canHide,
            IFormattingGroup formattingGroup)
        {
            var startTagProperties = ItemFactory.PropertiesFactory.CreateStartTagProperties(startTagContent);
            SetDisplayText(startTagProperties, tagId);
            startTagProperties.TagId = new TagId(BuildQuickTagId(tagId));
            startTagProperties.CanHide = canHide;

            if (formattingGroup != null)
                startTagProperties.Formatting = formattingGroup;

            var endTagProperties = ItemFactory.PropertiesFactory.CreateEndTagProperties(endTagContent);
            SetDisplayText(endTagProperties, tagId);
            endTagProperties.CanHide = canHide;

            var tagPair = ItemFactory.CreateTagPair(startTagProperties, endTagProperties);
            return tagPair;
        }

        private static Dictionary<QuickInsertIds, QuickInsert> _definitions = new Dictionary<QuickInsertIds, QuickInsert>
        {
            {
                QuickInsertIds.qBold,
                new QuickInsert(
                    QuickInsertIds.qBold,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qBold),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qBold),
                    _markupData[QuickInsertIds.qBold],
                    (_markupData[QuickInsertIds.qBold][0] as ITagPair)?.StartTagProperties.Formatting,
                    true )
            },
            {
                QuickInsertIds.qItalic,
                new QuickInsert(
                    QuickInsertIds.qItalic,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qItalic),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qItalic),
                    _markupData[QuickInsertIds.qItalic],
                    (_markupData[QuickInsertIds.qItalic][0] as ITagPair)?.StartTagProperties.Formatting,
                    true)
            },
            {
                QuickInsertIds.qUnderline,
                new QuickInsert(
                    QuickInsertIds.qUnderline,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qUnderline),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qUnderline),
                    _markupData[QuickInsertIds.qUnderline],
                    (_markupData[QuickInsertIds.qUnderline][0] as ITagPair)?.StartTagProperties.Formatting,
                    true)
            },
            {
                QuickInsertIds.qNonBreakingHyphen,
                new QuickInsert(
                    QuickInsertIds.qNonBreakingHyphen,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNonBreakingHyphen),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNonBreakingHyphen),
                    _markupData[QuickInsertIds.qNonBreakingHyphen],
                    null,
                    true)
            },
            {
                QuickInsertIds.qOptionalHyphen,
                new QuickInsert(
                    QuickInsertIds.qOptionalHyphen,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qOptionalHyphen),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qOptionalHyphen),
                    _markupData[QuickInsertIds.qOptionalHyphen],
                    null,
                    true)
            },
            {
                QuickInsertIds.qSubscript,
                new QuickInsert(
                    QuickInsertIds.qSubscript,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qSubscript),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qSubscript),
                    _markupData[QuickInsertIds.qSubscript],
                    (_markupData[QuickInsertIds.qSubscript][0] as ITagPair)?.StartTagProperties.Formatting,
                    true)
            },
            {
                QuickInsertIds.qSuperscript,
                new QuickInsert(
                    QuickInsertIds.qSuperscript,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qSuperscript),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qSuperscript),
                    _markupData[QuickInsertIds.qSuperscript],
                    (_markupData[QuickInsertIds.qSuperscript][0] as ITagPair)?.StartTagProperties.Formatting,
                    true)
            },
            {
                QuickInsertIds.qSmallCaps,
                new QuickInsert(
                    QuickInsertIds.qSmallCaps,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qSmallCaps),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qSmallCaps),
                    _markupData[QuickInsertIds.qSmallCaps],
                    (_markupData[QuickInsertIds.qSmallCaps][0] as ITagPair)?.StartTagProperties.Formatting,
                    true)
            },
            {
                QuickInsertIds.qStrikeThrough,
                new QuickInsert(
                    QuickInsertIds.qStrikeThrough,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qStrikeThrough),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qStrikeThrough),
                    _markupData[QuickInsertIds.qStrikeThrough],
                    (_markupData[QuickInsertIds.qStrikeThrough][0] as ITagPair)?.StartTagProperties.Formatting,
                    true)
            },
            {
                QuickInsertIds.qDoubleUnderline,
                new QuickInsert(
                    QuickInsertIds.qDoubleUnderline,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qDoubleUnderline),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qDoubleUnderline),
                    _markupData[QuickInsertIds.qDoubleUnderline],
                    (_markupData[QuickInsertIds.qDoubleUnderline][0] as ITagPair)?.StartTagProperties.Formatting,
                    true)
            },
            {
                QuickInsertIds.qSortOrder,
                new QuickInsert(
                    QuickInsertIds.qSortOrder,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qSortOrder),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qSortOrder),
                    _markupData[QuickInsertIds.qSortOrder],
                    (_markupData[QuickInsertIds.qSortOrder][0] as ITagPair)?.StartTagProperties.Formatting,
                    true)
            },
            {
                QuickInsertIds.qNoDoubleUnderline,
                new QuickInsert(
                    QuickInsertIds.qNoDoubleUnderline,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoDoubleUnderline),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoDoubleUnderline),
                    _markupData[QuickInsertIds.qNoDoubleUnderline],
                    (_markupData[QuickInsertIds.qNoDoubleUnderline][0] as ITagPair)?.StartTagProperties.Formatting,
                    false)
            },
            {
                QuickInsertIds.qNoSmallCaps,
                new QuickInsert(
                    QuickInsertIds.qNoSmallCaps,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoSmallCaps),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoSmallCaps),
                    _markupData[QuickInsertIds.qNoSmallCaps],
                    (_markupData[QuickInsertIds.qNoSmallCaps][0] as ITagPair)?.StartTagProperties.Formatting,
                    false)
            },
            {
                QuickInsertIds.qNoStrikeThrough,
                new QuickInsert(
                    QuickInsertIds.qNoStrikeThrough,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoStrikeThrough),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoStrikeThrough),
                    _markupData[QuickInsertIds.qNoStrikeThrough],
                    (_markupData[QuickInsertIds.qNoStrikeThrough][0] as ITagPair)?.StartTagProperties.Formatting,
                    false)
            },
            {
                QuickInsertIds.qNoBold,
                new QuickInsert(
                    QuickInsertIds.qNoBold,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoBold),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoBold),
                    _markupData[QuickInsertIds.qNoBold],
                    (_markupData[QuickInsertIds.qNoBold][0] as ITagPair)?.StartTagProperties.Formatting,
                    false)
            },
            {
                QuickInsertIds.qNoItalic,
                new QuickInsert(
                    QuickInsertIds.qNoItalic,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoItalic),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoItalic),
                    _markupData[QuickInsertIds.qNoItalic],
                    (_markupData[QuickInsertIds.qNoItalic][0] as ITagPair)?.StartTagProperties.Formatting,
                    false)
            },
            {
                QuickInsertIds.qNoUnderline,
                new QuickInsert(
                    QuickInsertIds.qNoUnderline,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoUnderline),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoUnderline),
                    _markupData[QuickInsertIds.qNoUnderline],
                    (_markupData[QuickInsertIds.qNoUnderline][0] as ITagPair)?.StartTagProperties.Formatting,
                    false)
            },
            {
                QuickInsertIds.qNoSubscript,
                new QuickInsert(
                    QuickInsertIds.qNoSubscript,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoSubscript),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoSubscript),
                    _markupData[QuickInsertIds.qNoSubscript],
                    (_markupData[QuickInsertIds.qNoSubscript][0] as ITagPair)?.StartTagProperties.Formatting,
                    false)
            },
            {
                QuickInsertIds.qNoSuperscript,
                new QuickInsert(
                    QuickInsertIds.qNoSuperscript,
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertName, QuickInsertIds.qNoSuperscript),
                    QuickInsertResources.GetEnumerationString(ResourceTypes.QuickInsertDescription, QuickInsertIds.qNoSuperscript),
                    _markupData[QuickInsertIds.qNoSuperscript],
                    (_markupData[QuickInsertIds.qNoSuperscript][0] as ITagPair)?.StartTagProperties.Formatting,
                    false)
            }
        };

        public IQuickInsert BuildQuickInsert(QuickInsertIds id)
        {
            return _definitions[id];
        }

        public IQuickInsert BuildClonedQuickInsert(QuickInsertIds id)
        {
            return _definitions[id].Clone() as IQuickInsert;
        }

        public bool IsQuickInsert(IAbstractMarkupData item)
        {
            if (!(item is ITagPair || item is IPlaceholderTag))
            {
                return false;
            }
            var abstractTag = item as IAbstractTag;

            return TryParseQuickInsertId(abstractTag.TagProperties.TagId.Id, out var quickTag);
        }

        public bool TryParseQuickInsertId(string tagId, out QuickInsertIds quickTag)
        {
            foreach (var enumValue in Enum.GetValues(typeof(QuickInsertIds)))
            {
                if (BuildQuickTagId((QuickInsertIds)enumValue) == tagId)
                {
                    quickTag = (QuickInsertIds)enumValue;
                    return true;
                }
            }

            quickTag = QuickInsertIds.None;
            return false;
        }

        private static string BuildQuickTagId(QuickInsertIds id)
        {
            return id.ToString();
        }

        public List<QuickInsertIds> ParseQuickInsertIds(string quickInsertIds)
        {
            var resultsList = new List<QuickInsertIds>();

            if (string.IsNullOrEmpty(quickInsertIds))
            {
                return null;
            }

            var items = quickInsertIds.Split(';');

            QuickInsertIds currentId;

            foreach (var item in items)
            {
                var success = TryParseQuickInsertId(item, out currentId);
                if (success)
                {
                    resultsList.Add(currentId);
                }
            }

            return resultsList;
        }

        private static class QuickInsertResources
        {
            public static string GetEnumerationString(ResourceTypes type, Enum enumeration)
            {
                string resourceName =
                    string.Concat(type.ToString(), "_", enumeration);
                var result = StringResources.ResourceManager.GetString(resourceName);
                return string.IsNullOrEmpty(result) ? string.Empty : result;
            }
        }
    }
}
