using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using Sdl.Community.projectAnonymizer.Models;

namespace Sdl.Community.projectAnonymizer.Helpers
{
	public static class Expressions
	{
		public static void ExportExporessions(string  filePath,List<RegexPattern> patterns)
		{
			using (var file = File.CreateText(filePath))
			{
				var serializer = new JsonSerializer();
				serializer.Serialize(file, patterns);
			}
		}

		public static List<RegexPattern> GetImportedExpressions(List<string> files)
		{
			var patterns = new List<RegexPattern>();
			foreach (var file in files)
			{
				using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					using (var document = SpreadsheetDocument.Open(fileStream, false))
					{
						var workbookPart = document.WorkbookPart;
						var tablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
						var stringTable = tablePart.SharedStringTable;
						//need to loop maybe we'll have more sheets
						var worksheetPart = workbookPart.WorksheetParts.First();
						var sheet = worksheetPart.Worksheet;

						var cells = sheet.Descendants<Cell>().ToList();

						for (var i = 0; i < cells.Count; i = i + 2)
						{
							var pattern = new RegexPattern
							{
								Id = Guid.NewGuid().ToString(),
								ShouldEnable = true
							};
							var items = cells.Skip(i).Take(2);
							foreach (var cell in items)
							{
								if (cell.DataType != null && cell.DataType == CellValues.SharedString)
								{
									var celId = int.Parse(cell.CellValue.Text);
									var cellValue = stringTable.ChildElements[celId].InnerText;
									if (celId % 2 == 0)
									{
										pattern.Pattern = cellValue;
									}
									else
									{
										pattern.Description = cellValue;
									}
								}
							}
							patterns.Add(pattern);
						}
					}
				}
			}
			return patterns;
		}
	}
}
