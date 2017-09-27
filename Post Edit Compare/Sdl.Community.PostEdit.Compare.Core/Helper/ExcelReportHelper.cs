using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Sdl.Community.PostEdit.Compare.Core.Helper
{
	public static class ExcelReportHelper
	{
		public static void CreateReportHeader(ExcelPackage xlPackage, ExcelWorksheet worksheet)
		{
			//worksheet.Cells["A1:T1"].Value = "Post-Edit Comparison Report";
			worksheet.Cells["A1"].Value = "tezt";
			xlPackage.Save();
		}
	}
}
