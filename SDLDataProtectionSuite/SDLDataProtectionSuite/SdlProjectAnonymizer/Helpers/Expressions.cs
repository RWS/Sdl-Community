using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers.OpenXml;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers
{
	public static class Expressions
	{
		public static void ExportExpressions(string filePath, List<RegexPattern> patterns)
		{
			var package = Excel.GetSheetInfo(filePath, true);
			var excelDocument = new Excel();
			var worksheet = excelDocument.AddWorksheet(package, "Exported expressions");
			var order = 0;

			var rowIndex = Convert.ToUInt32(1);

			excelDocument.SetCellValue(package, worksheet, 1, rowIndex, "ID");
			excelDocument.SetCellValue(package, worksheet, 2, rowIndex, "Order");
			excelDocument.SetCellValue(package, worksheet, 3, rowIndex, "Rule");
			excelDocument.SetCellValue(package, worksheet, 4, rowIndex, "Description");

			for (var i = patterns.Count - 1; i >= 0; i--)
			{
				if (patterns[i] == null)
					continue;

				rowIndex++;
				excelDocument.SetCellValue(package, worksheet, 1, rowIndex, patterns[i].Id);
				var orderNumber = order++;
				excelDocument.SetCellValue(package, worksheet, 2, rowIndex, orderNumber.ToString());
				excelDocument.SetCellValue(package, worksheet, 3, rowIndex, patterns[i].Pattern);
				excelDocument.SetCellValue(package, worksheet, 4, rowIndex, patterns[i].Description);
			}
			excelDocument.SetColumnWidth(worksheet, 1, 10);
			excelDocument.SetColumnWidth(worksheet, 2, 10);
			excelDocument.SetColumnWidth(worksheet, 3, 50);
			excelDocument.SetColumnWidth(worksheet, 4, 35);

			worksheet.Save();
			package.Dispose();
		}

		public static List<RegexPattern> GetImportedExpressions(List<string> files)
		{
			var patterns = new List<RegexPattern>();
			foreach (var file in files)
			{
				//Open the Excel file in Read Mode using OpenXml.
				using var doc = SpreadsheetDocument.Open(file, false);
				//Read the first Sheet from Excel file.
				var sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

				//Get the Worksheet instance.
				var worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

				//Fetch all the rows present in the Worksheet.
				var rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

				//Loop through the Worksheet rows.
				foreach (var row in rows)
				{
					var pattern = new RegexPattern
					{
						ShouldEnable = true,
						Description = string.Empty,
						Pattern = string.Empty
					};

					var colValue = row.ToList();
					if (row.RowIndex.Value == 1)
					{
						if (!IsValidColumnHeader(colValue[0].InnerText, "ID") |
							!IsValidColumnHeader(colValue[1].InnerText, "Order") |
							!IsValidColumnHeader(colValue[2].InnerText, "Rule") |
							!IsValidColumnHeader(colValue[3].InnerText, "Description")
						   )
						{
							return null;
						}
					}

					//Use the first row to add columns to DataTable.
					if (row.RowIndex.Value > 1)
					{
						for (var i = 0; i < colValue.Count; i++)
						{
							var address = ((DocumentFormat.OpenXml.Spreadsheet.CellType)colValue[i]).CellReference.Value;
							var cellValue = colValue[i].InnerText;
							if (address.Contains("A"))
							{
								pattern.Id = cellValue ?? string.Empty;
							}
							else if (address.Contains("C"))
							{
								pattern.Pattern = cellValue ?? string.Empty;
							}
							else if (address.Contains("D"))
							{
								pattern.Description = cellValue ?? string.Empty;
							}
						}
						patterns.Add(pattern);
					}
				}
			}
			return patterns;
		}

		private static bool IsValidColumnHeader(string colmun, string expected)
		{
			if (string.Compare(colmun, expected, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				MessageBox.Show("Invalid import format, found '" + colmun + "', expected column header name '" + expected + "'", "TradosTM Anonymizer");

				return false;
			}

			return true;
		}
	}
}