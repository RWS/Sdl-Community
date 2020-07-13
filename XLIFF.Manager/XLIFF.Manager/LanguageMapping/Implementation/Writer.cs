using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.XLIFF.Manager.LanguageMapping.Model;
using Sdl.Community.XLIFF.Manager.LanguageMapping.OpenXml;

namespace Sdl.Community.XLIFF.Manager.LanguageMapping.Implementation
{
	internal class Writer
	{	
		internal bool WriteLanguages(List<MappedLanguage> mappedLanguages, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception(PluginResources.Warning_FilePathNull);
			}

			if (mappedLanguages == null || mappedLanguages.Count == 0)
			{
				throw new NullReferenceException(PluginResources.Warning_LanguagesEmpty);
			}			

			File.Delete(path);

			var excelDocument = new Excel();
			var spreadsheet = excelDocument.CreateWorkbook(path);
			excelDocument.AddBasicStyles(spreadsheet);
			var worksheet1 = excelDocument.AddWorksheet(spreadsheet, "Languages");

			var rowIndex = Convert.ToUInt32(1);

			// write the header row
			excelDocument.SetCellValue(spreadsheet, worksheet1, 1, rowIndex, "Code");
			excelDocument.SetCellValue(spreadsheet, worksheet1, 2, rowIndex, "Language");
			excelDocument.SetCellValue(spreadsheet, worksheet1, 3, rowIndex, "Region");
			excelDocument.SetCellValue(spreadsheet, worksheet1, 4, rowIndex, "Mapped Code");
			excelDocument.SetCellValue(spreadsheet, worksheet1, 5, rowIndex, "Custom Display Name");
			
			// write the language rows
			foreach (var langauge in mappedLanguages)
			{
				rowIndex++;

				excelDocument.SetCellValue(spreadsheet, worksheet1, 1, rowIndex, langauge.LanguageCode);
				excelDocument.SetCellValue(spreadsheet, worksheet1, 2, rowIndex, langauge.LanguageName);
				excelDocument.SetCellValue(spreadsheet, worksheet1, 3, rowIndex, langauge.LanguageRegion);
				excelDocument.SetCellValue(spreadsheet, worksheet1, 4, rowIndex, langauge.MappedCode);
				excelDocument.SetCellValue(spreadsheet, worksheet1, 5, rowIndex, langauge.CustomDisplayName);								
			}

			// set the column widths
			excelDocument.SetColumnWidth(worksheet1, 1, 20);
			excelDocument.SetColumnWidth(worksheet1, 2, 25);
			excelDocument.SetColumnWidth(worksheet1, 3, 35);
			excelDocument.SetColumnWidth(worksheet1, 4, 20);
			excelDocument.SetColumnWidth(worksheet1, 5, 35);

			// add a table filter
			var autoFilter = new AutoFilter { Reference = "A1:E" + mappedLanguages.Count + 1 };
			worksheet1.Append(autoFilter);
		
			worksheet1.Save();
			spreadsheet.Close();			

			return true;
		}
	}
}
