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
        private static string ColumnIndexToLetter(int colIndex)
        {
            // Convert 1-based column index to Excel column letters
            string col = "";
            while (colIndex > 0)
            {
                int remainder = (colIndex - 1) % 26;
                col = (char)(65 + remainder) + col;
                colIndex = (colIndex - 1) / 26;
            }
            return col;
        }



        public static void WriteExcelSpreadsheet(string htmlReport, string outputPath)
        {
            var tables = ExtractTableWithId(htmlReport);
            SaveTableToExcel(tables, outputPath);
        }

        private static void AddRows(List<HtmlNode> rows, SheetData sheetData, WorksheetPart worksheetPart)
        {
            // Determine how many columns exist by checking the first row (header).
            var headerCells = rows[0].Descendants("th").Concat(rows[0].Descendants("td")).Count();
            string lastColRef = ColumnIndexToLetter(headerCells); // For merging (e.g., A2:F2)

            // Ensure <MergeCells> exists in the worksheet for merging
            var mergeCells = worksheetPart.Worksheet.GetFirstChild<MergeCells>();
            if (mergeCells == null)
            {
                mergeCells = new MergeCells();
                worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.GetFirstChild<SheetData>());
            }

            for (int index = 0; index < rows.Count; index++)
            {
                var rowNode = rows[index];
                var cells = rowNode.Descendants("th").Concat(rowNode.Descendants("td")).ToList();
                var excelRow = new Row();

                // If this is the second row (index == 1), merge across all columns
                if (index == 1 && headerCells > 1)
                {
                    // Combine all cell text into one
                    string mergedText = string.Join(" ", cells.Select(c => c.InnerText.Trim()));

                    // Create one wide cell
                    var mergedCell = CreateCellInternal(mergedText, true /* isSecondRow */, isMerged: true);
                    excelRow.AppendChild(mergedCell);

                    // Merge from A2 to [lastColRef]2
                    string mergeRef = $"A2:{lastColRef}2";
                    mergeCells.AppendChild(new MergeCell { Reference = mergeRef });
                }
                else
                {
                    // Normal row handling
                    bool isSecondRow = (index == 1);
                    foreach (var cell in cells)
                    {
                        var excelCell = CreateCell(cell, isSecondRow);
                        excelRow.AppendChild(excelCell);
                    }
                }

                sheetData.AppendChild(excelRow);
            }
        }





        private static Cell CreateCell(HtmlNode cell, bool isSecondRow)
        {
            // Original logic to handle text vs. percent
            string rawText = HttpUtility.HtmlDecode(cell.InnerText.Trim());
            bool isNumeric = double.TryParse(rawText.Replace("%", "").Trim(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out double numValue);

            // Build the new cell
            var excelCell = new Cell();

            // If numeric, store as number
            if (isNumeric && !rawText.Contains('%'))
            {
                excelCell.DataType = CellValues.Number;
                excelCell.CellValue = new CellValue(numValue.ToString(CultureInfo.InvariantCulture));
            }
            // If numeric with %, treat as fraction
            else if (isNumeric && rawText.Contains('%'))
            {
                double fraction = numValue / 100.0;
                excelCell.DataType = CellValues.Number;
                excelCell.CellValue = new CellValue(fraction.ToString(CultureInfo.InvariantCulture));
                excelCell.StyleIndex = 10; // e.g., custom percent style
            }
            else
            {
                // Default to string
                excelCell.DataType = CellValues.String;
                excelCell.CellValue = new CellValue(rawText);
            }

            // If it's a header cell (th), apply header style (e.g., index = 11)
            if (cell.Name.Equals("th", StringComparison.OrdinalIgnoreCase))
                excelCell.StyleIndex = 11;

            // If it's the second row (file name row), we want it centered
            // but only if not merged. Merged logic is handled in AddRows.
            if (isSecondRow && excelCell.StyleIndex == 0)
                excelCell.StyleIndex = 12; // Centered text style

            return excelCell;
        }
        private static Cell CreateCellInternal(string text, bool isSecondRow, bool isMerged)
        {
            var cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(text);

            // If second row, center horizontally
            if (isSecondRow)
                cell.StyleIndex = 12;

            // If merged, there's only one cell in that row, so no numeric detection needed
            return cell;
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

                AddRows(rows, sheetData, worksheetPart); // Add rows to the sheet
            }

            workbookPart.Workbook.Save(); // Ensure workbook saves after all sheets are added
        }
    }
}