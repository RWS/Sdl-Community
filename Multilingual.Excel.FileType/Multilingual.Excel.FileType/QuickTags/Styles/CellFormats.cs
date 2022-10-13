using System.Collections.Generic;

namespace Multilingual.Excel.FileType.QuickTags.Styles
{
    internal class CellFormats
    {
        private readonly IList<CellFormat> _cellFormats;

        public CellFormats()
        {
            _cellFormats = new List<CellFormat>();
        }

        public void AddCellFormat(CellFormat cellFormat)
        {
            _cellFormats.Add(cellFormat);
        }

        public CellFormat GetCellFormat(int index)
        {
            if (index >= _cellFormats.Count && _cellFormats.Count == 0)
                return new CellFormat();

            return _cellFormats[index];
        }
    }
}