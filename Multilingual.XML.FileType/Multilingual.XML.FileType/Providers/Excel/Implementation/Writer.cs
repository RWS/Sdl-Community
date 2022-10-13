using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using Multilingual.XML.FileType.Models;

namespace Multilingual.XML.FileType.Providers.Excel.Implementation
{
	internal class Writer
	{	
		internal bool WritePlaceholderPatterns(List<PlaceholderPattern> placeholderPatterns, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("File path cannot be null!");
			}

			if (placeholderPatterns == null || placeholderPatterns.Count == 0)
			{
				throw new NullReferenceException("Placeholders cannot be empty or null!");
			}			

			File.Delete(path);

			var excelDocument = new OpenXml.Excel();
			var spreadsheet = excelDocument.CreateWorkbook(path);
			excelDocument.AddBasicStyles(spreadsheet);
			var worksheet1 = excelDocument.AddWorksheet(spreadsheet, "Placeholders");

			var rowIndex = Convert.ToUInt32(1);

			// write the header row
			excelDocument.SetCellValue(spreadsheet, worksheet1, 1, rowIndex, "Pattern");
			excelDocument.SetCellValue(spreadsheet, worksheet1, 2, rowIndex, "Segmentation");
			excelDocument.SetCellValue(spreadsheet, worksheet1, 3, rowIndex, "Description");
			
			// write the language rows
			foreach (var langauge in placeholderPatterns)
			{
				rowIndex++;

				excelDocument.SetCellValue(spreadsheet, worksheet1, 1, rowIndex, langauge.Pattern);
				excelDocument.SetCellValue(spreadsheet, worksheet1, 2, rowIndex, langauge.SegmentationHint.ToString());
				excelDocument.SetCellValue(spreadsheet, worksheet1, 3, rowIndex, langauge.Description);
			}

			// set the column widths
			excelDocument.SetColumnWidth(worksheet1, 1, 30);
			excelDocument.SetColumnWidth(worksheet1, 2, 30);
			excelDocument.SetColumnWidth(worksheet1, 3, 100);

			// add a table filter
			var autoFilter = new AutoFilter { Reference = "A1:C" + placeholderPatterns.Count + 1 };
			worksheet1.Append(autoFilter);
		
			worksheet1.Save();
			spreadsheet.Close();			

			return true;
		}
	}
}
