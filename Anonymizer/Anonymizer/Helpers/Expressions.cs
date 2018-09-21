using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using Sdl.Community.projectAnonymizer.Models;

namespace Sdl.Community.projectAnonymizer.Helpers
{
	public static class Expressions
	{
		public static void ExportExporessions(string filePath, List<RegexPattern> patterns)
		{
			var package = GetExcelPackage(filePath);
			var worksheet = package.Workbook.Worksheets.Add("Exported expressions");
			var lineNumber = 1;
			foreach (var pattern in patterns)
			{
				if (pattern != null)
				{
					worksheet.Cells["A" + lineNumber].Value = pattern.Pattern;
					worksheet.Cells["B" + lineNumber].Value = pattern.Description;
					lineNumber++;
				}
			}
			package.Save();
		}

		public static List<RegexPattern> GetImportedExpressions(List<string> files)
		{
			var patterns = new List<RegexPattern>();
			foreach (var file in files)
			{
				var package = GetExcelPackage(file);
				var workSheet = package.Workbook.Worksheets[1];
				for (var i = workSheet.Dimension.Start.Row;
					i <= workSheet.Dimension.End.Row;
					i++)
				{
					var pattern = new RegexPattern
					{
						Id = Guid.NewGuid().ToString(),
						ShouldEnable = true,
						Description = string.Empty,
						Pattern = string.Empty
					};
					for (var j = workSheet.Dimension.Start.Column;
						j <= workSheet.Dimension.End.Column;
						j++)
					{
						var address = workSheet.Cells[i, j].Address;

						var cellValue = workSheet.Cells[i, j].Value;
						if (address.Contains("A"))
						{
							pattern.Pattern = cellValue.ToString();
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

		private static ExcelPackage GetExcelPackage(string filePath)
		{
			var fileInfo = new FileInfo(filePath);
			var excelPackage = new ExcelPackage(fileInfo);
			return excelPackage;
		}
	}
}