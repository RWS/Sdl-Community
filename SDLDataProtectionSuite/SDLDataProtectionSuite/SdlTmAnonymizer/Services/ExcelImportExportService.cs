using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.Log;
using Sdl.LanguagePlatform.TranslationMemory;
using SegmentComparer.Structure;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services
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

				var colmun01 = workSheet.Cells[1, 1].Value;
				var column02 = workSheet.Cells[1, 2].Value;
				var column03 = workSheet.Cells[1, 3].Value;
				var column04 = workSheet.Cells[1, 4].Value;

				if (!IsValidColumnHeader(colmun01?.ToString(), "Name") |
					!IsValidColumnHeader(column02?.ToString(), "Type") |
					!IsValidColumnHeader(column03?.ToString(), "Value") |
					!IsValidColumnHeader(column04?.ToString(), "New Value"))
				{
					return null;
				}

				for (var i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
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
					for (var j = workSheet.Dimension.Start.Column + 2; j <= workSheet.Dimension.End.Column; j++)
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
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}

			var package = GetExcelPackage(filePath);
			var worksheet = package.Workbook.Worksheets.Add("Exported custom fields");
			var lineNumber = 1;

			worksheet.Cells["A" + lineNumber].Value = "Name";
			worksheet.Cells["B" + lineNumber].Value = "Type";
			worksheet.Cells["C" + lineNumber].Value = "Value";
			worksheet.Cells["D" + lineNumber].Value = "New Value";

			worksheet.Column(1).Width = 20;
			worksheet.Column(2).Width = 25;
			worksheet.Column(3).Width = 25;
			worksheet.Column(4).Width = 25;

			var startData = lineNumber;

			foreach (var field in customFields)
			{
				if (field != null)
				{
					foreach (var detail in field.FieldValues)
					{
						lineNumber++;
						worksheet.Cells["A" + lineNumber].Value = field.Name;
						worksheet.Cells["B" + lineNumber].Value = field.ValueType;
						worksheet.Cells["C" + lineNumber].Value = detail.Value;
						worksheet.Cells["D" + lineNumber].Value = detail.NewValue;

					}
				}
			}

			var range = worksheet.Cells[startData, 1, lineNumber, 4];
			var table = worksheet.Tables.Add(range, "tableData");

			package.Save();
		}

		public List<Rule> ImportedRules(List<string> files)
		{
			var rules = new List<Rule>();
			foreach (var file in files)
			{
				var package = GetExcelPackage(file);
				var workSheet = package.Workbook.Worksheets[1];

				var colmun01 = workSheet.Cells[1, 1].Value;
				var column02 = workSheet.Cells[1, 2].Value;
				var column03 = workSheet.Cells[1, 3].Value;
				var column04 = workSheet.Cells[1, 4].Value;

				if (!IsValidColumnHeader(colmun01?.ToString(), "ID") |
					!IsValidColumnHeader(column02?.ToString(), "Order") |
					!IsValidColumnHeader(column03?.ToString(), "Rule") |
					!IsValidColumnHeader(column04?.ToString(), "Description"))
				{
					return null;
				}

				for (var i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
				{
					var rule = new Rule
					{
						IsSelected = true
					};

					for (var j = workSheet.Dimension.Start.Column; j <= workSheet.Dimension.End.Column; j++)
					{
						var address = workSheet.Cells[i, j].Address;

						var cellValue = workSheet.Cells[i, j].Value;
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
			return rules;
		}

		public void ExportRules(string filePath, List<Rule> rules)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}

			using (var package = GetExcelPackage(filePath))
			{
				var worksheet = package.Workbook.Worksheets.Add("Exported expressions");
				var lineNumber = 1;

				worksheet.Cells["A" + lineNumber].Value = "ID";
				worksheet.Cells["B" + lineNumber].Value = "Order";
				worksheet.Cells["C" + lineNumber].Value = "Rule";
				worksheet.Cells["D" + lineNumber].Value = "Description";

				worksheet.Column(1).Width = 10;
				worksheet.Column(2).Width = 10;
				worksheet.Column(3).Width = 50;
				worksheet.Column(4).Width = 35;

				var startData = lineNumber;

				foreach (var rule in rules.OrderBy(a => a.Order))
				{
					lineNumber++;
					worksheet.Cells["A" + lineNumber].Value = rule.Id;
					worksheet.Cells["B" + lineNumber].Value = rule.Order;
					worksheet.Cells["C" + lineNumber].Value = rule.Name;
					worksheet.Cells["D" + lineNumber].Value = rule.Description;
				}

				var range = worksheet.Cells[startData, 1, lineNumber, 4];
				var table = worksheet.Tables.Add(range, "tableData");

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

				var colmun01 = workSheet.Cells[1, 1].Value;
				var column02 = workSheet.Cells[1, 2].Value;

				if (!IsValidColumnHeader(colmun01?.ToString(), "User Name") |
					!IsValidColumnHeader(column02?.ToString(), "New Value"))
				{
					return null;
				}

				for (var i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
				{
					var user = new User
					{
						IsSelected = true,
						UserName = string.Empty,
						Alias = string.Empty
					};

					for (var j = workSheet.Dimension.Start.Column; j <= workSheet.Dimension.End.Column; j++)
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
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}

			using (var package = GetExcelPackage(filePath))
			{
				var worksheet = package.Workbook.Worksheets.Add("Exported system fields");
				var lineNumber = 1;

				worksheet.Cells["A" + lineNumber].Value = "User Name";
				worksheet.Cells["B" + lineNumber].Value = "New Value";

				worksheet.Column(1).Width = 30;
				worksheet.Column(2).Width = 30;

				var startData = lineNumber;
				foreach (var user in users)
				{
					if (user != null)
					{
						lineNumber++;

						worksheet.Cells["A" + lineNumber].Value = user.UserName;
						worksheet.Cells["B" + lineNumber].Value = user.Alias;
					}
				}

				var range = worksheet.Cells[startData, 1, lineNumber, 2];
				var table = worksheet.Tables.Add(range, "tableData");

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
			worksheet.Cells["D" + lineNumber].Value = "Original Value";
			worksheet.Cells["E" + lineNumber].Value = "New Value";
			worksheet.Cells["F" + lineNumber].Value = "Result";

			worksheet.Column(1).Width = 10;
			worksheet.Column(2).Width = 12;
			worksheet.Column(3).Width = 12;
			worksheet.Column(4).Width = 70;
			worksheet.Column(5).Width = 70;
			worksheet.Column(6).Width = 10;

			var comparer = new SegmentComparer.Comparer();
			foreach (var action in report.Actions)
			{
				lineNumber++;
				if (action != null)
				{
					worksheet.Cells["A" + lineNumber].Value = action.TmId?.Id.ToString();
					worksheet.Cells["B" + lineNumber].Value = action.Name;
					worksheet.Cells["C" + lineNumber].Value = action.Type;

					var comparison = comparer.CompareSegment(action.TmId?.Id.ToString(), action.Previous, action.Value, true, 1);

					var previousValue = worksheet.Cells["D" + lineNumber];
					previousValue.IsRichText = true;

					var currentValue = worksheet.Cells["E" + lineNumber];
					currentValue.IsRichText = true;

					foreach (var unit in comparison.ComparisonUnits)
					{
						switch (unit.Type)
						{
							case ComparisonUnit.ComparisonType.New:
								{
									if (unit.TextType != ComparisonUnit.ContentType.Tag)
									{
										var valueNormalText = currentValue.RichText.Add(unit.Text);
										valueNormalText.Color = System.Drawing.Color.Black;
										valueNormalText.Bold = false;
									}
									else
									{
										var valueNewText = currentValue.RichText.Add(unit.Text);
										valueNewText.Color = System.Drawing.Color.Red;
										valueNewText.Bold = true;
									}
									break;
								}
							case ComparisonUnit.ComparisonType.Removed:
								{
									var previousRemovedText = previousValue.RichText.Add(unit.Text);
									previousRemovedText.Color = System.Drawing.Color.Red;
									previousRemovedText.Bold = true;
									break;
								}
							case ComparisonUnit.ComparisonType.Identical:
							case ComparisonUnit.ComparisonType.None:
								{
									var valueNormalText = currentValue.RichText.Add(unit.Text);
									valueNormalText.Color = System.Drawing.Color.Black;
									valueNormalText.Bold = false;

									var previousNormalText = previousValue.RichText.Add(unit.Text);
									previousNormalText.Color = System.Drawing.Color.Black;
									previousNormalText.Bold = false;
									break;
								}
						}
					}

					worksheet.Cells["F" + lineNumber].Value = string.IsNullOrEmpty(action.Result) ? "OK" : action.Result;

					worksheet.Cells["D" + lineNumber].Style.WrapText = true;
					worksheet.Cells["E" + lineNumber].Style.WrapText = true;
				}
			}

			var range = worksheet.Cells[startData, 1, lineNumber, 6];
			var table = worksheet.Tables.Add(range, "tableData");

			lineNumber++;
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
