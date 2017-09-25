using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Sdl.Community.PostEdit.Compare.DAL.ExcelTableModel;
using Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis;

namespace Sdl.Community.PostEdit.Compare.Core.Reports
{
	public static class ExcelReport
	{
		public static void CreateExcelReport(PEMModel analysisModel)
		{

			var reportPath = Path.Combine(@"C:\Users\aghisa\Desktop", "report.xlsx");
			if (File.Exists(reportPath)) { File.Delete(reportPath); };
			var newFile = new FileInfo(Path.Combine(@"C:\Users\aghisa\Desktop", "report.xlsx"));

			using (ExcelPackage xlPackage = new ExcelPackage(newFile))
			{
				// get handle to the existing worksheet
				var worksheet = xlPackage.Workbook.Worksheets.Add("Test");
				
				var tableHeader = GetTableHeaderValues();
				var headerCellIndex = 1;
				for(var i = 0; i < tableHeader.Count; i++)
				{
					worksheet.Cells[1,headerCellIndex].Value = tableHeader[i];
					headerCellIndex++;
				}

				xlPackage.Save();
				var countRow = worksheet.Dimension.End.Row;
			
			}
			
		}

		private static List<string> GetTableHeaderValues()
		{
			//var headerValues = new PostEditModificationsExcelTableModel
			//{
			//	AnalysisBand = "Analysis Band",
			//	Segments = "Segments",
			//	Words = "Words",
			//	Characters = "Characters",
			//	Percent = "Percent",

			//};
			//return headerValues;
			return new List<string>
			{
				"Analysis Band",
				"Segments",
				"Words",
				"Characters",
				"Percent"
			};
		} 
	}
}
