using System.Collections.Generic;

namespace Multilingual.Excel.FileType.Models
{
	public class MultilingualParagraphUnit
	{
		public MultilingualParagraphUnit()
		{
			ParagraphUnitInfos = new List<ParagraphUnitInfo>();
		}

		public int ExcelSheetIndex { get; set; }

		public string ExcelSheetName { get; set; }

		public uint ExcelRowIndex { get; set; }

		public List<ParagraphUnitInfo> ParagraphUnitInfos { get; set; }
	}
}
