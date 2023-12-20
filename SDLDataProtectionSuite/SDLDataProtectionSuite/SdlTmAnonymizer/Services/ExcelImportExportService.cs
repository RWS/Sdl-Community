using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers.OpenXml;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.Log;
using Sdl.LanguagePlatform.TranslationMemory;
using Rule = Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.Rule;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services
{
	public class ExcelImportExportService
	{
		public void ExportCustomFields(string filePath, List<CustomField> customFields)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			using var package = Excel.GetSheetInfo(filePath, true);
			var excelDocument = new Excel();
			var worksheet = excelDocument.AddWorksheet(package, "Exported custom fields");

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

			worksheet.Save();
		}

		public void ExportLogReportToExcel(string filePath, Report report)
		{
			if (File.Exists(filePath))
				File.Delete(filePath);

			using var package = Excel.GetSheetInfo(filePath, true);

			var excelDocument = new Excel();

			var worksheet = excelDocument.AddWorksheet(package, "Log Report");
			var sheet = new SheetView();
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

			AddLogReportDataTable(report, package, excelDocument, worksheet, ref lineNumber);
			worksheet.Save();
		}

		public void ExportRules(string filePath, List<Rule> rules)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}

			using var package = Excel.GetSheetInfo(filePath, true);

			var excelDocument = new Excel();
			var worksheet = excelDocument.AddWorksheet(package, "Exported expressions");
			var rowIndex = Convert.ToUInt32(1);

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

			worksheet.Save();
		}

		public void ExportUsers(string filePath, List<User> users)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}

			using var package = Excel.GetSheetInfo(filePath, true);

			var excelDocument = new Excel();
			var worksheet = excelDocument.AddWorksheet(package, "Exported system fields");
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

			worksheet.Save();
		}

		public List<CustomField> ImportCustomFields(List<string> files)
		{
			var customFields = new List<CustomField>();
			foreach (var file in files)
			{
				using (var doc = SpreadsheetDocument.Open(file, false))
				{
					//Read the first Sheet from Excel file.
					var sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

					var excelReader = new ExcelReader(doc.WorkbookPart.SharedStringTablePart?.SharedStringTable);

					//Get the Worksheet instance.
					var worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

					//Fetch all the rows present in the Worksheet.
					var rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

					//Loop through the Worksheet rows.
					foreach (var row in rows)
					{
						var colValue = row.ToList();
						if (row.RowIndex.Value == 1)
						{
							if (!IsValidColumnHeader(excelReader.GetCellText(colValue[0].InnerText), "Name") |
								!IsValidColumnHeader(excelReader.GetCellText(colValue[1].InnerText), "Type") |
								!IsValidColumnHeader(excelReader.GetCellText(colValue[2].InnerText), "Value") |
								!IsValidColumnHeader(excelReader.GetCellText(colValue[3].InnerText), "New Value")
								)
							{
								return null;
							}
						}

						if (row.RowIndex.Value <= 1)
							continue;
						foreach (var cell in colValue)
						{
							var address = ((CellType)cell).CellReference?.Value;
							var cellValue = excelReader.GetCellText(cell.InnerText);
							var customFieldName = cellValue;
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
									details.Value = cellValue;
									//D column contains alias value
									var newValue = cellValue;
									if (newValue != null)
									{
										details.NewValue = newValue;
									}
									existingField.FieldValues.Add(details);
									break;
								}
							}
						}
					}
				}
			}
			return customFields;
		}

		public List<Rule> ImportedRules(List<string> files)
		{
			var rules = new List<Rule>();
			foreach (var file in files)
			{
				using (var doc = SpreadsheetDocument.Open(file, false))
				{
					//Read the first Sheet from Excel file.
					var sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

					var excelReader = new ExcelReader(doc.WorkbookPart.SharedStringTablePart?.SharedStringTable);

					//Get the Worksheet instance.
					var worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

					//Fetch all the rows present in the Worksheet.
					var rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

					//Loop through the Worksheet rows.
					foreach (var row in rows)
					{
						var rule = new Rule
						{
							IsSelected = true
						};
						var colValue = row.ToList();
						if (row.RowIndex.Value == 1)
						{
							if (!IsValidColumnHeader(excelReader.GetCellText(colValue[0].InnerText), "ID") |
								!IsValidColumnHeader(excelReader.GetCellText(colValue[1].InnerText), "Order") |
								!IsValidColumnHeader(excelReader.GetCellText(colValue[2].InnerText), "Rule") |
								!IsValidColumnHeader(excelReader.GetCellText(colValue[3].InnerText), "Description")
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
								var address = ((CellType)colValue[i]).CellReference.Value;
								var cellValue = excelReader.GetCellText(colValue[i].InnerText);
								if (address.Contains("A"))
								{
									rule.Id = cellValue ?? string.Empty;
								}
								else if (address.Contains("B"))
								{
									rule.Order = cellValue != null && int.TryParse(cellValue, out var value) ? value : 0;
								}
								else if (address.Contains("C"))
								{
									rule.Name = cellValue ?? string.Empty;
								}
								else if (address.Contains("D"))
								{
									rule.Description = cellValue ?? string.Empty;
								}
							}
							rules.Add(rule);
						}
					}
				}
			}
			return rules;
		}

		public List<User> ImportUsers(List<string> files)
		{
			var listOfUsers = new List<User>();
			foreach (var file in files)
			{
				using (var doc = SpreadsheetDocument.Open(file, false))
				{
					//Read the first Sheet from Excel file.
					var sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

					var excelReader = new ExcelReader(doc.WorkbookPart.SharedStringTablePart?.SharedStringTable);

					var worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

					//Fetch all the rows present in the Worksheet.
					var rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

					//Loop through the Worksheet rows.
					foreach (var row in rows)
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
							if (!IsValidColumnHeader(excelReader.GetCellText(colValue[0].InnerText), "User Name") |
								!IsValidColumnHeader(excelReader.GetCellText(colValue[1].InnerText), "New Value")
								)
							{
								return null;
							}
						}

						//Use the first row to add columns to DataTable.
						if (row.RowIndex.Value <= 1)
							continue;

						foreach (var cell in colValue)
						{
							var address = ((CellType)cell).CellReference.Value;
							var cellValue = excelReader.GetCellText(cell.InnerText);
							if (address.Contains("A") && cellValue != null)
							{
								user.UserName = cellValue;
							}
							else if (address.Contains("B") && cellValue != null)
							{
								user.Alias = cellValue;
							}
						}

						listOfUsers.Add(user);
					}
				}
			}
			return listOfUsers;
		}

		private static void AddLogReportDataTable(Report report, SpreadsheetDocument package, Excel excelDocument, Worksheet worksheet, ref uint lineNumber)
		{
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

			foreach (var action in report.Actions)
			{
				lineNumber++;
				if (action == null)
					continue;

				excelDocument.SetCellValue(package, worksheet, 1, lineNumber, action.TmId?.Id.ToString());
				excelDocument.SetCellValue(package, worksheet, 2, lineNumber, action.Name);
				excelDocument.SetCellValue(package, worksheet, 3, lineNumber, action.Type);

				excelDocument.SetCellValue(package, worksheet, 5, lineNumber, string.IsNullOrEmpty(action.Result) ? "OK" : action.Result);
			}

			lineNumber++;
		}

		private static void AddLogReportHeaderItem(SpreadsheetDocument package, Excel excelDocument, Worksheet worksheet, string labelText, object value, ref uint lineNumber)
		{
			excelDocument.SetCellValue(package, worksheet, 1, lineNumber, labelText);
			excelDocument.SetCellValue(package, worksheet, 2, lineNumber, Convert.ToString(value));
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