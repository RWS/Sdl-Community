using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using InterpretBank.GlossaryExchangeService.Interface;
using InterpretBank.GlossaryExchangeService.Wrappers.Interface;

namespace InterpretBank.GlossaryExchangeService.ExchangeFormats
{
	public class ExcelImport : IImport
	{
		public ExcelImport(ISpreadsheetDocumentWrapper excelDocument, string path)
		{
			ExcelDocument = excelDocument;
			Path = path;
		}

		private ISpreadsheetDocumentWrapper ExcelDocument { get; }
		private string Path { get; }

		public IEnumerable<string[]> ImportTerms()
		{
			var rows = ExcelDocument.GetRowsSax(Path);

			foreach (var row in rows)
			{
				var cells = row?.ChildElements.Select(cell => (Cell)cell).ToList();

				var term = new string[cells.Count];
				for (var i = 0; i < cells.Count; i++)
				{
					var value = cells[i].CellValue?.InnerText;
					value = string.IsNullOrEmpty(value) ? null : value;

					term[i] = ExcelDocument.GetCellText(value) ?? value;
				}

				yield return term;
			}
		}
	}
}