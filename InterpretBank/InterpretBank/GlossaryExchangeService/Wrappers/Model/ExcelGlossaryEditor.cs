using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace InterpretBank.GlossaryExchangeService.Wrappers.Model
{
	public class ExcelGlossaryEditor : IDisposable
	{
		private bool _isFirstRow = true;

		public ExcelGlossaryEditor(SpreadsheetDocument spreadsheetDocument, OpenXmlWriter openXmlWriter)
		{
			SpreadsheetDocument = spreadsheetDocument;
			OpenXmlWriter = openXmlWriter;
		}

		private bool IsFirstRow
		{
			get => _isFirstRow;
			set
			{
				if (_isFirstRow) _isFirstRow = value;
			}
		}

		private OpenXmlWriter OpenXmlWriter { get; }

		private SpreadsheetDocument SpreadsheetDocument { get; }

		/// <summary>
		/// Writes directly to cell, without using SharedStringTable
		/// </summary>
		/// <param name="term"></param>
		public void CreateCellWithValue(string term)
		{
			var attributeList2 = new List<OpenXmlAttribute> { new("t", null, "str") };
			OpenXmlWriter.WriteStartElement(new Cell(), attributeList2);
			OpenXmlWriter.WriteElement(new CellValue(term));
			WriteEndElement();
		}

		public void CreateRow(int termIndex)
		{
			if (!IsFirstRow)
			{
				WriteEndElement();
			}

			IsFirstRow = false;

			var attributeList = new List<OpenXmlAttribute>
			{
				// this is the row index
				new("r", null, termIndex.ToString())
			};

			OpenXmlWriter.WriteStartElement(new Row(), attributeList);
		}

		public void Dispose()
		{
			OpenXmlWriter.Dispose();
			SpreadsheetDocument.Dispose();
		}

		private void WriteEndElement() => OpenXmlWriter.WriteEndElement();
	}
}