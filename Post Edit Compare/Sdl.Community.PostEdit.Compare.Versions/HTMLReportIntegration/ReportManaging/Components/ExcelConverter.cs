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
            var tables = ExtractTableWithId(htmlReport);
            SaveTableToExcel(tables, outputPath);
        }

        private static void AddRows(List<HtmlNode> rows, SheetData sheetData)
        {
            foreach (var (row, index) in rows.Select((row, index) => (row, index)))
            {
                var cells = row.Descendants("th").Concat(row.Descendants("td"));
                var excelRow = new Row();

                // Check if this is the second row (index 1)
                bool isSecondRow = index == 1;

                foreach (var cell in cells)
                {
                    var excelCell = CreateCell(cell, isSecondRow);

                    // Apply center alignment to the second row
                    if (isSecondRow)
                    {
                        excelCell.StyleIndex = 12; // Assuming 12 is the style index for centered text
                    }

                    excelRow.AppendChild(excelCell);
                }

                sheetData?.AppendChild(excelRow);

                // Apply column width for the second row to make it centered horizontally
                if (isSecondRow)
                {
                    var columns = sheetData.Elements<Row>().FirstOrDefault()?.Elements<Cell>().ToList();
                    if (columns != null)
                    {
                        foreach (var column in columns)
                        {
                            // Set column width for the second row (e.g., setting a fixed width of 15)
                            var columnWidth = new Column { Min = 1, Max = (uint)columns.Count, Width = 15, CustomWidth = true };
                            sheetData.InsertAt(new Columns(columnWidth), 0); // Insert at the beginning of the sheet
                        }
                    }
                }
            }
        }



        private static Cell CreateCell(HtmlNode cell, bool isSecondRow)
        {
            var cellText = HttpUtility.HtmlDecode(isSecondRow ? cell.InnerText.Replace("\n", "") : cell.InnerText.Trim());
            var cellFormatting = GetCellFormating(cellText);
            Cell excelCell;

            if (cellFormatting == CellValues.Number && cellText.Contains('%'))
            {
                var percentage = double.Parse(cellText.TrimEnd('%'), NumberStyles.Any,
                    CultureInfo.InvariantCulture) / 100;
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
            return excelCell;
        }

        private static List<string> ExtractTableWithId(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);  // HtmlAgilityPack handles malformed HTML

            // Find the table containing a <th> element with the text "ID"
            var tables = doc.DocumentNode.Descendants("table")
                .Where(t => t.Descendants("tr")
                    .Any(tr => tr.Descendants("th")
                        .Any(th => th.InnerText.Trim() == "ID")));

            var tablesHtml = tables.Select(t => t.OuterHtml).ToList();
            // Return the outer HTML of the table, or null if not found
            return tablesHtml;
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

            // index 12: centered text style (apply horizontal alignment)
            cellFormats.Append(new CellFormat
            {
                Alignment = new Alignment { Horizontal = HorizontalAlignmentValues.Center }
            });

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

        private static void SaveTableToExcel(List<string> tableHtmls, string filePath)
        {
            using var spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // Add styles with percentage formatting (StyleIndex = 10)
            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = GenerateStylesheet();
            stylesPart.Stylesheet.Save();

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            uint sheetId = 1; // Ensure each sheet gets a unique ID

            foreach (var tableHtml in tableHtmls)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(tableHtml);

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Ensure unique SheetId
                var sheet = new Sheet
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = sheetId++, // Increment SheetId for each sheet
                    Name = GetFilename(doc.DocumentNode.Descendants("tr")?.ToList())
                };

                sheets.Append(sheet); // Append the new sheet to the Sheets collection

                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                var rows = doc.DocumentNode.Descendants("tr")?.ToList();

                if (rows == null || !rows.Any()) continue; // Skip empty or missing rows

                AddRows(rows, sheetData); // Add rows to the sheet
            }

            workbookPart.Workbook.Save(); // Ensure workbook saves after all sheets are added
        }
    }
}