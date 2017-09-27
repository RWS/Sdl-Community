using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Sdl.Community.PostEdit.Compare.Core.Helper
{
	public static class ExcelReportHelper
	{
		public static void CreateReportHeader(ExcelPackage xlPackage, ExcelWorksheet worksheet)
		{
			worksheet.Cells["A1"].Value = "Post-Edit Comparison Report" ;
			worksheet.Cells["A1:T1"].Merge = true;		
			worksheet.Cells["A1"].Style.Font.Size = 18;
			worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


			worksheet.Cells["A2"].Value = "Generated " + DateTime.Now;
			worksheet.Cells["A2:T2"].Merge = true;
			worksheet.Cells["A2"].Style.Font.Size = 10;
			worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

			xlPackage.Save();
		}

		public static void TableTitle(ExcelPackage xlPackage, ExcelWorksheet worksheet, string title,int numberOfCellsToMerge)
		{
			var lastUsedRowIndex = worksheet.Dimension.End.Row;
			worksheet.Cells[lastUsedRowIndex + 3, 1].Value = title;
			worksheet.Cells[lastUsedRowIndex + 3, 1].Style.Font.Size = 14;
			worksheet.Cells[lastUsedRowIndex + 3, 1].Style.Font.Bold = true;
			worksheet.Cells[lastUsedRowIndex + 1, numberOfCellsToMerge].Merge = true;
		}
	}
}
