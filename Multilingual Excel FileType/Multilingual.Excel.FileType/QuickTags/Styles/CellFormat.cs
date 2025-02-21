namespace Multilingual.Excel.FileType.QuickTags.Styles
{
    internal class CellFormat
    {
        public Font Font { get; set; }
        public bool ApplyFont { get; set; }
        public Alignment Alignment { get; set; }
        public bool ApplyAlignment { get; set; }
        public CellFormat()
        {
            Font = Font.Default;
            Alignment = Alignment.Default;

            ApplyFont = true;
            ApplyAlignment = true;
        }
    }
}