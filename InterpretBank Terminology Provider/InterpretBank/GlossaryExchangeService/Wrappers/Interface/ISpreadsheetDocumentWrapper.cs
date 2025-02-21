using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using InterpretBank.GlossaryExchangeService.Wrappers.Model;

namespace InterpretBank.GlossaryExchangeService.Wrappers.Interface;

public interface ISpreadsheetDocumentWrapper
{
	ExcelGlossaryEditor CreateSpreadsheet(string path);

	string GetCellText(string id);

	IEnumerable<Row> GetRowsDom(string path);

	IEnumerable<OpenXmlElement> GetRowsSax(string path);
}