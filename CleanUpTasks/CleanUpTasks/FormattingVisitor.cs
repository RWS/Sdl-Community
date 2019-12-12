using System.Collections.Generic;
using System.Linq;
using Sdl.FileTypeSupport.Framework.Formatting;
using SDLCommunityCleanUpTasks.Models;

namespace SDLCommunityCleanUpTasks
{
	public class FormattingVisitor : IVerifyingFormattingVisitor
    {
        private readonly Dictionary<string, bool> verifier =
                    new Dictionary<string, bool>()
                    {
                        { TagTable.BackgroundColor, false },
                        { TagTable.Bold, false },
                        { TagTable.FontName, false },
                        { TagTable.FontSize, false },
                        { TagTable.TextColor, false },
                        { TagTable.Italic, false },
                        { TagTable.TextDirection, false },
                        { TagTable.TextPosition, false },
                        { TagTable.Underline, false },
                        { TagTable.Strikethrough, false }
                    };

        private readonly ICleanUpSourceSettings settings = null;

        public FormattingVisitor(ICleanUpSourceSettings settings)
        {

            this.settings = settings;
        }

        public void ResetVerifier()
        {
            foreach (var key in verifier.Keys.ToList())
            {
                verifier[key] = false;
            }
        }

        public bool ShouldRemoveTag()
        {
            if (AllSettingsMatch())
            {
                return true;
            }

            if (ShouldNotRemoveTagExists())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void VisitBackgroundColor(BackgroundColor item)
        {
            verifier[TagTable.BackgroundColor] = item.Value != null;
        }

        public void VisitBold(Bold item)
        {
            verifier[TagTable.Bold] = item.Value;
        }

        public void VisitFontName(FontName item)
        {
            verifier[TagTable.FontName] = !string.IsNullOrEmpty(item.Value);
        }

        public void VisitFontSize(FontSize item)
        {
            verifier[TagTable.FontSize] = item.Value > 0;
        }

        public void VisitItalic(Italic item)
        {
            verifier[TagTable.Italic] = item.Value;
        }

        public void VisitStrikethrough(Strikethrough item)
        {
            verifier[TagTable.Strikethrough] = item.Value;
        }

        public void VisitTextColor(TextColor item)
        {
            verifier[TagTable.TextColor] = item.Value != null;
        }

        public void VisitTextDirection(TextDirection item)
        {
            verifier[TagTable.TextDirection] = !string.IsNullOrEmpty(item.StringValue);
        }

        public void VisitTextPosition(TextPosition item)
        {
            verifier[TagTable.TextPosition] = item.Value != TextPosition.SuperSub.Invalid;
        }

        public void VisitUnderline(Underline item)
        {
            verifier[TagTable.Underline] = item.Value;
        }

        private bool AllSettingsMatch()
        {
            return verifier[TagTable.BackgroundColor] == settings.FormatTagList[TagTable.BackgroundColor] &&
                   verifier[TagTable.Bold] == settings.FormatTagList[TagTable.Bold] &&
                   verifier[TagTable.FontName] == settings.FormatTagList[TagTable.FontName] &&
                   verifier[TagTable.FontSize] == settings.FormatTagList[TagTable.FontSize] &&
                   verifier[TagTable.TextColor] == settings.FormatTagList[TagTable.TextColor] &&
                   verifier[TagTable.Italic] == settings.FormatTagList[TagTable.Italic] &&
                   verifier[TagTable.TextDirection] == settings.FormatTagList[TagTable.TextDirection] &&
                   verifier[TagTable.TextPosition] == settings.FormatTagList[TagTable.TextPosition] &&
                   verifier[TagTable.Underline] == settings.FormatTagList[TagTable.Underline] &&
                   verifier[TagTable.Strikethrough] == settings.FormatTagList[TagTable.Strikethrough];
        }

        private bool ShouldNotRemoveTagExists()
        {
            foreach (var pair in settings.FormatTagList)
            {
                if (!pair.Value)
                {
                    if (verifier[pair.Key])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #region Not Used

        public void VisitUnknownFormatting(UnknownFormatting item)
        {
        }

        #endregion Not Used
    }
}