using System;

namespace Sdl.Community.Structures.Comparer
{
    
    [Serializable]
    public class ComparerSettings : ICloneable
    {

        

        public int Id { get; set; }       
        public int CompanyProfileId { get; set; }
        public int ComparisonType { get; set; }
        public bool ConsolidateChanges { get; set; }
        public bool IncludeTagsInComparison { get; set; }
        public DifferencesFormatting StyleNewText { get; set; }
        public DifferencesFormatting StyleRemovedText { get; set; }
        public DifferencesFormatting StyleNewTag { get; set; }
        public DifferencesFormatting StyleRemovedTag { get; set; }



        public ComparerSettings()
        {
            Id = -1;
            CompanyProfileId = -1;
            ComparisonType = 0;//words
            ConsolidateChanges = true;
            IncludeTagsInComparison = true;


            #region  |  new text style  |

            StyleNewText = new DifferencesFormatting();
            StyleNewText.Name = DifferencesFormatting.StyleName.NewText;
            StyleNewText.StyleBold = "Deactivate";
            StyleNewText.StyleItalic = "Deactivate";
            StyleNewText.StyleStrikethrough = "Deactivate";
            StyleNewText.StyleUnderline = "Activate";
            StyleNewText.TextPosition = "Normal";
            StyleNewText.FontSpecifyColor = true;
            StyleNewText.FontColor = "#0000FF";
            StyleNewText.FontSpecifyBackroundColor = true;
            StyleNewText.FontBackroundColor = "#FFFF66";

            #endregion

            #region  |  removed text style  |
            StyleRemovedText = new DifferencesFormatting();
            StyleRemovedText.Name = DifferencesFormatting.StyleName.RemovedText;
            StyleRemovedText.StyleBold = "Deactivate";
            StyleRemovedText.StyleItalic = "Deactivate";
            StyleRemovedText.StyleStrikethrough = "Activate";
            StyleRemovedText.StyleUnderline = "Deactivate";
            StyleRemovedText.TextPosition = "Normal";
            StyleRemovedText.FontSpecifyColor = true;
            StyleRemovedText.FontColor = "#FF0000";
            StyleRemovedText.FontSpecifyBackroundColor = false;
            StyleRemovedText.FontBackroundColor = "#FFFFFF";
            #endregion

            #region  |  new tag style  |
            StyleNewTag = new DifferencesFormatting();            
            StyleNewTag.Name = DifferencesFormatting.StyleName.NewTag;
            StyleNewTag.StyleBold = "Deactivate";
            StyleNewTag.StyleItalic = "Deactivate";
            StyleNewTag.StyleStrikethrough = "Deactivate";
            StyleNewTag.StyleUnderline = "Deactivate";
            StyleNewTag.TextPosition = "Normal";
            StyleNewTag.FontSpecifyColor = false;
            StyleNewTag.FontColor = "#000000";
            StyleNewTag.FontSpecifyBackroundColor = true;
            StyleNewTag.FontBackroundColor = "#DDEEFF";
            #endregion

            #region  |  removed tag style  |

            StyleRemovedTag = new DifferencesFormatting();
            StyleRemovedTag.Name = DifferencesFormatting.StyleName.RemovedTag;
            StyleRemovedTag.StyleBold = "Deactivate";
            StyleRemovedTag.StyleItalic = "Deactivate";
            StyleRemovedTag.StyleStrikethrough = "Deactivate";
            StyleRemovedTag.StyleUnderline = "Deactivate";
            StyleRemovedTag.TextPosition = "Normal";
            StyleRemovedTag.FontSpecifyColor = false;
            StyleRemovedTag.FontColor = "#000000";
            StyleRemovedTag.FontSpecifyBackroundColor = true;
            StyleRemovedTag.FontBackroundColor = "#FFE8E8";

            #endregion
        }

        public object Clone()
        {
            var comparer = new ComparerSettings();

            comparer.Id = Id;
            comparer.CompanyProfileId = CompanyProfileId;
            comparer.ComparisonType = ComparisonType;
            comparer.ConsolidateChanges = ConsolidateChanges;
            comparer.IncludeTagsInComparison = IncludeTagsInComparison;

            comparer.StyleNewText = (DifferencesFormatting)StyleNewText.Clone();
            comparer.StyleRemovedText = (DifferencesFormatting)StyleRemovedText.Clone();
            comparer.StyleNewTag = (DifferencesFormatting)StyleNewTag.Clone();
            comparer.StyleRemovedTag = (DifferencesFormatting)StyleRemovedTag.Clone();

            return comparer;
        }
    }
}
