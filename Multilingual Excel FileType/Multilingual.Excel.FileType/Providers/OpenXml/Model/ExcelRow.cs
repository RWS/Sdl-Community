using System.Collections.Generic;

namespace Multilingual.Excel.FileType.Providers.OpenXml.Model
{
	public class ExcelRow
	{
		public ExcelRow()
		{
			Cells = new List<ExcelCell>();
		}

		public uint Index { get; set; }


		public List<ExcelCell> Cells;
	}
}
