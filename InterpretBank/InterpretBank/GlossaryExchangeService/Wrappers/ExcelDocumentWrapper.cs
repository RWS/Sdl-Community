using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using InterpretBank.GlossaryExchangeService.Wrappers.Interface;
using InterpretBank.GlossaryExchangeService.Wrappers.Model;

namespace InterpretBank.GlossaryExchangeService.Wrappers
{
	public class ExcelDocumentWrapper : ISpreadsheetDocumentWrapper
	{
		private SharedStringTable SharedStringTable { get; set; }

		public ExcelGlossaryEditor CreateSpreadsheet(string path)
		{
			var workbook = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
			workbook.AddWorkbookPart();
			var workSheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();

			CreateWorkbookElement(workbook, workSheetPart);

			var writer = OpenXmlWriter.Create(workSheetPart);
			writer.WriteStartElement(new Worksheet());
			writer.WriteStartElement(new SheetData());

			return new ExcelGlossaryEditor(workbook, writer);
		}

		public string GetCellText(string id) =>
					id == null ? null : SharedStringTable?.ChildElements[int.Parse(id)].InnerText;

		public IEnumerable<Row> GetRowsDom(string path)
		{
			using var spreadsheetDocument = SpreadsheetDocument.Open(path, false);

			var workbookPart = spreadsheetDocument.WorkbookPart;
			if (workbookPart == null) return default;

			SharedStringTable = workbookPart.SharedStringTablePart?.SharedStringTable;

			var worksheetPart = workbookPart.WorksheetParts.First();
			var sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

			return sheetData.Elements<Row>();
		}

		/// <summary>
		/// SAX Method.
		/// <br/> Slower but loads just one item at a time in memory.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<OpenXmlElement> GetRowsSax(string path)
		{
			using var spreadsheet = SpreadsheetDocument.Open(path, false);

			var workbookPart = spreadsheet.WorkbookPart;
			if (workbookPart == null) yield break;

			SharedStringTable = workbookPart.SharedStringTablePart?.SharedStringTable;

			using var reader = OpenXmlReader.Create(workbookPart.WorksheetParts.First());
			while (reader.Read())
			{
				if (reader.ElementType != typeof(Row)) continue;
				yield return reader.LoadCurrentElement();
			}
		}

		private void CreateWorkbookElement(SpreadsheetDocument workbook, WorksheetPart workSheetPart)
		{
			using var writer2 = OpenXmlWriter.Create(workbook.WorkbookPart);
			writer2.WriteStartElement(new Workbook());
			writer2.WriteStartElement(new Sheets());

			writer2.WriteElement(new Sheet()
			{
				Name = "Sheet1",
				SheetId = 1,
				Id = workbook.WorkbookPart.GetIdOfPart(workSheetPart)
			});

			writer2.WriteEndElement(); // Write end for WorkSheet Element
			writer2.WriteEndElement(); // Write end for WorkBook Element
		}
	}
}