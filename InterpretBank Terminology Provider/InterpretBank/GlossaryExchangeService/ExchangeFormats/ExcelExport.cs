using System.Collections.Generic;
using InterpretBank.GlossaryExchangeService.Interface;
using InterpretBank.GlossaryExchangeService.Wrappers.Interface;

namespace InterpretBank.GlossaryExchangeService.ExchangeFormats
{
	public class ExcelExport : IExport
	{
		public ExcelExport(ISpreadsheetDocumentWrapper excelDocument, string path)
		{
			ExcelDocument = excelDocument;
			Path = path;
		}

		private ISpreadsheetDocumentWrapper ExcelDocument { get; }
		private string Path { get; }

		public void ExportTerms(IEnumerable<string[]> terms, string glossaryName = null, string subGlossaryName = null)
		{
			using var spreadsheet = ExcelDocument.CreateSpreadsheet(Path);

			//for (var i = 1; i <= 500000; i++)
			//{
			//	spreadsheet.CreateRow(spreadsheet, i);

			//	for (var j = 1; j <= 30; ++j)
			//	{
			//		spreadsheet.CreateCellWithValue($"R{i}C{j}");
			//	}

			//	// this is for Row
			//	spreadsheet.WriteEndElement();
			//}

			var termIndex = 0;
			foreach (var term in terms)
			{
				termIndex++;
				spreadsheet.CreateRow(termIndex);

				for (var j = 1; j <= term.Length; ++j)
				{
					spreadsheet.CreateCellWithValue(term[j - 1]);
				}
			}
		}
	}
}