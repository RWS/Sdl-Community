using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Helpers
{
	public static class CustomFieldData
	{
		public static List<CustomField> GetImportedCustomFields(List<string> files)
		{
			var customFields = new List<CustomField>();
			foreach (var file in files)
			{
				var package = ExcelFile.GetExcelPackage(file);
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
							Details = new ObservableCollection<Details>()
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
							var details = new Details
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
								customExistingField.Details.Add(details);
								break;
							}
						}
					}
				}
			}
			return customFields;
		}

		public static void ExportCustomFields(string filePath, List<CustomField> customFields)
		{
			var package = ExcelFile.GetExcelPackage(filePath);
			var worksheet = package.Workbook.Worksheets.Add("Exported custom fields");
			var lineNumber = 1;
			foreach (var field in customFields)
			{
				if (field != null)
				{
					foreach (var detail in field.Details)
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
	}
}
