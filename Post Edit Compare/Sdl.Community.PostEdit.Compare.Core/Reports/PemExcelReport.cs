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
	public static class PemExcelReport
	{
		public static void CreatePemExcelReport(List<PEMModel> analyseResults)
		{
			//save report to specific folder is not implemented yet
			var reportPath = Path.Combine(@"C:\Users\aghisa\Desktop", "report.xlsx");
			if (File.Exists(reportPath)) { File.Delete(reportPath); };
			var newFile = new FileInfo(Path.Combine(@"C:\Users\aghisa\Desktop", "report.xlsx"));

			using (ExcelPackage xlPackage = new ExcelPackage(newFile))
			{
				// get handle to the existing worksheet
				var worksheet = xlPackage.Workbook.Worksheets.Add("Test");
				
				CreateTableHeader(xlPackage, worksheet);
				CreateFirstColumnValues(xlPackage, worksheet);
				FillTableWithAnalyseResults(xlPackage, worksheet, analyseResults);

			}

		}

		private static void FillTableWithAnalyseResults(ExcelPackage xlPackage, ExcelWorksheet worksheet, List<PEMModel> analyseResults)
		{
			var analysisBandCell = worksheet.Cells.FirstOrDefault(c => c.Value.Equals(Constants.AnalysisBand));
			var rowIndex = analysisBandCell.Rows + 1;
			var columnIndex = analysisBandCell.Columns + 1;
			var matchValuesTest = new List<string>();
			var matchValue = string.Empty;
			for (var i = rowIndex; i <= GetFirstColumnValues().Count+1; i++)
			{
				matchValue = worksheet.Cells[i, 1].Text;

				for (var j = columnIndex; j <= GetTableHeaderValues().Count+1; j++)
				{
					var typeValue = worksheet.Cells[1, j].Text;
					var value = GetMatchingResultValue(matchValue, typeValue, analyseResults);
					worksheet.Cells[i, j].Value = value;
				}
			}

			xlPackage.Save();
		}

		private static decimal? GetMatchingResultValue(string matchValue,string typeValue, List<PEMModel> analyseResults)
		{
			var analyseResult = analyseResults.FirstOrDefault(m => (m.AnalyseResult.Item1.Equals(matchValue))
			&& (m.AnalyseResult.Item2.Equals(typeValue)));

			if (analyseResult != null)
			{
				return analyseResult.AnalyseResult.Item3;
			}
			return null;
		}


		private static void CreateFirstColumnValues(ExcelPackage xlPackage, ExcelWorksheet worksheet)
		{
			var columnValues = GetFirstColumnValues();
			var rowIndex= worksheet.Dimension.End.Row+1;
			
			foreach(var column in columnValues)
			{
				worksheet.Cells[rowIndex, 1].Value = column;
				rowIndex++;
			}
			
			xlPackage.Save();
		}

		private static List<string> GetFirstColumnValues()
		{
			var columnValues = new List<string>
			{
				Constants.ExactMatch,
				Constants.Fuzzy99,
				Constants.Fuzzy94,
				Constants.Fuzzy84,
				Constants.Fuzzy74,
				Constants.New,
				Constants.Total
			};
			return columnValues;
		}
		private static void CreateTableHeader(ExcelPackage xlPackage, ExcelWorksheet worksheet)
		{
			var tableHeader = GetTableHeaderValues();
			int rowNr;//= worksheet.Dimension.End.Row; 
			int columnNr;//= worksheet.Dimension.End.Column;
			if (worksheet.Dimension == null)
			{
				rowNr = 1;
				columnNr = 1;
			}
			else
			{
				rowNr = worksheet.Dimension.End.Row; 
				columnNr = worksheet.Dimension.End.Column;
			}
			foreach (var item in tableHeader)
			{
				worksheet.Cells[rowNr, columnNr].Value = item;
				columnNr++;

			}
			xlPackage.Save();
		}

		private static List<string> GetTableHeaderValues()
		{
			var headerValues = new List<string>
			{
					Constants.AnalysisBand,
					Constants.Segments,
					Constants.Words,
					Constants.Characters,
					Constants.Percent,
					Constants.Total
			};
			return headerValues;
		}
			
	}
}
