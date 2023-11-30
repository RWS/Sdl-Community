using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using DocumentFormat.OpenXml;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;
//using OfficeOpenXml.Table;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers.OpenXml;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.Log;
using Sdl.LanguagePlatform.TranslationMemory;
using SegmentComparer.Structure;
using Rule = Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.Rule;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services
{
	public class ExcelImportExportService
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

		private static string GetValue(SpreadsheetDocument doc, Cell cell)
		{
			string value = cell.CellValue.InnerText;
			if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
			{
				return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
			}
			return value;
		}
		public List<CustomField> ImportCustomFields(List<string> files)
		{
			var customFields = new List<CustomField>();
			foreach (var file in files)
			{
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
						var colValue = row.ToList();
						if (row.RowIndex.Value == 1)
						{
							if (!IsValidColumnHeader(colValue[0].InnerText?.ToString(), "Name") |
								!IsValidColumnHeader(colValue[1].InnerText?.ToString(), "Type") |
								!IsValidColumnHeader(colValue[2].InnerText?.ToString(), "Value") |
								!IsValidColumnHeader(colValue[3].InnerText?.ToString(), "New Value") 
								)
							{
								return null;
							}
						}
						if (row.RowIndex.Value > 1)
						{
							for (var i = 0; i < colValue.Count; i++)
							{
								var address = ((DocumentFormat.OpenXml.Spreadsheet.CellType)colValue[i]).CellReference.Value;
								var cellValue = colValue[i].InnerText;
								var customFieldName = cellValue.ToString();
								var existingField = customFields.FirstOrDefault(c => c.Name.Equals(customFieldName));
								if (existingField == null)
								{
									var fieldType = cellValue;// workSheet.Cells[i, 2].Value.ToString();
									var studioCustomFieldType = (FieldValueType)Enum.Parse(typeof(FieldValueType), fieldType);
									var field = new CustomField
									{
										IsSelected = true,
										Name = customFieldName,
										ValueType = studioCustomFieldType,
										FieldValues = new List<CustomFieldValue>()
									};
									customFields.Add(field);
								}
								else
								{
									var details = new CustomFieldValue
									{
										Value = string.Empty,
										NewValue = string.Empty
									};

									//C column contains the original value
									if (address.Contains("C") && cellValue != null)
									{
										details.Value = cellValue.ToString();
										//D column contains alias value
										var newValue = cellValue;
										if (newValue != null)
										{
											details.NewValue = newValue.ToString();
										}
										existingField.FieldValues.Add(details);
										break;
									}
								}
							}
						}
					}
				}
			}
			return customFields;
		}

		//public List<CustomField> ImportCustomFields(List<string> files)
		//{
		//	var customFields = new List<CustomField>();
		//	foreach (var file in files)
		//	{
		//		var package = GetExcelPackage(file);
		//		var workSheet = package.Workbook.Worksheets[1];

		//		var colmun01 = workSheet.Cells[1, 1].Value;
		//		var column02 = workSheet.Cells[1, 2].Value;
		//		var column03 = workSheet.Cells[1, 3].Value;
		//		var column04 = workSheet.Cells[1, 4].Value;

		//		if (!IsValidColumnHeader(colmun01?.ToString(), "Name") |
		//			!IsValidColumnHeader(column02?.ToString(), "Type") |
		//			!IsValidColumnHeader(column03?.ToString(), "Value") |
		//			!IsValidColumnHeader(column04?.ToString(), "New Value"))
		//		{
		//			return null;
		//		}

		//		for (var i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
		//		{
		//			var customFieldName = workSheet.Cells[i, 1].Value.ToString();
		//			var existingField = customFields.FirstOrDefault(c => c.Name.Equals(customFieldName));
		//			if (existingField == null)
		//			{
		//				var fieldType = workSheet.Cells[i, 2].Value.ToString();
		//				var studioCustomFieldType = (FieldValueType)Enum.Parse(typeof(FieldValueType), fieldType);
		//				var field = new CustomField
		//				{
		//					IsSelected = true,
		//					Name = customFieldName,
		//					ValueType = studioCustomFieldType,
		//					FieldValues = new List<CustomFieldValue>()
		//				};
		//				customFields.Add(field);
		//			}
		//			for (var j = workSheet.Dimension.Start.Column + 2; j <= workSheet.Dimension.End.Column; j++)
		//			{
		//				var address = workSheet.Cells[i, j].Address;
		//				// get the filed with the same name
		//				var cellValue = workSheet.Cells[i, j].Value;
		//				var customExistingField = customFields.FirstOrDefault(c => c.Name.Equals(customFieldName));

		//				if (customExistingField != null)
		//				{
		//					var details = new CustomFieldValue
		//					{
		//						Value = string.Empty,
		//						NewValue = string.Empty
		//					};

		//					//C column contains the original value
		//					if (address.Contains("C") && cellValue != null)
		//					{
		//						details.Value = cellValue.ToString();
		//						//D column contains alias value
		//						var newValue = workSheet.Cells[i, j + 1].Value;
		//						if (newValue != null)
		//						{
		//							details.NewValue = newValue.ToString();
		//						}
		//						customExistingField.FieldValues.Add(details);
		//						break;
		//					}
		//				}
		//			}
		//		}
		//	}
		//	return customFields;
		//}

		public void ExportCustomFields(string filePath, List<CustomField> customFields)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			var package = GetSheetInfo(filePath, true);
			var excelDocument = new Excel();
			excelDocument.AddBasicStyles(package);
			var worksheet = excelDocument.AddWorksheet(package, "Exported custom fields");
			WorksheetPart worksheetPart = package.WorkbookPart.AddNewPart<WorksheetPart>();

			var rowIndex = Convert.ToUInt32(1);

			// write the header row
			excelDocument.SetCellValue(package, worksheet, 1, rowIndex, "Name");
			excelDocument.SetCellValue(package, worksheet, 2, rowIndex, "Type");
			excelDocument.SetCellValue(package, worksheet, 3, rowIndex, "Value");
			excelDocument.SetCellValue(package, worksheet, 4, rowIndex, "New Value");
			excelDocument.SetColumnWidth(worksheet, 1, 20);
			excelDocument.SetColumnWidth(worksheet, 2, 25);
			excelDocument.SetColumnWidth(worksheet, 3, 25);
			excelDocument.SetColumnWidth(worksheet, 4, 25);

			foreach (var field in customFields)
			{
				if (field != null)
				{
					foreach (var detail in field.FieldValues)
					{
						rowIndex++;
						excelDocument.SetCellValue(package, worksheet, 1, rowIndex, field.Name);
						excelDocument.SetCellValue(package, worksheet, 2, rowIndex, field.ValueType.ToString());
						excelDocument.SetCellValue(package, worksheet, 3, rowIndex, detail.Value);
						excelDocument.SetCellValue(package, worksheet, 4, rowIndex, detail.NewValue);
					}
				}
			}
			TableDefinitionPart tableDefinitionPart1 = worksheetPart.AddNewPart<TableDefinitionPart>("rId1");
			GenerateTableDefinitionPart1Content(tableDefinitionPart1);
			worksheet.Save();
		}
		private static void GenerateTableDefinitionPart1Content(TableDefinitionPart tableDefinitionPart1)
		{
			var table1 = new DocumentFormat.OpenXml.Spreadsheet.Table() { Id = (UInt32Value)1U, Name = "tableData", DisplayName = "tableData", Reference = "A1:D2", TotalsRowShown = false };
			AutoFilter autoFilter1 = new AutoFilter() { Reference = "A1:D2" };

			TableColumns tableColumns1 = new TableColumns() { Count = (UInt32Value)4U };
			TableColumn tableColumn1 = new TableColumn() { Id = (UInt32Value)1U, Name = "Name" };
			TableColumn tableColumn2 = new TableColumn() { Id = (UInt32Value)2U, Name = "Type" };
			TableColumn tableColumn3 = new TableColumn() { Id = (UInt32Value)3U, Name = "Value" };
			TableColumn tableColumn4 = new TableColumn() { Id = (UInt32Value)4U, Name = "New Value" };

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

		public List<Rule> ImportedRules(List<string> files)
		{
			var rules = new List<Rule>();
			foreach (var file in files)
			{
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
						var rule = new Rule
						{
							IsSelected = true
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
							for(var i=0;i <colValue.Count;i++)
							{
								var address = ((DocumentFormat.OpenXml.Spreadsheet.CellType)colValue[i]).CellReference.Value;
								var cellValue = colValue[i].InnerText;
								if (address.Contains("A"))
								{
									rule.Id = cellValue?.ToString() ?? string.Empty;
								}
								else if (address.Contains("B"))
								{
									rule.Order = cellValue != null && int.TryParse(cellValue.ToString(), out var value) ? value : 0;
								}
								else if (address.Contains("C"))
								{
									rule.Name = cellValue?.ToString() ?? string.Empty;
								}
								else if (address.Contains("D"))
								{
									rule.Description = cellValue?.ToString() ?? string.Empty;
								}								
							}
							rules.Add(rule);
						}
					}
				}
			}
			return rules;
		}

		public void ExportRules(string filePath, List<Rule> rules)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			using (var package = GetSheetInfo(filePath, true))
			{
				var excelDocument = new Excel();
				excelDocument.AddBasicStyles(package);
				var worksheet = excelDocument.AddWorksheet(package, "Exported expressions");
				WorksheetPart worksheetPart = package.WorkbookPart.AddNewPart<WorksheetPart>();
				var rowIndex = Convert.ToUInt32(1);
				// write the header row
				excelDocument.SetCellValue(package, worksheet, 1, rowIndex, "ID");
				excelDocument.SetCellValue(package, worksheet, 2, rowIndex, "Order");
				excelDocument.SetCellValue(package, worksheet, 3, rowIndex, "Rule");
				excelDocument.SetCellValue(package, worksheet, 4, rowIndex, "Description");

				foreach (var rule in rules.OrderBy(a => a.Order))
				{
					rowIndex++;
					excelDocument.SetCellValue(package, worksheet, 1, rowIndex, rule.Id);
					excelDocument.SetCellValue(package, worksheet, 2, rowIndex, rule.Order.ToString());
					excelDocument.SetCellValue(package, worksheet, 3, rowIndex, rule.Name);
					excelDocument.SetCellValue(package, worksheet, 4, rowIndex, rule.Description);
				}
				excelDocument.SetColumnWidth(worksheet, 1, 10);
				excelDocument.SetColumnWidth(worksheet, 2, 10);
				excelDocument.SetColumnWidth(worksheet, 3, 50);
				excelDocument.SetColumnWidth(worksheet, 4, 35);

				TableDefinitionPart tableDefinitionPart1 = worksheetPart.AddNewPart<TableDefinitionPart>("rId1");
				GenerateTableDefinitionPart1Content(tableDefinitionPart1);
				worksheet.Save();
			}
		}

		public List<User> ImportUsers(List<string> files)
		{
			var listOfUsers = new List<User>();
			foreach (var file in files)
			{
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
						var user = new User
						{
							IsSelected = true,
							UserName = string.Empty,
							Alias = string.Empty
						};

						var colValue = row.ToList();
						if (row.RowIndex.Value == 1)
						{
							if (!IsValidColumnHeader(colValue[0].InnerText?.ToString(), "User Name") |
								!IsValidColumnHeader(colValue[1].InnerText?.ToString(), "New Value") 
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
								if (address.Contains("A") && cellValue != null)
								{
									user.UserName = cellValue.ToString();
								}
								else if (address.Contains("B") && cellValue != null)
								{
									user.Alias = cellValue.ToString();
								}
							}
							listOfUsers.Add(user);
						}
					}
				}
			}
			return listOfUsers;
		}
		public void ExportUsers(string filePath, List<User> users)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			using (var package = GetSheetInfo(filePath, true))
			{
				var excelDocument = new Excel();
				excelDocument.AddBasicStyles(package);
				var worksheet = excelDocument.AddWorksheet(package, "Exported system fields");
				WorksheetPart worksheetPart = package.WorkbookPart.AddNewPart<WorksheetPart>();
				var rowIndex = Convert.ToUInt32(1);

				// write the header row
				excelDocument.SetCellValue(package, worksheet, 1, rowIndex, "User Name");
				excelDocument.SetCellValue(package, worksheet, 2, rowIndex, "New Value");
				foreach (var user in users)
				{
					if (user != null)
					{
						rowIndex++;
						excelDocument.SetCellValue(package, worksheet, 1, rowIndex, user.UserName);
						excelDocument.SetCellValue(package, worksheet, 2, rowIndex, user.Alias);
					}
				}

				excelDocument.SetColumnWidth(worksheet, 1, 30);
				excelDocument.SetColumnWidth(worksheet, 2, 30);

				TableDefinitionPart tableDefinitionPart1 = worksheetPart.AddNewPart<TableDefinitionPart>("rId1");
				GenerateTableDefinitionPart1Content(tableDefinitionPart1);
				worksheet.Save();
			}
		}

		public void ExportLogReportToExcel(string filePath, Model.Log.Report report)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			using (var package = GetSheetInfo(filePath, true))
			{
				var excelDocument = new Excel();
				excelDocument.AddBasicStyles(package);
				var worksheet = excelDocument.AddWorksheet(package, "Log Report");
				SheetView sheet = new SheetView();
				sheet.ShowGridLines = true;
				worksheet.SheetViews.Append(sheet);
				var lineNumber = Convert.ToUInt32(1);

				AddLogReportHeaderItem(package, excelDocument, worksheet, "Report Path:", report.ReportFullPath, ref lineNumber);
				AddLogReportHeaderItem(package, excelDocument, worksheet, "Report Type:", report.Type, ref lineNumber);
				AddLogReportHeaderItem(package, excelDocument, worksheet, "Process Scope:", report.Scope, ref lineNumber);
				AddLogReportHeaderItem(package, excelDocument, worksheet, "TM Path:", report.TmFile.Path, ref lineNumber);
				AddLogReportHeaderItem(package, excelDocument, worksheet, "Server TM:", report.TmFile.IsServerTm, ref lineNumber);
				AddLogReportHeaderItem(package, excelDocument, worksheet, "Created:", report.Created.ToString(CultureInfo.InvariantCulture), ref lineNumber);
				AddLogReportHeaderItem(package, excelDocument, worksheet, "Updated Units:", report.UpdatedCount, ref lineNumber);

				//var table = AddLogReportDataTable(report, package,excelDocument, worksheet, ref lineNumber);
				AddLogReportDataTable(report, package, excelDocument, worksheet, ref lineNumber);
				worksheet.Save();
			}
		}

		private static void AddLogReportDataTable(Report report, SpreadsheetDocument package, Excel excelDocument, Worksheet worksheet, ref uint lineNumber)
		{	
			WorksheetPart worksheetPart = package.WorkbookPart.AddNewPart<WorksheetPart>();

			excelDocument.SetCellValue(package, worksheet, 1, lineNumber, "ID");
			excelDocument.SetCellValue(package, worksheet, 2, lineNumber, "Name");
			excelDocument.SetCellValue(package, worksheet, 3, lineNumber, "Type");
			excelDocument.SetCellValue(package, worksheet, 4, lineNumber, "Original Value");
			excelDocument.SetCellValue(package, worksheet, 5, lineNumber, "New Value");
			excelDocument.SetCellValue(package, worksheet, 6, lineNumber, "Result");

			excelDocument.SetColumnWidth(worksheet, 1, 10);
			excelDocument.SetColumnWidth(worksheet, 2, 12);
			excelDocument.SetColumnWidth(worksheet, 3, 12);
			excelDocument.SetColumnWidth(worksheet, 4, 70);
			excelDocument.SetColumnWidth(worksheet, 5, 70);
			excelDocument.SetColumnWidth(worksheet, 6, 10);

			var comparer = new SegmentComparer.Comparer();
			
			foreach (var action in report.Actions)
			{
				lineNumber++;
				if (action != null)
				{
					excelDocument.SetCellValue(package, worksheet, 1, lineNumber, action.TmId?.Id.ToString());
					excelDocument.SetCellValue(package, worksheet, 2, lineNumber, action.Name);
					excelDocument.SetCellValue(package, worksheet, 3, lineNumber, action.Type);

					var comparison = comparer.CompareSegment(action.TmId?.Id.ToString(), action.Previous, action.Value, true, 1);


					//foreach (var unit in comparison.ComparisonUnits)
					//{
					//	switch (unit.Type)
					//	{
					//		case ComparisonUnit.ComparisonType.New:
					//			{
					//				if (unit.TextType != ComparisonUnit.ContentType.Tag)
					//				{
					//					var valueNormalText = currentValue.RichText.Add(unit.Text);
					//					valueNormalText.Color = System.Drawing.Color.Black;
					//					valueNormalText.Bold = false;
					//				}
					//				else
					//				{
					//					var valueNewText = currentValue.RichText.Add(unit.Text);
					//					valueNewText.Color = System.Drawing.Color.Red;
					//					valueNewText.Bold = true;
					//				}
					//				break;
					//			}
					//		case ComparisonUnit.ComparisonType.Removed:
					//			{
					//				var previousRemovedText = previousValue.RichText.Add(unit.Text);
					//				previousRemovedText.Color = System.Drawing.Color.Red;
					//				previousRemovedText.Bold = true;
					//				break;
					//			}
					//		case ComparisonUnit.ComparisonType.Identical:
					//		case ComparisonUnit.ComparisonType.None:
					//			{
					//				var valueNormalText = currentValue.RichText.Add(unit.Text);
					//				valueNormalText.Color = System.Drawing.Color.Black;
					//				valueNormalText.Bold = false;

					//				var previousNormalText = previousValue.RichText.Add(unit.Text);
					//				previousNormalText.Color = System.Drawing.Color.Black;
					//				previousNormalText.Bold = false;
					//				break;
					//			}
					//	}
					//}

					excelDocument.SetCellValue(package, worksheet, 5, lineNumber, string.IsNullOrEmpty(action.Result) ? "OK" : action.Result);
					
					//worksheet.Cells["F" + lineNumber].Value = string.IsNullOrEmpty(action.Result) ? "OK" : action.Result;

					//worksheet.Cells["D" + lineNumber].Style.WrapText = true;
					//worksheet.Cells["E" + lineNumber].Style.WrapText = true;
				}
			}

			//var range = worksheet.Cells[startData, 1, lineNumber, 6];
			//var table = worksheet.Tables.Add(range, "tableData");
			TableDefinitionPart tableDefinitionPart1 = worksheetPart.AddNewPart<TableDefinitionPart>("rId1");
			GenerateTableDefinitionPart1Content(tableDefinitionPart1);
			lineNumber++;
		}

		private static void AddLogReportHeaderItem(SpreadsheetDocument package, Excel excelDocument, Worksheet worksheet, string labelText, object value, ref uint lineNumber)
		{

			// write the header row
			excelDocument.SetCellValue(package, worksheet, 1, lineNumber, labelText);

			excelDocument.SetCellValue(package, worksheet, 2, lineNumber, Convert.ToString(value));

			//worksheet.Cells["A" + lineNumber].Value = labelText;
			//worksheet.Cells["A" + lineNumber].Style.Font.Bold = true;
			//worksheet.Cells["C" + lineNumber].Value = value;
			//worksheet.Cells["C" + lineNumber].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
			//worksheet.Cells["A" + lineNumber + ":" + "B" + lineNumber].Merge = true;

			lineNumber++;
		}

		private static bool IsValidColumnHeader(string colmun, string expected)
		{
			if (string.Compare(colmun, expected, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				MessageBox.Show(string.Format(StringResources.Invalid_import_format_found_0_expected_column_header_name_1, colmun, expected), StringResources.SDLTM_Anonymizer_Name);

				return false;
			}

			return true;
		}
	}
}
