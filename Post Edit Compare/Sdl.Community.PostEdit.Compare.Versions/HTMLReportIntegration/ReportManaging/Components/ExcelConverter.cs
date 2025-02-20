using OfficeOpenXml;
using OfficeOpenXml.Style;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging.Components
{
    public class ExcelConverter
    {
        public static void WriteExcelSpreadsheet(string htmlReport, string outputPath)
        {
            var tables = ExtractTableWithId(htmlReport);
            SaveTableToExcel(tables, outputPath);
        }

        private static List<HtmlNode> ExtractTableWithId(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) throw new ArgumentNullException("HTML cannot be null. Please select a report");
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var tables = doc.DocumentNode.Descendants("table")
                .Where(t => t.Descendants("tr").FirstOrDefault()?.Descendants("th")
                    .Any(th => th.InnerText.Trim().Equals("ID", StringComparison.OrdinalIgnoreCase)) ?? false)
                .ToList();

            if (!tables.Any()) throw new ArgumentNullException("HTML cannot be null. Please select a report");
            return tables;
        }

        private static void SaveTableToExcel(List<HtmlNode> tableNodes, string filePath)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                foreach (var tableNode in tableNodes)
                {
                    var rows = tableNode.SelectNodes(".//tr");
                    if (rows == null || rows.Count < 2)
                        continue;

                    var secondRowCells = rows[1].SelectNodes("td|th");
                    string sheetName = string.Join(" ",
                        secondRowCells.Select(c => HttpUtility.HtmlDecode(c.InnerText.Trim())));

                    var ws = package.Workbook.Worksheets.Add(sheetName);

                    int excelRow = 1;

                    foreach (var row in rows)
                    {
                        var cells = row.SelectNodes("td|th");
                        if (cells == null)
                        {
                            excelRow++;
                            continue;
                        }

                        if (excelRow == 2)
                        {
                            MergeAndStyleSecondRow(ws, cells, excelRow);
                        }
                        else
                        {
                            AddRowToWorksheet(ws, cells, excelRow);
                        }

                        excelRow++;
                    }

                    AutoFitColumns(ws);
                    ws.View.FreezePanes(2, 1); // Freeze the top row
                }

                package.SaveAs(new FileInfo(filePath));
            }
        }

        private static void AddRowToWorksheet(ExcelWorksheet ws, HtmlNodeCollection cells, int excelRow)
        {
            int excelCol = 1;
            foreach (var cell in cells)
            {
                var cellText = HttpUtility.HtmlDecode(cell.InnerText.Trim());
                var cellRef = ws.Cells[excelRow, excelCol];

                if (cellText.EndsWith("%") && double.TryParse(cellText.TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture, out double percentageValue))
                {
                    cellRef.Value = percentageValue / 100;
                    cellRef.Style.Numberformat.Format = "0.00%";
                }
                else if (double.TryParse(cellText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
                {
                    cellRef.Value = numericValue;
                    cellRef.Style.Numberformat.Format = "#,##0.00";
                }
                else
                {
                    cellRef.Value = cellText;
                }

                // Style first row as header
                if (excelRow == 1)
                {
                    StyleHeaderCell(cellRef);
                }

                excelCol++;
            }
        }

        private static void MergeAndStyleSecondRow(ExcelWorksheet ws, HtmlNodeCollection cells, int excelRow)
        {
            ws.Cells[excelRow, 1, excelRow, 11].Merge = true;
            ws.Cells[excelRow, 1].Value = string.Join(" ", cells.Select(c => HttpUtility.HtmlDecode(c.InnerText.Trim()))); // Join the content
            ws.Cells[excelRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[excelRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[excelRow, 1].Style.Font.Bold = true;
            ws.Cells[excelRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[excelRow, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 155, 198, 199)); // Green
            ws.Cells[excelRow, 1].Style.Font.Color.SetColor(Color.Black); // Change font color to black
        }

        private static void StyleHeaderCell(ExcelRange cellRef)
        {
            cellRef.Style.Font.Bold = true;
            cellRef.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cellRef.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 155, 198, 199)); // Green
            cellRef.Style.Font.Color.SetColor(Color.White);
            cellRef.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private static void AutoFitColumns(ExcelWorksheet ws)
        {
            if (ws.Dimension != null)
            {
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                for (int col = 1; col <= ws.Dimension.End.Column; col++)
                {
                    if (ws.Column(col).Width > 20) // Set a maximum width for columns
                    {
                        ws.Column(col).Width = 20;
                    }
                }
            }
        }
    }
}
