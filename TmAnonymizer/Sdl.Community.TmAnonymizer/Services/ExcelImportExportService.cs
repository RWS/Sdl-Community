using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Model.Log;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class ExcelImportExportService
	{
		public ExcelPackage GetExcelPackage(string filePath)
		{
			var fileInfo = new FileInfo(filePath);
			var excelPackage = new ExcelPackage(fileInfo);
			return excelPackage;
		}

		public List<CustomField> ImportCustomFields(List<string> files)
		{
			var customFields = new List<CustomField>();
			foreach (var file in files)
			{
				var package = GetExcelPackage(file);
				var workSheet = package.Workbook.Worksheets[1];
				for (var i = workSheet.Dimension.Start.Row;
					i <= workSheet.Dimension.End.Row;
					i++)
				{
					var customFieldName = workSheet.Cells[i, 1].Value.ToString();
					var existingField = customFields.FirstOrDefault(c => c.Name.Equals(customFieldName));
					if (existingField == null)
					{
						var fieldType = workSheet.Cells[i, 2].Value.ToString();
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
					for (var j = workSheet.Dimension.Start.Column + 2;
						j <= workSheet.Dimension.End.Column;
						j++)
					{
						var address = workSheet.Cells[i, j].Address;
						// get the filed with the same name
						var cellValue = workSheet.Cells[i, j].Value;
						var customExistingField = customFields.FirstOrDefault(c => c.Name.Equals(customFieldName));

						if (customExistingField != null)
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
								var newValue = workSheet.Cells[i, j + 1].Value;
								if (newValue != null)
								{
									details.NewValue = newValue.ToString();
								}
								customExistingField.FieldValues.Add(details);
								break;
							}
						}
					}
				}
			}
			return customFields;
		}

		public void ExportCustomFields(string filePath, List<CustomField> customFields)
		{
			var package = GetExcelPackage(filePath);
			var worksheet = package.Workbook.Worksheets.Add("Exported custom fields");
			var lineNumber = 1;
			foreach (var field in customFields)
			{
				if (field != null)
				{
					foreach (var detail in field.FieldValues)
					{
						worksheet.Cells["A" + lineNumber].Value = field.Name;
						worksheet.Cells["B" + lineNumber].Value = field.ValueType;
						worksheet.Cells["C" + lineNumber].Value = detail.Value;
						worksheet.Cells["D" + lineNumber].Value = detail.NewValue;
						lineNumber++;
					}
				}
			}
			package.Save();
		}

		public List<Rule> ImportedRules(List<string> files)
		{
			var patterns = new List<Rule>();
			foreach (var file in files)
			{
				var package = GetExcelPackage(file);
				var workSheet = package.Workbook.Worksheets[1];
				for (var i = workSheet.Dimension.Start.Row;
					i <= workSheet.Dimension.End.Row;
					i++)
				{
					var pattern = new Rule()
					{
						Id = Guid.NewGuid().ToString(),
						IsSelected = true,
						Description = string.Empty,
						Name = string.Empty
					};

					for (var j = workSheet.Dimension.Start.Column;
						j <= workSheet.Dimension.End.Column;
						j++)
					{
						var address = workSheet.Cells[i, j].Address;

						var cellValue = workSheet.Cells[i, j].Value;
						if (address.Contains("A"))
						{
							pattern.Name = cellValue.ToString();
						}
						else
						{
							pattern.Description = cellValue.ToString();
						}
					}
					patterns.Add(pattern);
				}
			}
			return patterns;
		}

		public void ExportRules(string filePath, List<Rule> patterns)
		{
			using (var package = GetExcelPackage(filePath))
			{
				var worksheet = package.Workbook.Worksheets.Add("Exported expressions");
				var lineNumber = 1;
				foreach (var pattern in patterns)
				{
					if (pattern != null)
					{
						worksheet.Cells["A" + lineNumber].Value = pattern.Name;
						worksheet.Cells["B" + lineNumber].Value = pattern.Description;
						lineNumber++;
					}
				}

				package.Save();
			}
		}

		public List<User> ImportUsers(List<string> files)
		{
			var listOfUsers = new List<User>();
			foreach (var file in files)
			{
				var package = GetExcelPackage(file);
				var workSheet = package.Workbook.Worksheets[1];
				for (var i = workSheet.Dimension.Start.Row;
					i <= workSheet.Dimension.End.Row;
					i++)
				{
					var user = new User
					{
						IsSelected = true,
						UserName = string.Empty,
						Alias = string.Empty
					};

					for (var j = workSheet.Dimension.Start.Column;
						j <= workSheet.Dimension.End.Column;
						j++)
					{
						var address = workSheet.Cells[i, j].Address;

						var cellValue = workSheet.Cells[i, j].Value;
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
			return listOfUsers;
		}

		public void ExportUsers(string filePath, List<User> users)
		{
			using (var package = GetExcelPackage(filePath))
			{
				var worksheet = package.Workbook.Worksheets.Add("Exported system fields");
				var lineNumber = 1;
				foreach (var user in users)
				{
					if (user != null)
					{
						worksheet.Cells["A" + lineNumber].Value = user.UserName;
						worksheet.Cells["B" + lineNumber].Value = user.Alias;
						lineNumber++;
					}
				}

				package.Save();
			}
		}

		public void ExportLogReportToExcel(string filePath, Model.Log.Report report)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}

			using (var package = GetExcelPackage(filePath))
			{
				var worksheet = package.Workbook.Worksheets.Add("Log Report");
				worksheet.View.ShowGridLines = false;
				var lineNumber = 1;
				
				AddLogReportHeaderItem(worksheet, "Report Path:", report.ReportFullPath, ref lineNumber);				
				AddLogReportHeaderItem(worksheet, "Report Type:", report.Type, ref lineNumber);				
				AddLogReportHeaderItem(worksheet, "Process Scope:", report.Scope, ref lineNumber);				
				AddLogReportHeaderItem(worksheet, "TM Path:", report.TmFile.Path, ref lineNumber);				
				AddLogReportHeaderItem(worksheet, "Server TM:", report.TmFile.IsServerTm, ref lineNumber);				
				AddLogReportHeaderItem(worksheet, "Created:", report.Created.ToString(CultureInfo.InvariantCulture), ref lineNumber);				
				AddLogReportHeaderItem(worksheet, "Updated Units:", report.UpdatedCount, ref lineNumber);			
												
				var table = AddLogReportDataTable(report, worksheet, ref lineNumber);

				package.Save();
			}
		}

		private static ExcelTable AddLogReportDataTable(Report report, ExcelWorksheet worksheet, ref int lineNumber)
		{
			lineNumber++;			

			var startData = lineNumber;

			worksheet.Cells["A" + lineNumber].Value = "ID";
			worksheet.Cells["B" + lineNumber].Value = "Name";
			worksheet.Cells["C" + lineNumber].Value = "Type";
			worksheet.Cells["D" + lineNumber].Value = "Value";
			worksheet.Cells["E" + lineNumber].Value = "Previous";
			worksheet.Cells["F" + lineNumber].Value = "Result";

			worksheet.Column(1).Width = 10;
			worksheet.Column(2).Width = 12;
			worksheet.Column(3).Width = 12;
			worksheet.Column(4).Width = 70;
			worksheet.Column(5).Width = 70;
			worksheet.Column(6).Width = 10;

			foreach (var action in report.Actions)
			{
				lineNumber++;
				if (action != null)
				{
					worksheet.Cells["A" + lineNumber].Value = action.TmId?.Id.ToString();
					worksheet.Cells["B" + lineNumber].Value = action.Name;
					worksheet.Cells["C" + lineNumber].Value = action.Type;
					worksheet.Cells["D" + lineNumber].Value = action.Value;
					worksheet.Cells["E" + lineNumber].Value = action.Previous;
					worksheet.Cells["F" + lineNumber].Value = string.IsNullOrEmpty(action.Result) ? "OK" : action.Result;

					worksheet.Cells["D" + lineNumber].Style.WrapText = true;
					worksheet.Cells["E" + lineNumber].Style.WrapText = true;
				}
			}
		
			var range = worksheet.Cells[startData, 1, lineNumber, 6];

			lineNumber++;
			var table = worksheet.Tables.Add(range, "tableData");
			return table;
		}

		private static void AddLogReportHeaderItem(ExcelWorksheet worksheet, string labelText, object value, ref int lineNumber)
		{
			worksheet.Cells["A" + lineNumber].Value = labelText;
			worksheet.Cells["A" + lineNumber].Style.Font.Bold = true;
			worksheet.Cells["C" + lineNumber].Value = value;
			worksheet.Cells["C" + lineNumber].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
			worksheet.Cells["A" + lineNumber + ":" + "B" + lineNumber].Merge = true;

			lineNumber++;
		}
	}
}
