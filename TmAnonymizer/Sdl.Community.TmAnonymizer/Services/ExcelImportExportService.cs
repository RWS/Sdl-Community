using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Sdl.Community.SdlTmAnonymizer.Model;
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
						var studioCustomFieldType = (FieldValueType) Enum.Parse(typeof(FieldValueType), fieldType);
						var field = new CustomField
						{
							IsSelected = true,
							Name = customFieldName,
							ValueType = studioCustomFieldType,
							FieldValues = new List<CustomFieldValue>()
						};
						customFields.Add(field);
					}
					for (var j = workSheet.Dimension.Start.Column+2;
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
			var package = GetExcelPackage(filePath);
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
			var package = GetExcelPackage(filePath);
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
}
