using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Multilingual.Excel.FileType.Providers.OpenXml.Model;
using Multilingual.Excel.FileType.Services;
using Fill = DocumentFormat.OpenXml.Spreadsheet.Fill;
using Hyperlink = DocumentFormat.OpenXml.Spreadsheet.Hyperlink;

namespace Multilingual.Excel.FileType.Providers.OpenXml
{
	public class ExcelReader
	{
		private List<ExcelColumn> _excelColumns;
		private ExcelOptions _excelOptions;
		private readonly object _lockObject = new object();
		private readonly ColorService _colorService;

		public ExcelReader(ColorService colorService)
		{
			_colorService = colorService;
		}

		public List<ExcelSheet> GetExcelSheets(string path, ExcelOptions excelOptions, List<ExcelColumn> excelColumns)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException(nameof(path), string.Format("The file path cannot be null! {0}", path));
			}

			if (!File.Exists(path))
			{
				throw new ArgumentException(nameof(path), string.Format("Unable to locate the file path! {0}", path));
			}

			_excelOptions = excelOptions;
			_excelColumns = excelColumns;

			var sheets = ReadExcelSheets(path);

			return sheets;
		}

		private List<ExcelSheet> ReadExcelSheets(string path)
		{
			lock (_lockObject)
			{
				var excelSheets = new List<ExcelSheet>();

				using (var spreadsheetDocument = SpreadsheetDocument.Open(path, false))
				{
					var workBookPart = spreadsheetDocument.WorkbookPart;

					foreach (var workSheetPart in workBookPart.WorksheetParts)
					{
						var excelSheet = GetSheetFromWorkSheet(workBookPart, workSheetPart);
						if (excelSheet.State != null && excelSheet.State.HasValue &&
							(excelSheet.State.Value == SheetStateValues.Hidden ||
							 excelSheet.State.Value == SheetStateValues.VeryHidden))
						{
							continue;
						}

						var worksheet = GetExcelWorkSheet(workBookPart, workSheetPart, excelSheet, spreadsheetDocument);
						excelSheets.Add(worksheet);
					}
				}

				if (!_excelOptions.ReadAllWorkSheets)
				{
					return new List<ExcelSheet> { excelSheets.OrderBy(a => a.Index).FirstOrDefault() };
				}

				return excelSheets;
			}
		}

		private ExcelSheet GetExcelWorkSheet(WorkbookPart workBookPart, WorksheetPart workSheetPart, Sheet workSheet,
			SpreadsheetDocument spreadsheetDocument)
		{
			var excelWorkSheet = new ExcelSheet
			{
				Name = workSheet.Name,
				Index = Convert.ToInt32(workSheet.SheetId?.Value)
			};

			var excelRows = new List<ExcelRow>();
			var sheetData = workSheetPart.Worksheet.Elements<SheetData>().First();

			// get all hyperlinks - you might want to store them ...
			var hyperlinks = workSheetPart.RootElement?.Descendants<Hyperlinks>().FirstOrDefault()?.Cast<Hyperlink>().ToList();

			foreach (var row in sheetData.Elements<Row>())
			{
				if (row.Hidden != null && row.Hidden.HasValue && row.Hidden)
				{
					continue;
				}

				var excelRow = ReadExcelRow(row, spreadsheetDocument, workSheetPart, hyperlinks);
				if (excelRow != null)
				{
					excelRows.Add(excelRow);
				}
			}

			excelWorkSheet.Rows = excelRows;
			return excelWorkSheet;
		}

		private ExcelRow ReadExcelRow(OpenXmlElement row, SpreadsheetDocument spreadsheetDocument, WorksheetPart workSheetPart, IReadOnlyCollection<Hyperlink> hyperlinks)
		{
			var xmlRow = row as Row;
			if (xmlRow == null)
			{
				return null;
			}

			if (xmlRow.RowIndex < _excelOptions.FirstRowIndex)
			{
				return null;
			}

			if (xmlRow.RowIndex == _excelOptions.FirstRowIndex && _excelOptions.FirstRowIndexIsHeading)
			{
				AddColumnHeaders(row, spreadsheetDocument);
				return null;
			}

			var excelRow = new ExcelRow
			{
				Index = xmlRow.RowIndex
			};

			foreach (var excelColumn in _excelColumns)
			{
				var columnIndex = GetColumnIndex(excelColumn.Name);
				excelColumn.Index = columnIndex;

				var cell = row.Descendants<Cell>().FirstOrDefault(a => a.CellReference?.Value == excelColumn.Name + excelRow.Index);

				var hyperlink = GetHyperLink(workSheetPart, hyperlinks, cell);

				var cellValue = GetCellValue(cell, spreadsheetDocument);
				var cellBackgroundColor = GetCellBackgroundColor(cell, spreadsheetDocument);

				var excelCell = new ExcelCell
				{
					Column = excelColumn,
					Value = cellValue,
					Hyperlink = hyperlink,
					Background = cellBackgroundColor
				};

				excelRow.Cells.Add(excelCell);
			}

			return excelRow;
		}

		private Models.Hyperlink GetHyperLink(WorksheetPart workSheetPart, IEnumerable<Hyperlink> hyperlinks, Cell cell)
		{
			if (cell == null)
			{
				return null;
			}

			if (!_excelOptions.ReadHyperlinks)
			{
				return null;
			}

			// get the Hyperlink object "behind" the cell
			var hyperlinkInstance = hyperlinks?.SingleOrDefault(i => i.Reference?.Value == cell.CellReference?.Value);
			if (hyperlinkInstance == null)
			{
				return null;
			}

			// the URI is stored in the HyperlinkRelationship
			var hyperlinkRelationship = workSheetPart.HyperlinkRelationships.SingleOrDefault(i => i.Id == hyperlinkInstance.Id);
	

			var hyperlink = new Models.Hyperlink
			{
				Id = hyperlinkInstance.Id,
				Location = hyperlinkInstance.Location,
				Reference = hyperlinkInstance.Reference,
				IsExternal = hyperlinkRelationship?.IsExternal ?? false,
				Display = hyperlinkInstance.Display,
				Tooltip = hyperlinkInstance.Tooltip,
				Url = Uri.UnescapeDataString(hyperlinkRelationship?.Uri.ToString() ?? string.Empty)
			};

			try
			{
				if (hyperlinkRelationship?.Uri != null)
				{
					if (hyperlinkRelationship.Uri.Scheme.StartsWith("mailto",
							StringComparison.CurrentCultureIgnoreCase))
					{
						hyperlink.IsEmail = true;
						hyperlink.Email = Uri.UnescapeDataString(hyperlinkRelationship.Uri.UserInfo + "@" + hyperlinkRelationship.Uri.Host);
						var query = hyperlinkRelationship.Uri.Query.TrimStart('?');
						if (query.StartsWith("subject", StringComparison.CurrentCultureIgnoreCase))
						{
							var indexOfSubject = query.IndexOf("subject=") + "subject=".Length;
							hyperlink.Subject = Uri.UnescapeDataString(query.Substring(indexOfSubject));
						}
					}
				}
			}
			catch
			{
				// ignore; catch all
			}

			return hyperlink;
		}

		private void AddColumnHeaders(OpenXmlElement row, SpreadsheetDocument spreadsheetDocument)
		{
			var xmlRow = row as Row;
			if (xmlRow == null)
			{
				return;
			}

			foreach (var excelColumn in _excelColumns)
			{
				var columnIndex = GetColumnIndex(excelColumn.Name);
				excelColumn.Index = columnIndex;

				var cell = row.Descendants<Cell>()
					.FirstOrDefault(a => a.CellReference?.Value == excelColumn.Name + xmlRow.RowIndex);

				if (cell != null)
				{
					var cellValue = GetCellValue(cell, spreadsheetDocument);
					excelColumn.Text = cellValue?.Trim();
				}
			}
		}

		private string GetCellBackgroundColor(CellType cell, SpreadsheetDocument spreadsheetDocument)
		{
			var styles = spreadsheetDocument.WorkbookPart?.WorkbookStylesPart;

			var cellStyleIndex = GetCellStyleIndex(cell);
			var cellFormat = (CellFormat)styles?.Stylesheet.CellFormats?.ChildElements[cellStyleIndex];
			if (cellFormat?.FillId == null)
			{
				return null;
			}

			var fill = (Fill)styles.Stylesheet.Fills?.ChildElements[(int)cellFormat.FillId.Value];
			var ct = fill?.PatternFill?.ForegroundColor;
			if (ct == null)
			{
				return null;
			}

			if (ct.Auto != null)
			{
				Console.Out.WriteLine("System auto color");
			}

			if (ct.Rgb != null)
			{
				Console.Out.WriteLine("RGB value -> {0}", ct.Rgb.Value);
				return ct.Rgb.Value;
			}

			var themeValue = ct.Theme;
			if (themeValue != null)
			{
				//var ee = spreadsheetDocument.WorkbookPart?.ThemePart?.Theme.ThemeElements?.ColorScheme?.ChildElements;
				//var c2t = (Color2Type)spreadsheetDocument.WorkbookPart?.ThemePart?.Theme.ThemeElements?.ColorScheme?.ChildElements[(int)themeValue.Value-1];
				//Console.Out.WriteLine("RGB color model hex -> {0}", c2t.RgbColorModelHex.Val);

				var colorValue = _colorService.GetColor(ct, Convert.ToInt32(themeValue.Value));
				return colorValue;
			}

			if (ct.Indexed != null)
			{
				Console.Out.WriteLine("Indexed color -> {0}", ct.Indexed.Value);

				var ic = styles.Stylesheet.Colors?.IndexedColors?.ChildElements[(int)ct.Indexed.Value];
				switch (ic)
				{
					case RgbColor rgbColor:
						return rgbColor.Rgb?.Value;
					case IndexedColors indexedColors:
						return indexedColors.ToString();
					default:
						return null;
				}
			}

			return null;
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

		private static string GetCellValue(CellType cell, SpreadsheetDocument spreadsheetDocument)
		{
			if (cell == null)
			{
				return null;
			}

			var value = cell.CellValue?.Text;
			if (cell.DataType?.Value == CellValues.SharedString)
			{
				return spreadsheetDocument.WorkbookPart.SharedStringTablePart.
					SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
			}

			return value;
		}

		private uint GetColumnIndex(string cellReference)
		{
			if (string.IsNullOrEmpty(cellReference))
			{
				throw new NullReferenceException();
			}

			//remove digits
			var columnReference = Regex.Replace(cellReference.ToUpper(), @"[\d]", string.Empty);

			var columnNumber = -1;
			var mulitplier = 1;

			//working from the end of the letters take the ASCII code less 64 (so A = 1, B =2...etc)
			//then multiply that number by our multiplier (which starts at 1)
			//multiply our multiplier by 26 as there are 26 letters
			foreach (var c in columnReference.ToCharArray().Reverse())
			{
				columnNumber += mulitplier * ((int)c - 64);

				mulitplier = mulitplier * 26;
			}

			//the result is zero based so return columnnumber + 1 for a 1 based answer
			//this will match Excel's COLUMN function
			return Convert.ToUInt32(columnNumber + 1);
		}

		private string GetColumnName(StringValue cellName)
		{
			var regex = new Regex("[a-zA-Z]+");
			var match = regex.Match(cellName);
			return match.Value;
		}

		private uint GetRowIndex(StringValue cellName)
		{
			var regex = new Regex(@"\d+");
			var match = regex.Match(cellName);
			return uint.Parse(match.Value);
		}

		public static Sheet GetSheetFromWorkSheet(WorkbookPart workbookPart, WorksheetPart worksheetPart)
		{
			var relationshipId = workbookPart.GetIdOfPart(worksheetPart);
			var sheets = workbookPart.Workbook.Sheets.Elements<Sheet>();

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
