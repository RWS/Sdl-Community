using System;
using System.Collections.Generic;
using System.Drawing;
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
	public static class TerpExcelReport
	{
		public static void CreateTerpExcelReport(ExcelPackage xlPackage, ExcelWorksheet worksheet, List<TERpModel> terpAnalyseResults)
		{

			ExcelReportHelper.TableTitle(xlPackage, worksheet, "TERp Analysis", 6);
			ExcelReportHelper.CreateTableHeader(xlPackage, worksheet, GetTableHeaderValues());
			ExcelReportHelper.CreateFirstColumnValues(xlPackage, worksheet, GetFirstColumnValues());
			FillTableWithAnalyseResults(xlPackage, worksheet, terpAnalyseResults);
			xlPackage.Save();

		}


		private static void FillTableWithAnalyseResults(ExcelPackage xlPackage, ExcelWorksheet worksheet, List<TERpModel> analyseResults)
		{
			var rangeCellQueryResult =
								from cell in worksheet.Cells
									where cell.Value?.ToString() == Constants.Range
								select cell;

			if (rangeCellQueryResult != null)
			{
				var rangeCell = rangeCellQueryResult.ToList()[0];
				var rowIndex = rangeCell.Start.Row + 1;
				var columnIndex = rangeCell.Start.Column + 1;
				var matchValuesTest = new List<string>();
				var matchValue = string.Empty;
				for (var i = rowIndex; i <= GetFirstColumnValues().Count + rangeCell.Start.Row; i++)
				{
					matchValue = worksheet.Cells[i, 1].Text;

					for (var j = columnIndex; j <= GetTableHeaderValues().Count + 1; j++)
					{
						var typeValue = worksheet.Cells[rangeCell.Start.Row, j].Text;
						var value = GetMatchingResultValue(matchValue, typeValue, analyseResults);

						worksheet.Cells[i, j].Value = value;
					}
				}

				xlPackage.Save();
			}
			
		}

		private static decimal? GetMatchingResultValue(string matchValue, string typeValue, List<TERpModel> analyseResults)
		{
			var analyseResult = analyseResults.FirstOrDefault(m => (m.TERpAnalyseResult.Item1.Equals(matchValue))
			&& (m.TERpAnalyseResult.Item2.Equals(typeValue)));

			if (analyseResult != null)
			{
				return analyseResult.TERpAnalyseResult.Item3;
			}
			return null;
		}

		private static List<string> GetTableHeaderValues()
		{
			var headerValues = new List<string>
			{
					Constants.Range,
					Constants.Segments,
					Constants.Words,
					Constants.RefWords,
					Constants.Errors,
					Constants.Ins,
					Constants.Del,
					Constants.Sub,
					Constants.Shft
			};
			return headerValues;
		}
		private static List<string> GetFirstColumnValues()
		{
			var columnValues = new List<string>
			{
				Constants.Terp00,
				Constants.Terp01,
				Constants.Terp06,
				Constants.Terp10,
				Constants.Terp20,
				Constants.Terp30,
				Constants.Terp40,
				Constants.Terp50,
				Constants.Total

			};
			return columnValues;
		}
	}
}
