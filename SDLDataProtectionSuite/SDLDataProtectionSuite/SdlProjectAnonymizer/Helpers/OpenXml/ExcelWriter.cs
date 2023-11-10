using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers.OpenXml.Model;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers
{
	public class ExcelWriter
	{
		private List<ExcelColumn> _excelColumns;
		private ExcelOptions _excelOptions;
		private readonly object _lockObject = new object();

		public void UpdateExcelSheets(string path, List<ExcelSheet> excelSheets, ExcelOptions excelOptions, List<ExcelColumn> excelColumns)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException(nameof(path), "File path cannot be null!");
			}

			_excelOptions = excelOptions;
			_excelColumns = excelColumns;

			WriteExcelSheets(path, excelSheets);
		}

		private void WriteExcelSheets(string path, IReadOnlyCollection<ExcelSheet> excelSheets)
		{
			lock (_lockObject)
			{
				var excelDocument = new Excel();

				using (var spreadsheetDocument = SpreadsheetDocument.Open(path, true))
				{
					foreach (var excelSheet in excelSheets)
					{
						var worksheetPart = GetWorksheetPartByName(spreadsheetDocument, excelSheet.Name);

						foreach (var excelRow in excelSheet.Rows)
						{
							foreach (var cell in excelRow.Cells)
							{
								excelDocument.SetCellValue(spreadsheetDocument, worksheetPart.Worksheet, cell.Column.Index, excelRow.Index, cell.Value, false, false);
							}
						}

						worksheetPart.Worksheet.Save();
					}
				}
			}
		}

		private static WorksheetPart GetWorksheetPartByName(SpreadsheetDocument document, string sheetName)
		{
			var sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == sheetName).ToList();

			if (!sheets.Any())
			{
				// The specified worksheet does not exist.
				return null;
			}

			var relationshipId = sheets.First().Id.Value;
			var worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(relationshipId);
			return worksheetPart;

		}

		public static Sheet GetSheetFromWorkSheet(WorkbookPart workbookPart, WorksheetPart worksheetPart)
		{
			string relationshipId = workbookPart.GetIdOfPart(worksheetPart);
			IEnumerable<Sheet> sheets = workbookPart.Workbook.Sheets.Elements<Sheet>();
			return sheets.FirstOrDefault(s => s.Id.HasValue && s.Id.Value == relationshipId);
		}

		public static Worksheet GetWorkSheetFromSheet(WorkbookPart workbookPart, Sheet sheet)
		{
			var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
			return worksheetPart.Worksheet;
		}

		public static Sheet GetSheetFromName(WorkbookPart workbookPart, string sheetName)
		{
			return workbookPart.Workbook.Sheets.Elements<Sheet>()
				.FirstOrDefault(s => s.Name.HasValue && s.Name.Value == sheetName);
		}
	}
}
