using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Multilingual.Excel.FileType.Providers.OpenXml.Model;
using Hyperlink = DocumentFormat.OpenXml.Spreadsheet.Hyperlink;

namespace Multilingual.Excel.FileType.Providers.OpenXml
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

		private void WriteExcelSheets(string path, IEnumerable<ExcelSheet> excelSheets)
		{
			lock (_lockObject)
			{
				var excelDocument = new Excel();

				using (var spreadsheetDocument = SpreadsheetDocument.Open(path, true))
				{
					foreach (var excelSheet in excelSheets)
					{
						var workSheetPart = GetWorksheetPartByName(spreadsheetDocument, excelSheet.Name);

						var hyperlinks = workSheetPart.Worksheet.Descendants<Hyperlinks>().FirstOrDefault();
						if (hyperlinks == null)
						{
							hyperlinks = new Hyperlinks();
							var pm = workSheetPart.Worksheet.Descendants<PageMargins>().First();
							workSheetPart.Worksheet.InsertBefore(hyperlinks, pm);
						}

						var hyperlinkList = hyperlinks.Cast<Hyperlink>().ToList();

						foreach (var excelRow in excelSheet.Rows)
						{
							var styleIndex = 0;
							foreach (var excelCell in excelRow.Cells)
							{
								excelDocument.SetCellValue(spreadsheetDocument, workSheetPart.Worksheet,
									excelCell.Column.Index, excelRow.Index, excelCell.Value, false, false);

								var isHyperlink = !string.IsNullOrEmpty(excelCell.Hyperlink?.Reference);
								if (isHyperlink)
								{
									var row = excelDocument.GetRow(excelRow, workSheetPart);
									var cell = excelDocument.GetCell(excelCell.Column.Index, excelRow.Index, row);
									var cellStyleIndex = GetCellStyleIndex(cell);
									if (styleIndex <= 0 && cell.StyleIndex != null && cell.StyleIndex > 0)
									{
										styleIndex = cellStyleIndex;
									}

									var hyperlink = hyperlinkList.SingleOrDefault(i => i.Reference?.Value == cell.CellReference?.Value);
									excelDocument.SetHyperlink(hyperlinks, hyperlink, workSheetPart, excelCell.Hyperlink);
									
									cell.StyleIndex = Convert.ToUInt32(styleIndex);
								}
							}
						}

						workSheetPart.Worksheet.Save();
					}
				}
			}
		}

		private static int GetCellStyleIndex(CellType cell)
		{
			int cellStyleIndex;
			if (cell?.StyleIndex == null)
			{
				cellStyleIndex = 0;
			}
			else
			{
				cellStyleIndex = (int)cell.StyleIndex.Value;
			}

			return cellStyleIndex;
		}

		public string ColumnNameFromIndex(uint columnIndex)
		{
			var columnName = "";

			while (columnIndex > 0)
			{
				var remainder = (columnIndex - 1) % 26;
				columnName = Convert.ToChar(65 + remainder) + columnName;
				columnIndex = (columnIndex - remainder) / 26;
			}

			return columnName;
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
