using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.MTCloud.Languages.Provider.Model;
using Sdl.Community.MTCloud.Languages.Provider.OpenXml;

namespace Sdl.Community.MTCloud.Languages.Provider.Implementation
{
	internal class Writer
	{	
		internal bool WriteLanguages(List<MTCloudLanguage> langauges, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("File path cannot be null!");
			}

			if (langauges == null || langauges.Count == 0)
			{
				throw new NullReferenceException("Languages cannot be empty or null!");
			}			

			File.Delete(path);

			var excelDocument = new Excel();
			var spreadsheet = excelDocument.CreateWorkbook(path);
			excelDocument.AddBasicStyles(spreadsheet);
			var worksheet1 = excelDocument.AddWorksheet(spreadsheet, "Languages");

			var rowIndex = Convert.ToUInt32(1);

			// write the header row
			excelDocument.SetCellValue(spreadsheet, worksheet1, 1, rowIndex, "Language");
			excelDocument.SetCellValue(spreadsheet, worksheet1, 2, rowIndex, "Region");
			excelDocument.SetCellValue(spreadsheet, worksheet1, 3, rowIndex, "Trados Code");
			excelDocument.SetCellValue(spreadsheet, worksheet1, 4, rowIndex, "MT Code");
			excelDocument.SetCellValue(spreadsheet, worksheet1, 5, rowIndex, "MT Code (locale)");
			
			// write the language rows
			foreach (var langauge in langauges)
			{
				rowIndex++;

				excelDocument.SetCellValue(spreadsheet, worksheet1, 1, rowIndex, langauge.Language);
				excelDocument.SetCellValue(spreadsheet, worksheet1, 2, rowIndex, langauge.Region);
				excelDocument.SetCellValue(spreadsheet, worksheet1, 3, rowIndex, langauge.TradosCode);
				excelDocument.SetCellValue(spreadsheet, worksheet1, 4, rowIndex, langauge.MTCode);
				excelDocument.SetCellValue(spreadsheet, worksheet1, 5, rowIndex, langauge.MTCodeLocale);
			}

			// set the column widths
			excelDocument.SetColumnWidth(worksheet1, 1, 30);
			excelDocument.SetColumnWidth(worksheet1, 2, 30);
			excelDocument.SetColumnWidth(worksheet1, 3, 20);
			excelDocument.SetColumnWidth(worksheet1, 4, 20);
			excelDocument.SetColumnWidth(worksheet1, 5, 20);

			// add a table filter
			var autoFilter = new AutoFilter { Reference = "A1:E" + langauges.Count + 1 };
			worksheet1.Append(autoFilter);
		
			worksheet1.Save();
			spreadsheet.Close();			

			return true;
		}
	}
}
