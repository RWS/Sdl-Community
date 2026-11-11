using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Multilingual.Excel.FileType.Providers.OpenXml;
using Multilingual.Excel.FileType.Providers.OpenXml.Model;
using Multilingual.Excel.FileType.Services;
using Xunit;

namespace Multilingual.Excel.FileType.Tests
{
	public class ExcelRichTextRoundTripTests : IDisposable
	{
		private const string SheetName = "Sheet1";
		private const uint BoldStyleIndex = 1;
		private readonly string _path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xlsx");

		public ExcelRichTextRoundTripTests()
		{
			CreateWorkbook(_path);
		}

		public void Dispose()
		{
			File.Delete(_path);
		}

		[Fact]
		public void Read_RichTextEnabled_ReturnsMarkupForContentColumnsOnly()
		{
			var row = ReadRow(processRichText: true);

			Assert.Equal("1<sup>st</sup> place", row.Cells[0].Value);
			Assert.Equal("context", row.Cells[2].Value);
		}

		[Fact]
		public void Read_RichTextDisabled_ReturnsPlainText()
		{
			Assert.Equal("1st place", ReadRow(processRichText: false).Cells[0].Value);
		}

		[Fact]
		public void WriteThenRead_FormattedTarget_RoundTripsAsRichText()
		{
			const string markup = "<b>Hello</b> <i>w<sup>2</sup></i>";
			Write(markup, BoldStyleIndex, processRichText: true);

			Assert.Equal(markup, ReadRow(processRichText: true).Cells[1].Value);
			Assert.Equal("Hello w2", ReadRow(processRichText: false).Cells[1].Value);

			using (var document = SpreadsheetDocument.Open(_path, false))
			{
				var cell = GetCell(document, "B2");
				Assert.Equal(CellValues.InlineString, cell.DataType.Value);
				// Every run gets explicit properties, so text outside tags keeps the cell font without its bold
				var runs = cell.InlineString.Elements<Run>().ToList();
				Assert.All(runs, run => Assert.NotNull(run.RunProperties));
				Assert.Null(runs[1].RunProperties.GetFirstChild<Bold>());
			}
		}

		[Fact]
		public void Write_RichTextDisabled_WritesTagsAsLiteralText()
		{
			Write("1<sup>st</sup>", 0, processRichText: false);

			Assert.Equal("1<sup>st</sup>", ReadRow(processRichText: true).Cells[1].Value);
		}

		[Fact]
		public void Write_PlainTextOverRichText_RemovesInlineString()
		{
			Write("<b>rich</b>", 0, processRichText: true);
			Write("plain", 0, processRichText: true);

			using (var document = SpreadsheetDocument.Open(_path, false))
			{
				var cell = GetCell(document, "B2");
				Assert.Null(cell.InlineString);
				Assert.Equal("plain", cell.CellValue.Text);
			}
		}

		private ExcelRow ReadRow(bool processRichText)
		{
			var options = new ExcelOptions { FirstRowIndex = 1, FirstRowIndexIsHeading = true, ProcessRichTextFormatting = processRichText };
			var columns = new List<ExcelColumn>
			{
				new ExcelColumn { Name = "A", IsContentColumn = true },
				new ExcelColumn { Name = "B", IsContentColumn = true },
				new ExcelColumn { Name = "C" }
			};

			return new ExcelReader(new ColorService()).GetExcelSheets(_path, options, columns).Single().Rows.Single();
		}

		private void Write(string targetValue, uint targetStyleIndex, bool processRichText)
		{
			using (var document = SpreadsheetDocument.Open(_path, true))
			{
				GetCell(document, "B2").StyleIndex = targetStyleIndex;
			}

			var column = new ExcelColumn { Name = "B", Index = 2, IsContentColumn = true };
			var row = new ExcelRow { Index = 2 };
			row.Cells.Add(new ExcelCell { Column = column, Value = targetValue });
			var sheet = new ExcelSheet { Name = SheetName, Rows = new List<ExcelRow> { row } };

			new ExcelWriter().UpdateExcelSheets(_path, new List<ExcelSheet> { sheet },
				new ExcelOptions { ProcessRichTextFormatting = processRichText }, new List<ExcelColumn> { column });
		}

		private static Cell GetCell(SpreadsheetDocument document, string reference)
		{
			return document.WorkbookPart.WorksheetParts.Single().Worksheet.Descendants<Cell>()
				.Single(cell => cell.CellReference == reference);
		}

		private static void CreateWorkbook(string path)
		{
			using (var document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
			{
				var workbookPart = document.AddWorkbookPart();
				workbookPart.Workbook = new Workbook(new Sheets());

				var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
				stylesPart.Stylesheet = new Stylesheet(
					new Fonts(
						new Font(new FontSize { Val = 11 }, new FontName { Val = "Arial" }),
						new Font(new Bold(), new FontSize { Val = 11 }, new FontName { Val = "Arial" })),
					new Fills(
						new Fill(new PatternFill { PatternType = PatternValues.None }),
						new Fill(new PatternFill { PatternType = PatternValues.Gray125 })),
					new Borders(new Border()),
					new CellFormats(
						new CellFormat { FontId = 0, FillId = 0, BorderId = 0 },
						new CellFormat { FontId = 1, FillId = 0, BorderId = 0, ApplyFont = true }));

				var sharedStringPart = workbookPart.AddNewPart<SharedStringTablePart>();
				sharedStringPart.SharedStringTable = new SharedStringTable(
					new SharedStringItem(
						new Run(new Text("1")),
						new Run(new RunProperties(new VerticalTextAlignment { Val = VerticalAlignmentRunValues.Superscript }), new Text("st")),
						new Run(new RunProperties(new FontSize { Val = 11 }), new Text(" place") { Space = SpaceProcessingModeValues.Preserve })),
					new SharedStringItem(
						new Run(new RunProperties(new Bold()), new Text("context"))));

				var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
				worksheetPart.Worksheet = new Worksheet(new SheetData(
					new Row(
						new Cell { CellReference = "A1", DataType = CellValues.String, CellValue = new CellValue("English") },
						new Cell { CellReference = "B1", DataType = CellValues.String, CellValue = new CellValue("German") },
						new Cell { CellReference = "C1", DataType = CellValues.String, CellValue = new CellValue("Context") })
					{ RowIndex = 1 },
					new Row(
						new Cell { CellReference = "A2", DataType = CellValues.SharedString, CellValue = new CellValue("0") },
						new Cell { CellReference = "B2" },
						new Cell { CellReference = "C2", DataType = CellValues.SharedString, CellValue = new CellValue("1") })
					{ RowIndex = 2 }));

				workbookPart.Workbook.Sheets.Append(new Sheet
				{
					Id = workbookPart.GetIdOfPart(worksheetPart),
					SheetId = 1,
					Name = SheetName
				});
			}
		}
	}
}
