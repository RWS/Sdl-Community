using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging
{
    public class ExcelConverter
    {
        public static void ConvertHtmlTableToExcel(string htmlReport, string outputPath)
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

            // Create a new spreadsheet document
            using var spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // Create a worksheet part
            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Extract rows from the HTML table
            var rows = doc.DocumentNode.Descendants("tr")?.ToList();
            if (rows is null) return;

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            sheets.AppendChild(new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = GetFilename(rows)
            });

            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            rows = rows.Where((_, index) => index != 1).ToList();
            foreach (var row in rows)
            {
                var cells = row.Descendants("th").Concat(row.Descendants("td"));
                var excelRow = new Row();

                foreach (var cell in cells)
                {
                    var cellText = cell.InnerText.Trim();

                    cellText = HttpUtility.HtmlDecode(cellText);

                    var dataType = CellValues.String;
                    if (int.TryParse(cellText, out var number)) dataType = CellValues.Number;

                    var excelCell = new Cell()
                    {
                        DataType = dataType,
                        CellValue = new CellValue(cellText)
                    };
                    excelRow.AppendChild(excelCell);
                }

                sheetData.AppendChild(excelRow);
            }

            workbookPart.Workbook.Save();
        }
    }
}