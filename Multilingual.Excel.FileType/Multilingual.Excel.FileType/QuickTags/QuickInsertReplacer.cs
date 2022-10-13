using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;

namespace Multilingual.Excel.FileType.QuickTags
{
    internal static class QuickInsertReplacer
    {
        private static readonly IQuickInsertDefinitionsManager DefinitionsManager =
            new QuickInsertDefinitionsManager();

        private static readonly Dictionary<QuickInsertIds, IFormattingItem> QuickInsertFormattingItems =
            new Dictionary<QuickInsertIds, IFormattingItem>
            {
                [QuickInsertIds.qBold] =  new Bold(),
                [QuickInsertIds.qItalic] = new Italic(),
                [QuickInsertIds.qUnderline] = new Underline(),
                [QuickInsertIds.qSubscript] = new TextPosition(TextPosition.SuperSub.Subscript),
                [QuickInsertIds.qSuperscript] = new TextPosition(TextPosition.SuperSub.Superscript),
                [QuickInsertIds.qNoBold] = new Bold(false),
                [QuickInsertIds.qNoItalic] = new Italic(false),
                [QuickInsertIds.qNoUnderline] = new Underline(false),
                [QuickInsertIds.qNoSubscript] = new TextPosition(),
                [QuickInsertIds.qNoSuperscript] = new TextPosition(),
                [QuickInsertIds.qStrikeThrough] = new Strikethrough(),
                [QuickInsertIds.qNoStrikeThrough] = new Strikethrough(false)
            };

        public static bool TryGetFormattingItem(ITagPair tagPair, out IFormattingItem formattingItem)
        {
            if (DefinitionsManager.TryParseQuickInsertId(tagPair.StartTagProperties.TagId.Id, out var quickInsertId)
                && QuickInsertFormattingItems.TryGetValue(quickInsertId, out var mappedFormattingItem))
            {
                formattingItem = mappedFormattingItem;
                return true;
            }

            formattingItem = null;
            return false;
        }
    }
}