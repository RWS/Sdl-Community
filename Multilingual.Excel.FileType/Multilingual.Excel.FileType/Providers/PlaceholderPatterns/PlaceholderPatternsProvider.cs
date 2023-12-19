using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using Multilingual.Excel.FileType.Models;
using Multilingual.Excel.FileType.Providers.OpenXml;
using Multilingual.Excel.FileType.Providers.OpenXml.Model;
using Multilingual.Excel.FileType.Providers.PlaceholderPatterns.Interfaces;
using Multilingual.Excel.FileType.Services;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Providers.PlaceholderPatterns
{
	public class PlaceholderPatternsProvider : IPlaceholderPatternsProvider
	{
		public List<PlaceholderPattern> GetPlaceholderPatterns(string path, bool reset = false)
		{
			var common = new Common();
			if (reset || !File.Exists(path))
			{
				common.SaveDefaultPlaceholderPatternsDocument(path);
			}
			
			var excelOptions = new ExcelOptions
			{
				FirstRowIndex = 1,
				FirstRowIndexIsHeading = true,
				ReadAllWorkSheets = false,
				ReadHyperlinks = false,
			};

			var excelColumns = common.GetDefaultExcelColumns();
			var colorService = new ColorService();
			var reader = new ExcelReader(colorService);
			var excelSheets = reader.GetExcelSheets(path, excelOptions, excelColumns);
			var patterns = GetPlaceholderPatterns(excelSheets?.FirstOrDefault()?.Rows);

			return patterns;
		}

		
		private  List<PlaceholderPattern> GetPlaceholderPatterns(IEnumerable<ExcelRow> excelRows)
		{
			var patterns = new List<PlaceholderPattern>();
			foreach (var row in excelRows)
			{
				var pattern = new PlaceholderPattern
				{
					Order = (int)row.Index
				};

				foreach (var cell in row.Cells)
				{
					switch (cell.Column.Name)
					{
						case "A":
							pattern.Pattern = cell.Value;
							break;
						case "B":
							if (string.IsNullOrEmpty(cell.Value))
							{
								pattern.SegmentationHint = SegmentationHint.MayExclude;
							}
							else
							{
								var success =
									Enum.TryParse<SegmentationHint>(cell.Value, true, out var segmentationHint);
								pattern.SegmentationHint = success ? segmentationHint : SegmentationHint.MayExclude;
							}

							break;
						case "C":
							pattern.Description = cell.Value;
							break;
					}
				}

				patterns.Add(pattern);
			}

			return patterns;
		}

		public bool SavePlaceholderPatterns(List<PlaceholderPattern> placeholderPatterns, string path)
		{

			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException(nameof(path), "file path cannot be null!");
			}

			if (placeholderPatterns == null || placeholderPatterns.Count == 0)
			{
				throw new NullReferenceException("Placeholders cannot be empty or null!");
			}

			if (File.Exists(path))
			{
				File.Delete(path);
			}

			var excelDocument = new OpenXml.Excel();
			var spreadsheet = excelDocument.CreateWorkbook(path);
			excelDocument.AddBasicStyles(spreadsheet);
			var worksheet1 = excelDocument.AddWorksheet(spreadsheet, Constants.SheetNamePlaceholders);

			var rowIndex = Convert.ToUInt32(1);

			// write the header row
			excelDocument.SetCellValue(spreadsheet, worksheet1, 1, rowIndex, Constants.ColumnNamePattern);
			excelDocument.SetCellValue(spreadsheet, worksheet1, 2, rowIndex, Constants.ColumnNameSegmentationHint);
			excelDocument.SetCellValue(spreadsheet, worksheet1, 3, rowIndex, Constants.ColumnNameDescription);

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