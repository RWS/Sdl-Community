using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Compare.DAL.ExcelTableModel;
using Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis;

namespace Sdl.Community.PostEdit.Compare.Core.Reports
{
	public static class FileInformationExcelReport
	{
		public static void CreateFileTable(ExcelPackage xlPackage, ExcelWorksheet worksheet, List<FilesInformationModel> filesInfo)
		{
			ExcelReportHelper.CreateReportHeader(xlPackage, worksheet);
			ExcelReportHelper.TableTitle(xlPackage, worksheet, "Files information", GetTableHeaderValues().Count);
			ExcelReportHelper.CreateTableHeader(xlPackage, worksheet, GetTableHeaderValues());
			ExcelReportHelper.CreateFirstColumnValues(xlPackage, worksheet, GetFirstColumnValues());

			FillTableWithAnalyseResults(xlPackage, worksheet, filesInfo);
		}

		private static void FillTableWithAnalyseResults(ExcelPackage xlPackage, ExcelWorksheet worksheet, List<FilesInformationModel> filesInfo)
		{
			var versionsCellQueryResult =
								(from cell in worksheet.Cells
								 where cell.Value?.ToString() == Constants.Versions
								 select cell).Last();

			if (versionsCellQueryResult != null)
			{
				var rangeCell = versionsCellQueryResult.ToList()[0];
				var rowIndex = rangeCell.Start.Row + 1;
				var columnIndex = rangeCell.Start.Column + 1;
				var matchValuesTest = new List<string>();
				var matchValue = string.Empty;
				for (var i = rowIndex; i <= GetFirstColumnValues().Count + rangeCell.Start.Row; i++)
				{
					matchValue = worksheet.Cells[i, 1].Text;

					for (var j = columnIndex; j <= GetTableHeaderValues().Count ; j++)
					{
						var typeValue = worksheet.Cells[rangeCell.Start.Row, j].Text;
						var value = GetMatchingResultValue(matchValue, typeValue, filesInfo);

						worksheet.Cells[i, j].Value = value;
						
					}
				}

				xlPackage.Save();
				
			}
		}
		private static string GetMatchingResultValue(string matchValue, string typeValue, List<FilesInformationModel> filesInfo)
		{
			var info = filesInfo.FirstOrDefault(m => (m.FilesInfo.Item1.Equals(matchValue))
			&& (m.FilesInfo.Item2.Equals(typeValue)));

			if (info != null)
			{
				return info.FilesInfo.Item3;
			}
			return null;
		}

		private static List<string> GetFirstColumnValues()
		{
			var columnValues = new List<string>
			{
				Constants.Original,
				Constants.Updated
		
			};
			return columnValues;
		}
		private static List<string> GetTableHeaderValues()
		{
			var headerValues = new List<string>
			{
					Constants.Versions,
					Constants.Language,
					Constants.FilePath

			};
			return headerValues;
		}
	}
}
