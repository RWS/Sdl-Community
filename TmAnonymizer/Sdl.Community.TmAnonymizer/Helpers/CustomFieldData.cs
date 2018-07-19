using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Helpers
{
	public static class CustomFieldData
	{
		public static List<CustomField> GetImportedCustomFields(List<string> files)
		{
			var listOfCustomFields = new List<CustomField>();
			foreach (var file in files)
			{
				var package = ExcelFile.GetExcelPackage(file);
				var workSheet = package.Workbook.Worksheets[1];
				for (var i = workSheet.Dimension.Start.Row;
					i <= workSheet.Dimension.End.Row;
					i++)
				{
					var details = new Details() { Value = string.Empty, NewValue = string.Empty };
					var field = new CustomField()
					{
						IsSelected = true,
						Name = string.Empty,
						ValueType = FieldValueType.Unknown,
						Details = new System.Collections.ObjectModel.ObservableCollection<Details>(){ details }
					};
					for (var j = workSheet.Dimension.Start.Column;
						j <= workSheet.Dimension.End.Column;
						j++)
					{
						var address = workSheet.Cells[i, j].Address;

						var cellValue = workSheet.Cells[i, j].Value;

						foreach (var detail in field.Details)
						{
							if (address.Contains("A") && cellValue != null)
							{
								field.Name = cellValue.ToString();
							}
							if (address.Contains("B") && cellValue != null)
							{
								field.ValueType = (FieldValueType) Enum.Parse(typeof(FieldValueType), cellValue.ToString());
							}
							if (address.Contains("C") && cellValue != null)
							{
								detail.Value = cellValue.ToString();
							}
							if (address.Contains("D") && cellValue != null)
							{
								detail.NewValue = cellValue.ToString();
							}
						}
					}
					listOfCustomFields.Add(field);
				}
			}
			return listOfCustomFields;
		}

		public static void ExportCustomFields(string filePath, List<CustomField> customFields)
		{
			var package = ExcelFile.GetExcelPackage(filePath);
			var worksheet = package.Workbook.Worksheets.Add("Exported expressions");
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
