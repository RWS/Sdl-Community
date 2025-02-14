using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging.Components
{
    public class ExcelConverter
    {
        public static void WriteExcelSpreadsheet(string htmlReport, string outputPath)
        {
            var table = ExtractTableWithId(htmlReport);
            SaveTableToExcel(table, outputPath);
        }

        private static string ExtractTableWithId(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);  // HtmlAgilityPack handles malformed HTML

            // Find the table containing a <th> element with the text "ID"
            var table = doc.DocumentNode.Descendants("table")
                .FirstOrDefault(t => t.Descendants("tr")
                    .Any(tr => tr.Descendants("th")
                        .Any(th => th.InnerText.Trim() == "ID")));

            // Return the outer HTML of the table, or null if not found
            return table?.OuterHtml;
        }

        // Updated GenerateStylesheet with a green header fill style (StyleIndex = 11)
        private static Stylesheet GenerateStylesheet()
        {
            var numberingFormats = new NumberingFormats();
            uint customPercentFormatId = 164;
            numberingFormats.Append(new NumberingFormat { NumberFormatId = customPercentFormatId, FormatCode = "0%" });

            var fills = new Fills(
                new Fill(new PatternFill { PatternType = PatternValues.None }),      // Fill index 0
                new Fill(new PatternFill { PatternType = PatternValues.Gray125 }),     // Fill index 1
                new Fill(new PatternFill                                         // Fill index 2: green header fill
                {
                    PatternType = PatternValues.Solid,
                    ForegroundColor = new ForegroundColor { Rgb = new HexBinaryValue { Value = "9bc6c7" } },
                    BackgroundColor = new BackgroundColor { Rgb = "9bc6c7" }
                })
            );

            var fonts = new Fonts(new Font());
            var borders = new Borders(new Border());
            var cellStyleFormats = new CellStyleFormats(new CellFormat());

            var cellFormats = new CellFormats();
            cellFormats.Append(new CellFormat()); // index 0: default
            for (int i = 1; i < 10; i++)
                cellFormats.Append(new CellFormat()); // reserve indices 1-9

            // index 10: percentage style (no decimals)
            cellFormats.Append(new CellFormat { NumberFormatId = customPercentFormatId, ApplyNumberFormat = true });

            // index 11: header style with green fill
            cellFormats.Append(new CellFormat { FillId = 2, ApplyFill = true });

            return new Stylesheet(numberingFormats, fonts, fills, borders, cellStyleFormats, cellFormats);
        }

        private static CellValues GetCellFormating(string cellText)
        {
            if (!cellText.Any(char.IsDigit)) return CellValues.String;

            if (!cellText.Contains('%'))
                return int.TryParse(cellText, out _) ? CellValues.Number : CellValues.String;

            return int.TryParse(cellText.TrimEnd('%'), out _) ? CellValues.Number : CellValues.String;
        }

        private static string GetFilename(List<HtmlNode> rows)
        {
            var firstRow = rows[1];
            var fileName =
                firstRow.Descendants("th").Concat(firstRow.Descendants("td")).FirstOrDefault()?.InnerText.Trim();
            return fileName?.Split('.')[0];
        }

        private static void SaveTableToExcel(string tableHtml, string filePath)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(tableHtml);

            using var spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add styles with percentage formatting (StyleIndex = 10)
            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = GenerateStylesheet();
            stylesPart.Stylesheet.Save();

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            sheets.AppendChild(new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = GetFilename(doc.DocumentNode.Descendants("tr")?.ToList())
            });

            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            var rows = doc.DocumentNode.Descendants("tr")?.ToList();
            if (rows is null) return;

            rows = rows.Where((_, index) => index != 1).ToList();
            foreach (var row in rows)
            {
                var cells = row.Descendants("th").Concat(row.Descendants("td"));
                var excelRow = new Row();

                foreach (var cell in cells)
                {
                    var cellText = HttpUtility.HtmlDecode(cell.InnerText.Trim());
                    var cellFormatting = GetCellFormating(cellText);
                    Cell excelCell;

                    if (cellFormatting == CellValues.Number && cellText.Contains('%'))
                    {
                        var percentage = double.Parse(cellText.TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture) / 100;
                        excelCell = new Cell
                        {
                            CellValue = new CellValue(percentage.ToString(CultureInfo.InvariantCulture)),
                            StyleIndex = 10,
                            DataType = CellValues.Number
                        };
                    }
                    else
                    {
                        excelCell = new Cell { CellValue = new CellValue(cellText) };
                        if (cellFormatting == CellValues.String)
                            excelCell.DataType = CellValues.String;
                    }

                    if (cell.Name.Equals("th", StringComparison.OrdinalIgnoreCase))
                        excelCell.StyleIndex = 11; // Apply header (green) style

                    excelRow.AppendChild(excelCell);
                }

                sheetData?.AppendChild(excelRow);
            }

            workbookPart.Workbook.Save();
        }
    }
}