using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using OfficeOpenXml;
using Sdl.Community.projectAnonymizer.Models;

namespace Sdl.Community.projectAnonymizer.Helpers
{
	public static class Expressions
	{
		public static void ExportExporessions(string filePath, List<RegexPattern> patterns)
		{
			var package = GetExcelPackage(filePath, true);
			var worksheet = package.Workbook.Worksheets.Add("Exported expressions");
			var lineNumber = 1;
			var order = 0;

			worksheet.Cells["A" + lineNumber].Value = "ID";
			worksheet.Cells["B" + lineNumber].Value = "Order";
			worksheet.Cells["C" + lineNumber].Value = "Rule";
			worksheet.Cells["D" + lineNumber].Value = "Description";

			for (int i = patterns.Count - 1; i >= 0; i--)
			{
				if (patterns[i] != null)
				{
					lineNumber++;
					worksheet.Cells["A" + lineNumber].Value = patterns[i].Id;
					worksheet.Cells["B" + lineNumber].Value = order++;
					worksheet.Cells["C" + lineNumber].Value = patterns[i].Pattern;
					worksheet.Cells["D" + lineNumber].Value = patterns[i].Description;
				}
			}

			worksheet.Column(1).Width = 10;
			worksheet.Column(2).Width = 10;
			worksheet.Column(3).Width = 50;
			worksheet.Column(4).Width = 35;

			var range = worksheet.Cells[1, 1, lineNumber, 4];
			var table = worksheet.Tables.Add(range, "tableData");

			package.Save();
		}

		public static List<RegexPattern> GetImportedExpressions(List<string> files)
		{
			var patterns = new List<RegexPattern>();
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
					var pattern = new RegexPattern
					{
						ShouldEnable = true,
						Description = string.Empty,
						Pattern = string.Empty
					};

					for (var j = workSheet.Dimension.Start.Column; j <= workSheet.Dimension.End.Column; j++)
					{
						var address = workSheet.Cells[i, j].Address;

						var cellValue = workSheet.Cells[i, j].Value;
						if (address.Contains("A"))
						{
							pattern.Id = cellValue?.ToString() ?? string.Empty;
						}
						else if (address.Contains("C"))
						{
							pattern.Pattern = cellValue?.ToString() ?? string.Empty;
						}
						else if (address.Contains("D"))
						{
							pattern.Description = cellValue?.ToString() ?? string.Empty;
						}
					}
					patterns.Add(pattern);
				}
			}
			return patterns;
		}

		private static bool IsValidColumnHeader(string colmun, string expected)
		{
			if (string.Compare(colmun, expected, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				MessageBox.Show("Invalid import format, found '" + colmun + "', expected column header name '" + expected + "'", "SDLTM Anonymizer");

				return false;
			}

			return true;
		}

		private static ExcelPackage GetExcelPackage(string filePath, bool isExport = false)
		{
			if (isExport)
			{
				CreateExcelWithPatterns(ref filePath);
			}

			var fileInfo = new FileInfo(filePath);
			var excelPackage = new ExcelPackage(fileInfo);
			return excelPackage;
		}

		private static void CreateExcelWithPatterns(ref string filePath)
		{
			try
			{
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}
			catch (Exception e)
			{
				Console.Write(e);
				filePath = filePath.Insert(filePath.IndexOf(".xlsx"), "(new)");
				CreateExcelWithPatterns(ref filePath);
			}
		}
	}
}