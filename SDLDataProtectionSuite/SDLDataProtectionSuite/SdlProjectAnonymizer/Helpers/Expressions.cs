using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers
{
	public static class Expressions
	{
		public static void ExportExpressions(string filePath, List<RegexPattern> patterns)
		{
			var package = GetSheetInfo(filePath, true);
			var excelDocument = new Excel();			
			excelDocument.AddBasicStyles(package);
			var worksheet = excelDocument.AddWorksheet(package, "Exported expressions");
			WorksheetPart worksheetPart = package.WorkbookPart.AddNewPart<WorksheetPart>();

			//var lineNumber = 1;
			var order = 0;

			var rowIndex = Convert.ToUInt32(1);

			// write the header row
			excelDocument.SetCellValue(package, worksheet, 1, rowIndex, "ID");
			excelDocument.SetCellValue(package, worksheet, 2, rowIndex, "Order");
			excelDocument.SetCellValue(package, worksheet, 3, rowIndex, "Rule");
			excelDocument.SetCellValue(package, worksheet, 4, rowIndex, "Description");

			for (int i = patterns.Count - 1; i >= 0; i--)
			{
				if (patterns[i] != null)
				{
					//lineNumber++;
					rowIndex++;
					excelDocument.SetCellValue(package, worksheet, 1, rowIndex, patterns[i].Id);
					int orderNumber = order++;
					excelDocument.SetCellValue(package, worksheet, 2, rowIndex, orderNumber.ToString());
					excelDocument.SetCellValue(package, worksheet, 3, rowIndex, patterns[i].Pattern);
					excelDocument.SetCellValue(package, worksheet, 4, rowIndex, patterns[i].Description);
				}
			}
			excelDocument.SetColumnWidth(worksheet, 1, 10);
			excelDocument.SetColumnWidth(worksheet, 2, 10);
			excelDocument.SetColumnWidth(worksheet, 3, 50);
			excelDocument.SetColumnWidth(worksheet, 4, 35);

			TableDefinitionPart tableDefinitionPart1 = worksheetPart.AddNewPart<TableDefinitionPart>("rId1");
			GenerateTableDefinitionPart1Content(tableDefinitionPart1);
			worksheet.Save();
		}

		public static List<RegexPattern> GetImportedExpressions(List<string> files)
		{
			var patterns = new List<RegexPattern>();
			foreach (var file in files)
			{
				//Open the Excel file in Read Mode using OpenXml.
				using (SpreadsheetDocument doc = SpreadsheetDocument.Open(file, false))
				{
					//Read the first Sheet from Excel file.
					Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

					//Get the Worksheet instance.
					Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

					//Fetch all the rows present in the Worksheet.
					IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

					//Loop through the Worksheet rows.
					foreach (Row row in rows)
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
							if (!IsValidColumnHeader(colValue[0].InnerText?.ToString(), "ID") |
								!IsValidColumnHeader(colValue[1].InnerText?.ToString(), "Order") |
								!IsValidColumnHeader(colValue[2].InnerText?.ToString(), "Rule") |
								!IsValidColumnHeader(colValue[3].InnerText?.ToString(), "Description")
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
									pattern.Id = cellValue?.ToString() ?? string.Empty;
								}
								else if (address.Contains("C"))
								{
									pattern.Pattern = cellValue?.ToString() ?? string.Empty;
								}
								else if (address.Contains("D"))
								{
									pattern.Description = cellValue?.ToString() ?? string.Empty;
								}
							}
							patterns.Add(pattern);
						}
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
			
		public static Worksheet GetworksheetBySheetName(string filePath,string sheetName)
		{
			using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, true))
			{
				var workbookPart = document.WorkbookPart;
				string relationshipId = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name.Equals(sheetName))?.Id;

				var worksheet = ((WorksheetPart)workbookPart.GetPartById(relationshipId)).Worksheet;

				return worksheet;
			}
		}
		private static void GenerateTableDefinitionPart1Content(TableDefinitionPart tableDefinitionPart1)
		{
			Table table1 = new Table() { Id = (UInt32Value)1U, Name = "tableData", DisplayName = "tableData", Reference = "A1:D2", TotalsRowShown = false };
			AutoFilter autoFilter1 = new AutoFilter() { Reference = "A1:D2" };

			TableColumns tableColumns1 = new TableColumns() { Count = (UInt32Value)4U };
			TableColumn tableColumn1 = new TableColumn() { Id = (UInt32Value)1U, Name = "ID" };
			TableColumn tableColumn2 = new TableColumn() { Id = (UInt32Value)2U, Name = "Order" };
			TableColumn tableColumn3 = new TableColumn() { Id = (UInt32Value)3U, Name = "Rule" };
			TableColumn tableColumn4 = new TableColumn() { Id = (UInt32Value)4U, Name = "Description" };

			tableColumns1.Append(tableColumn1);
			tableColumns1.Append(tableColumn2);
			tableColumns1.Append(tableColumn3);
			tableColumns1.Append(tableColumn4);
			TableStyleInfo tableStyleInfo1 = new TableStyleInfo() { Name = "TableStyleLight17", ShowFirstColumn = false, ShowLastColumn = false, ShowRowStripes = true, ShowColumnStripes = false };

			table1.Append(autoFilter1);
			table1.Append(tableColumns1);
			table1.Append(tableStyleInfo1);

			tableDefinitionPart1.Table = table1;
		}
		
		private static string GetValue(SpreadsheetDocument doc, Cell cell)
		{
			string value = cell.CellValue.InnerText;
			if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
			{
				return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
			}
			return value;
		}
		public static SpreadsheetDocument GetSheetInfo(string filePath, bool isExport = false)
		{
			if (isExport)
			{
				CreateExcelWithPatterns(ref filePath);
			}
			var mySpreadsheet = SpreadsheetDocument.Open(filePath, true);
			return mySpreadsheet;
		}
		private static void CreateExcelWithPatterns(ref string filePath)
		{
			try
			{
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
				// Create a spreadsheet document by supplying the filepath.
				// By default, AutoSave = true, Editable = true, and Type = xlsx.
				using (var spreadsheet = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
				{
					spreadsheet.AddWorkbookPart();
					spreadsheet.WorkbookPart.Workbook = new Workbook();
					var wsPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
					wsPart.Worksheet = new Worksheet();

					var stylesPart = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();
					stylesPart.Stylesheet = new Stylesheet();

					// blank font list
					stylesPart.Stylesheet.Fonts = new DocumentFormat.OpenXml.Spreadsheet.Fonts();
					stylesPart.Stylesheet.Fonts.Count = 1;
					stylesPart.Stylesheet.Fonts.AppendChild(new DocumentFormat.OpenXml.Presentation.Font());

					// create fills
					stylesPart.Stylesheet.Fills = new Fills();

					// create a solid red fill
					var solidRed = new PatternFill() { PatternType = PatternValues.Solid };
					solidRed.ForegroundColor = new ForegroundColor { Rgb = HexBinaryValue.FromString("FFFF0000") }; // red fill
					solidRed.BackgroundColor = new BackgroundColor { Indexed = 64 };

					stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } }); // required, reserved by Excel
					stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } }); // required, reserved by Excel
					stylesPart.Stylesheet.Fills.AppendChild(new Fill { PatternFill = solidRed });
					stylesPart.Stylesheet.Fills.Count = 3;

					// blank border list
					stylesPart.Stylesheet.Borders = new Borders();
					stylesPart.Stylesheet.Borders.Count = 1;
					stylesPart.Stylesheet.Borders.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Border());

					// blank cell format list
					stylesPart.Stylesheet.CellStyleFormats = new CellStyleFormats();
					stylesPart.Stylesheet.CellStyleFormats.Count = 1;
					stylesPart.Stylesheet.CellStyleFormats.AppendChild(new CellFormat());

					// cell format list
					stylesPart.Stylesheet.CellFormats = new CellFormats();
					// empty one for index 0, seems to be required
					stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat());
					// cell format references style format 0, font 0, border 0, fill 2 and applies the fill
					stylesPart.Stylesheet.CellFormats.AppendChild(new CellFormat { FormatId = 0, FontId = 0, BorderId = 0, FillId = 2, ApplyFill = true }).AppendChild(new Alignment { Horizontal = DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center });
					stylesPart.Stylesheet.CellFormats.Count = 2;

					stylesPart.Stylesheet.Save();

					var sheetData = wsPart.Worksheet.AppendChild(new SheetData());
					sheetData.AppendChild(new Row());
					wsPart.Worksheet.Save();
					spreadsheet.WorkbookPart.Workbook.AppendChild(new Sheets());
					spreadsheet.WorkbookPart.Workbook.Save();
				}

			}
			catch (Exception e)
			{
				Console.Write(e);
				filePath = filePath.Insert(filePath.IndexOf(".xlsx", StringComparison.Ordinal), "(new)");
				CreateExcelWithPatterns(ref filePath);
			}
		}

	}
}