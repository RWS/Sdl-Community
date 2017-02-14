using System;

namespace Sdl.Community.Structures.Comparer
{
    
    [Serializable]
    public class DifferencesFormatting: ICloneable
    {
        public enum StyleName
        {
            NewText,
            RemovedText,
            NewTag,
            RemovedTag,
            None
        }
        public int Id { get;set;}      
        public int CompanyProfileId { get; set; }
        public StyleName Name { get; set; }
        public string StyleBold { get; set; }
        public string StyleItalic { get; set; }
        public string StyleStrikethrough { get; set; }
        public string StyleUnderline { get; set; }
        public string TextPosition { get; set; }
        public bool FontSpecifyColor { get; set; }
        public string FontColor { get; set; }
        public bool FontSpecifyBackroundColor { get; set; }
        public string FontBackroundColor { get; set; }

        public DifferencesFormatting()
        {
            Id = -1;   
            CompanyProfileId = -1;
            Name = StyleName.None;
            StyleBold = "Deactivate";
            StyleItalic = "Deactivate";
            StyleStrikethrough = "Deactivate";
            StyleUnderline = "Deactivate";
            TextPosition = "Normal";
            FontSpecifyColor = false;
            FontColor = "000000";
            FontSpecifyBackroundColor = false;
            FontBackroundColor = "FFFFFF";
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
