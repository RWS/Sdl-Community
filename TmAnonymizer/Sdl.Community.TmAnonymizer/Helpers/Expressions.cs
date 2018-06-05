using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.Helpers
{
	public static class Expressions
	{
		public static List<Rule> GetImportedExpressions(List<string> files)
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
		public static void ExportExporessions(string filePath, List<Rule> patterns)
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
		private static ExcelPackage GetExcelPackage(string filePath)
		{
			var fileInfo = new FileInfo(filePath);
			var excelPackage = new ExcelPackage(fileInfo);
			return excelPackage;
		}
	}
}
