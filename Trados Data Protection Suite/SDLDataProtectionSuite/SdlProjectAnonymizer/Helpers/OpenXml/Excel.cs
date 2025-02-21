﻿using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers.OpenXml
{
	public class Excel
	{
		public static SpreadsheetDocument GetSheetInfo(string filePath, bool isExport = false)
		{
			if (isExport)
			{
				CreateExcelWithPatterns(ref filePath);
			}
			var mySpreadsheet = SpreadsheetDocument.Open(filePath, true);
			return mySpreadsheet;
		}

		/// <summary>
		/// Add a single string to shared strings table.
		/// Shared string table is created if it doesn't exist.
		/// </summary>
		/// <param name="spreadsheet">Spreadsheet to use</param>
		/// <param name="stringItem">string to add</param>
		/// <param name="save">Save the shared string table</param>
		/// <returns></returns>
		public bool AddSharedString(SpreadsheetDocument spreadsheet, string stringItem, bool save = false)
		{
			var sharedStringTable = spreadsheet.WorkbookPart.SharedStringTablePart.SharedStringTable;

			if (0 != sharedStringTable.Count(item => item.InnerText == stringItem))
				return true;
			sharedStringTable.AppendChild(
				new SharedStringItem(
					new Text(stringItem)));

			// Save the changes
			if (save)
			{
				sharedStringTable.Save();
			}

			return true;
		}

		/// <summary>
		/// Adds a new worksheet to the workbook
		/// </summary>
		/// <param name="spreadsheet">Spreadsheet to use</param>
		/// <param name="name">Name of the worksheet</param>
		/// <returns>True if successful</returns>
		public Worksheet AddWorksheet(SpreadsheetDocument spreadsheet, string name)
		{
			// Add the worksheetpart
			var worksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
			worksheetPart.Worksheet = new Worksheet(new SheetData());
			worksheetPart.Worksheet.Save();

			var sheets = spreadsheet.WorkbookPart.Workbook.AppendChild(new Sheets());
			// Add the sheet and make relation to workbook
			var sheet = new Sheet
			{
				Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
				SheetId = (uint)(spreadsheet.WorkbookPart.Workbook.Sheets.Count() + 1),
				Name = name
			};
			sheets.Append(sheet);
			spreadsheet.WorkbookPart.Workbook.Save();

			return worksheetPart.Worksheet;
		}

		/// <summary>
		/// Converts a column number to column name (i.e. A, B, C..., AA, AB...)
		/// </summary>
		/// <param name="columnIndex">Index of the column</param>
		/// <returns>Column name</returns>
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

		/// <summary>
		/// Returns the index of a shared string.
		/// </summary>
		/// <param name="spreadsheet">Spreadsheet to use</param>
		/// <param name="stringItem">String to search for</param>
		/// <returns>Index of a shared string. -1 if not found</returns>
		public int IndexOfSharedString(SpreadsheetDocument spreadsheet, string stringItem)
		{
			var sharedStringTable = spreadsheet.WorkbookPart.SharedStringTablePart.SharedStringTable;
			var found = false;
			var index = 0;

			foreach (var sharedString in sharedStringTable.Elements<SharedStringItem>())
			{
				if (sharedString.InnerText == stringItem)
				{
					found = true;
					break;
				}
				index++;
			}

			return found ? index : -1;
		}

		/// <summary>
		/// Sets a string value to a cell
		/// </summary>
		/// <param name="spreadsheet">Spreadsheet to use</param>
		/// <param name="worksheet">Worksheet to use</param>
		/// <param name="columnIndex">Index of the column</param>
		/// <param name="rowIndex">Index of the row</param>
		/// <param name="stringValue">String value to set</param>
		/// <param name="useSharedString">Use shared strings? If true and the string isn't found in shared strings, it will be added</param>
		/// <param name="save">Save the worksheet</param>
		/// <returns>True if successful</returns>
		public bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, string stringValue, bool useSharedString = false, bool save = false)
		{
			var columnValue = stringValue;
			CellValues cellValueType;

			// Add the shared string if necessary
			if (useSharedString)
			{
				if (IndexOfSharedString(spreadsheet, stringValue) == -1)
				{
					AddSharedString(spreadsheet, stringValue, true);
				}
				columnValue = IndexOfSharedString(spreadsheet, stringValue).ToString();
				cellValueType = CellValues.SharedString;
			}
			else
			{
				cellValueType = CellValues.String;
			}

			return SetCellValue(worksheet, columnIndex, rowIndex, cellValueType, columnValue, null, save);
		}

		/// <summary>
		/// Sets the column width
		/// </summary>
		/// <param name="worksheet">Worksheet to use</param>
		/// <param name="columnIndex">Index of the column</param>
		/// <param name="width">Width to set</param>
		/// <returns>True if successful</returns>
		public bool SetColumnWidth(Worksheet worksheet, int columnIndex, int width)
		{
			// Get the column collection exists
			var columns = worksheet.Elements<Columns>().FirstOrDefault();
			if (columns == null)
			{
				return false;
			}
			// Get the column
			var column = columns.Elements<Column>().FirstOrDefault(item => item.Min == columnIndex);
			if (column != null)
			{
				column.Width = width;
				column.CustomWidth = true;
			}

			worksheet.Save();

			return true;
		}

		private static void CreateExcelWithPatterns(ref string filePath)
		{
			try
			{
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}

				using var spreadsheet = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);

				spreadsheet.AddWorkbookPart();
				spreadsheet.WorkbookPart.Workbook = new Workbook();

				spreadsheet.WorkbookPart.Workbook.Save();
			}
			catch (Exception e)
			{
				Console.Write(e);
				filePath = filePath.Insert(filePath.IndexOf(".xlsx", StringComparison.Ordinal), "(new)");
				CreateExcelWithPatterns(ref filePath);
			}
		}

		/// <summary>
		/// Sets a cell value. The row and the cell are created if they do not exist. If the cell exists, the contents of the cell is overwritten
		/// </summary>
		/// <param name="worksheet">Worksheet to use</param>
		/// <param name="columnIndex">Index of the column</param>
		/// <param name="rowIndex">Index of the row</param>
		/// <param name="valueType">Type of the value</param>
		/// <param name="value">The actual value</param>
		/// <param name="styleIndex">Index of the style to use. Null if no style is to be defined</param>
		/// <param name="save">Save the worksheet?</param>
		/// <returns>True if successful</returns>
		private bool SetCellValue(Worksheet worksheet, uint columnIndex, uint rowIndex, CellValues valueType, string value, uint? styleIndex, bool save = false)
		{
			var sheetData = worksheet.GetFirstChild<SheetData>();
			Row row;
			Row previousRow = null;
			Cell cell;
			Cell previousCell = null;
			var cellAddress = ColumnNameFromIndex(columnIndex) + rowIndex;

			// Check if the row exists, create if necessary
			if (sheetData.Elements<Row>().Count(item => item.RowIndex == rowIndex) != 0)
			{
				row = sheetData.Elements<Row>().First(item => item.RowIndex == rowIndex);
			}
			else
			{
				row = new Row { RowIndex = rowIndex };
				//sheetData.Append(row);
				for (var counter = rowIndex - 1; counter > 0; counter--)
				{
					previousRow = sheetData.Elements<Row>().FirstOrDefault(item => item.RowIndex == counter);
					if (previousRow != null)
					{
						break;
					}
				}
				sheetData.InsertAfter(row, previousRow);
			}

			// Check if the cell exists, create if necessary
			if (row.Elements<Cell>().Any(item => item.CellReference.Value == cellAddress))
			{
				cell = row.Elements<Cell>().First(item => item.CellReference.Value == cellAddress);
			}
			else
			{
				// Find the previous existing cell in the row
				for (var counter = columnIndex - 1; counter > 0; counter--)
				{
					previousCell = row.Elements<Cell>().FirstOrDefault(item => item.CellReference.Value == ColumnNameFromIndex(counter) + rowIndex);
					if (previousCell != null)
					{
						break;
					}
				}
				cell = new Cell { CellReference = cellAddress };
				row.InsertAfter(cell, previousCell);
			}

			// Add the value
			cell.CellValue = new CellValue(value);

			if (styleIndex != null)
			{
				cell.StyleIndex = styleIndex.Value;
			}
			if (valueType != CellValues.Date)
			{
				cell.DataType = new EnumValue<CellValues>(valueType);
			}

			if (save)
			{
				worksheet.Save();
			}

			return true;
		}
	}
}