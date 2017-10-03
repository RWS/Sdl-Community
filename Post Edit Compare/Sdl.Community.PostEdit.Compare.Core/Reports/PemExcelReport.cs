using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Compare.DAL.ExcelTableModel;
using Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis;

namespace Sdl.Community.PostEdit.Compare.Core.Reports
{
	public static class PemExcelReport
	{
		public static void CreatePemExcelReport(ExcelPackage xlPackage, ExcelWorksheet worksheet, List<PEMModel> analyseResults)
		{
			//ExcelReportHelper.CreateReportHeader(xlPackage, worksheet);
			ExcelReportHelper.TableTitle(xlPackage, worksheet, "Post-Edit Modifications Analysis", GetTableHeaderValues().Count);

			ExcelReportHelper.CreateTableHeader(xlPackage, worksheet, GetTableHeaderValues());
			ExcelReportHelper.CreateFirstColumnValues(xlPackage, worksheet,GetFirstColumnValues());

			FillTableWithAnalyseResults(xlPackage, worksheet, analyseResults);

		}

		private static void FillTableWithAnalyseResults(ExcelPackage xlPackage, ExcelWorksheet worksheet, List<PEMModel> analyseResults)
		{
			var test = worksheet.Cells;
			var select = test.Value;
			var analysisBandCell = worksheet.Cells.Last(c => c.Value.Equals(Constants.AnalysisBand));
			var rowIndex = analysisBandCell.Start.Row + 1;
			var columnIndex = analysisBandCell.Start.Column + 1;
			var matchValuesTest = new List<string>();
			var matchValue = string.Empty;
			for (var i = rowIndex; i <= GetFirstColumnValues().Count+ analysisBandCell.Start.Row; i++)
			{
				matchValue = worksheet.Cells[i, 1].Text;

				for (var j = columnIndex; j <= GetTableHeaderValues().Count + 1; j++)
				{
					var typeValue = worksheet.Cells[analysisBandCell.Start.Row, j].Text;
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
