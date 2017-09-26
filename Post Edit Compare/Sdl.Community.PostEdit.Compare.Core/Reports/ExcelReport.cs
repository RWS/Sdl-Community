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
		public static void CreateExcelReport(List<PEMModel> analyseResults)
		{

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
				//var tableHeader = GetTableHeaderValues();
				//var headerCellIndex = 1;
				//for(var i = 0; i < tableHeader.Count; i++)
				//{
				//	worksheet.Cells[1,headerCellIndex].Value = tableHeader[i];
				//	headerCellIndex++;
				//}

				//xlPackage.Save();
				//var countRow = worksheet.Dimension.End.Row;
			

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
			//var columnIndex = analysisBandCell.Columns;
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
				worksheet.Cells[rowIndex, 1].Value = column.Item3;
				rowIndex++;
			}
			
			xlPackage.Save();
		}

		private static List<Tuple<int,string,string>> GetFirstColumnValues()
		{
			var columnValues = new List<Tuple<int, string, string>>{
				Tuple.Create(0,"fuzzy100","100%"),
				Tuple.Create(1,"fuzzy99","95% - 99%"),
				Tuple.Create(2,"fuzzy94","85% - 94%"),
				Tuple.Create(3,"fuzzy84","75% - 84%"),
				Tuple.Create(4,"fuzzy74","50% - 74%"),
				Tuple.Create(5,"new","New"),
				Tuple.Create(6,"total","Total"),
			};
			return columnValues;
		}
		private static void CreateTableHeader(ExcelPackage xlPackage, ExcelWorksheet worksheet)
		{
			var tableHeader = GetTableHeaderValues();
			int rowNr;//= worksheet.Dimension.End.Row; // ultima linie folosita in excel
			int columnNr;//= worksheet.Dimension.End.Column;//ultima coloana
			if (worksheet.Dimension == null)
			{
				rowNr = 1;
				columnNr = 1;
			}
			else
			{
				rowNr = worksheet.Dimension.End.Row; // ultima linie folosita in excel
				columnNr = worksheet.Dimension.End.Column;//ultima coloana
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
			//var headerValues = new List<PostEditModificationsExcelTableModel>
			//{
			//	new PostEditModificationsExcelTableModel{
			//		HeaderValues = Tuple.Create(0,"analysisBand","Analysis Band")
			//	},
			//	new PostEditModificationsExcelTableModel{
			//		HeaderValues = Tuple.Create(1,"segments","Segments")
			//	},
			//	new PostEditModificationsExcelTableModel{
			//		HeaderValues = Tuple.Create(2,"words","Words")
			//	},
			//	new PostEditModificationsExcelTableModel{
			//		HeaderValues = Tuple.Create(3,"characters","Characters")
			//	},
			//	new PostEditModificationsExcelTableModel{
			//		HeaderValues = Tuple.Create(4,"percent","Percent")
			//	},
			//	new PostEditModificationsExcelTableModel{
			//		HeaderValues = Tuple.Create(5,"total","Total")
			//	},
			//};
			var headerValues = new List<string>
			{				
					Constants.AnalysisBand,
					Constants.Segments,
					Constants.Words,
					Constants.Characters,
					Constants.Percent,
					Constants.Total		};
			return headerValues;
		}
			
		//private static List<string> GetTableHeaderValues()
		//{
		//	//var headerValues = new PostEditModificationsExcelTableModel
		//	//{
		//	//	AnalysisBand = "Analysis Band",
		//	//	Segments = "Segments",
		//	//	Words = "Words",
		//	//	Characters = "Characters",
		//	//	Percent = "Percent",

		//	//};
		//	//return headerValues;
		//	return new List<string>
		//	{
		//		"Analysis Band",
		//		"Segments",
		//		"Words",
		//		"Characters",
		//		"Percent"
		//	};
		//} 
	}
}
