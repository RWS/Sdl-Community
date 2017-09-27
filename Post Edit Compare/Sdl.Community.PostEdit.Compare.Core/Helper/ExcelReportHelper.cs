using System;
using System.Collections.Generic;
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
			//worksheet.Cells["A1:A2"].Merge = true;
		//	worksheet.Cells["A2:T2"].Merge = true;
			xlPackage.Save();
		}
	}
}
