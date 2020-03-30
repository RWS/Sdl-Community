using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.MTCloud.Languages.Provider.Model;

namespace Sdl.Community.MTCloud.Languages.Provider.Implementation
{
	internal class Reader
	{
		private readonly Common _common;

		internal Reader()
		{
			_common = new Common();
		}

		/// <summary>
		/// Reads the cell values from the rows of the first sheet in the
		/// excel document <param name="path">file path</param>
		/// , considering the <param name="columns">columns</param> defined.
		/// </summary>
		/// <param name="path">The full path to the excel file</param>
		/// <param name="columns">
		///	The excel columns used to define the structure of the excel sheet.
		///	If null, then the default columns are used to identify the MT Clound languages.
		/// </param>
		internal List<ExcelRow> ReadLanguages(string path, List<ExcelColumn> columns = null)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("File path cannot be null!");
			}

			if (!File.Exists(path))
			{
				// create the default languages file
				_common.SaveDefaultLanguagesDocument(path);
			}

			if (columns == null)
			{
				columns = _common.GetDefaultExcelColumns();
			}

			// reads the rows from the first sheet
			var excelRows = GetExcelRows(path, columns);

			return excelRows;
		}

		private List<ExcelRow> GetExcelRows(string path, IReadOnlyList<ExcelColumn> columns)
		{
			var excelRows = new List<ExcelRow>();

			using (var doc = SpreadsheetDocument.Open(path, false))
			{
				var workbook = doc.WorkbookPart;
				var worksheet = workbook.WorksheetParts.First();
				var sheetData = worksheet.Worksheet.Elements<SheetData>().First();

				var rowIndex = 0;
				foreach (var row in sheetData.Elements<Row>())
				{
					rowIndex++;

					// ignore the header values
					if (rowIndex > 1)
					{
						var excelRow = ReadExcelRow(rowIndex, row, columns, doc);
						excelRows.Add(excelRow);
					}
				}
			}

			return excelRows;
		}

		private ExcelRow ReadExcelRow(int rowIndex, OpenXmlElement row, IReadOnlyList<ExcelColumn> excelColumns, SpreadsheetDocument doc)
		{
			var excelRow = new ExcelRow
			{
				Index = rowIndex
			};

			foreach (var cell in row.Elements<Cell>())
			{
				var columnIndex = GetColumnIndexFromCellReference(cell.CellReference.Value);
				if (columnIndex == -1)
				{
					continue;
				}

				var column = excelColumns[columnIndex];
				var value = GetCellValue(doc, cell);

				excelRow.Cells.Add(new ExcelCell
				{
					Column = column,
					Value = value
				});
			}

			// fill in empty cells
			FillEmptyRowCells(excelRow, excelColumns);

			return excelRow;
		}

		private static void FillEmptyRowCells(ExcelRow excelRow, IReadOnlyCollection<ExcelColumn> excelColumns)
		{
			if (excelRow.Cells.Count >= excelColumns.Count)
			{
				return;
			}

			foreach (var excelColumn in excelColumns)
			{
				var exists = excelRow.Cells.Exists(a => a.Column.Index == excelColumn.Index);
				if (!exists)
				{
					excelRow.Cells.Insert(excelColumn.Index, new ExcelCell
					{
						Column = excelColumn,
						Value = string.Empty
					});
				}
			}
		}

		private static string GetCellValue(SpreadsheetDocument doc, CellType cell)
		{
			var value = cell.CellValue.Text;
			if (cell.DataType?.Value == CellValues.SharedString)
			{
				return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
			}

			return value;
		}

		private static int GetColumnIndexFromCellReference(string value)
		{
			var index = -1;

			if (string.IsNullOrEmpty(value))
			{
				return index;
			}

			var regex = new Regex(@"(?<Column>[a-zA-Z]+)(?<Row>[0-9]+)", RegexOptions.IgnoreCase);
			var match = regex.Match(value);
			if (match.Success)
			{
				var column = match.Groups["Column"].Value;
				var row = match.Groups["Row"].Value;

				if (!string.IsNullOrEmpty(column) && column.Length == 1)
				{
					var abc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
					index = abc.IndexOf(column, StringComparison.InvariantCultureIgnoreCase);
				}
			}

			return index;
		}
	}
}
