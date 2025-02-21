using System.Collections.Generic;

namespace Multilingual.XML.FileType.Providers.Excel.Model
{
	public class ExcelRow
	{
		public ExcelRow()
		{
			Cells = new List<ExcelCell>();
		}

		public int Index { get; set; }


		public List<ExcelCell> Cells;
	}
}
