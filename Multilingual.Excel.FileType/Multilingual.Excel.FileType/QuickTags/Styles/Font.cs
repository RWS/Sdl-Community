using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using Multilingual.Excel.FileType.Constants;

namespace Multilingual.Excel.FileType.QuickTags.Styles
{
    internal class Font
    {
        public static Font Default =>
            new Font
            {
                Size = 11,
                Color = new FontColor {Rgb = "FF000000"},
                Name = "Calibri",
                Family = 2,
                Scheme = FontScheme.minor
            };

        public bool? Bold { get; set; }
        public bool? Italic { get; set; }
        public UnderlineStyle? Underline { get; set; }
        public FontColor Color { get; set; }
        public string Name { get; set; }
        public int? Family { get; set; }
        public FontScheme? Scheme { get; set; }
        public int? CharSet { get; set; }
        public double? Size { get; set; }
        public bool? StrikeThrough { get; set; }
        public VerticalTextAlignment? VerticalTextAlignment { get; set; }

        public bool IsEmpty =>
            !(Bold.HasValue || Italic.HasValue || Underline.HasValue ||
              Color != null || Name != null || Family.HasValue
              || Scheme.HasValue|| CharSet.HasValue || Size.HasValue ||
              StrikeThrough.HasValue || VerticalTextAlignment.HasValue);

        public bool IsEqual(Font other)
        {
            return Bold == other.Bold &&
                   Italic == other.Italic &&
                   Underline == other.Underline &&
                   ColorIsEqual(other.Color) &&
                   Name == other.Name &&
                   Family == other.Family &&
                   Scheme == other.Scheme &&
                   CharSet == other.CharSet &&
                   Equals(Size, other.Size) &&
                   StrikeThrough == other.StrikeThrough &&
                   VerticalTextAlignment == other.VerticalTextAlignment;
        }

        private bool ColorIsEqual(FontColor otherColor)
        {
            if (Color != null)
                return Color.Rgb == otherColor.Rgb &&
                       Color.Theme == otherColor.Theme &&
                       Color.Auto == otherColor.Auto &&
                       Color.Indexed == otherColor.Indexed &&
                       Equals(Color.Tint, otherColor.Tint);
            return false;
        }

        //public RunProperties ToRunProperties()
        //{
        //    var runProperties = new RunProperties();

        //    if (Bold.HasValue) runProperties.Bold = Bold;
        //    if (Italic.HasValue) runProperties.Italic = Italic;
        //    if (Underline.HasValue) runProperties.Underline = Underline;
        //    if (Color != null) runProperties.Color = Color;
        //    if (Name != null) runProperties.FontName = Name;
        //    if (Family.HasValue) runProperties.FontFamily = Family;
        //    if (Scheme.HasValue) runProperties.FontScheme = Scheme;
        //    if (CharSet.HasValue) runProperties.FontCharSet = CharSet;
        //    if (Size.HasValue) runProperties.Size = Size;
        //    if (StrikeThrough.HasValue) runProperties.StrikeThrough = StrikeThrough;
        //    if (VerticalTextAlignment.HasValue) runProperties.VerticalTextAlignment = VerticalTextAlignment;

        //    return runProperties;
        //}

        //public static Font FromCell(CellType cell, StylesContext stylesContext, FontColorMapper fontColorMapper)
        //{
        //    if (cell == null
        //        || !cell.HasAttributes
        //        || cell.GetAttributes().All(attribute => attribute.LocalName != StylesConstants.S))
        //    {
        //        return new Font();
        //    }

        //    var referenceForStyleCell = cell.GetAttribute(StylesConstants.S, string.Empty);
        //    var font = stylesContext.GetFontFromCellFormat(int.Parse(referenceForStyleCell.Value));

        //    fontColorMapper.ConvertFontColorToRgb(font.Color);

        //    return font;
        //}
    }
}