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
        public static void WriteExcelSpreadsheet(string htmlReport, string outputPath, string projectName)
        {
            var additionalInfo = ExtractAdditionalInfoFromHtml(htmlReport);
            var tables = ExtractTableWithId(htmlReport);
            SaveTableToExcel(projectName, additionalInfo, tables, outputPath);
        }

        private static (string Processed, string Compared, string Errors) ExtractAdditionalInfoFromHtml(string htmlReport)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlReport);
            var processedNode = doc.DocumentNode.SelectSingleNode("//tr[1]/td/span[2]/span[1]");
            var comparedNode = doc.DocumentNode.SelectSingleNode("//tr[1]/td/span[2]/span[3]");
            var errorsNode = doc.DocumentNode.SelectSingleNode("//tr[1]/td/span[2]/span[5]");

            return (
                processedNode != null ? HttpUtility.HtmlDecode(processedNode.InnerText.Trim()) : "Processed: 0",
                comparedNode != null ? HttpUtility.HtmlDecode(comparedNode.InnerText.Trim()) : "Compared: 0",
                errorsNode != null ? HttpUtility.HtmlDecode(errorsNode.InnerText.Trim()) : "Errors: 0"
            );
        }

        private static List<HtmlNode> ExtractTableWithId(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode.Descendants("table")
                .Where(t => t.Descendants("tr").FirstOrDefault()?.Descendants("th")
                    .Any(th => th.InnerText.Trim().Equals("ID", StringComparison.OrdinalIgnoreCase)) ?? false)
                .ToList();
        }

        private static void SaveTableToExcel(string projectName, (string Processed, string Compared, string Errors) additionalInfo, List<HtmlNode> tableNodes, string filePath)
        {
            using var package = new ExcelPackage();
            foreach (var tableNode in tableNodes)
            {
                var rows = tableNode.SelectNodes(".//tr");
                var headerRow = rows.FirstOrDefault();

                if (rows == null || rows.Count < 2)
                    continue;

                var sheetName = GenerateSheetName(rows[2]);
                var ws = package.Workbook.Worksheets.Add(sheetName);

                AddTitle(ws, projectName);
                AddAdditionalInfo(ws, additionalInfo);
                //HighlightRow(ws, 3, Color.FromArgb(220, 230, 241), true);

                var excelRow = 3;
                foreach (var row in rows)
                {
                    var cells = row.SelectNodes("td|th");
                    if (cells == null)
                    {
                        excelRow++;
                        continue;
                    }

                    if (excelRow == 3)
                    {
                        AddRowToWorksheet(ws, cells, excelRow, headerRow);
                        HighlightRow(ws, 3, Color.FromArgb(255, 155, 198, 199), true);
                    }
                    else if (excelRow == 4)
                    {
                        MergeAndStyleRow(ws, cells, excelRow, Color.FromArgb(255, 155, 198, 199));
                        HighlightRow(ws, excelRow, Color.FromArgb(230, 230, 230));
                    }
                    else
                    {
                        AddRowToWorksheet(ws, cells, excelRow, headerRow);
                    }

                    excelRow++;
                }

                AutoFitColumns(ws);
                ws.View.FreezePanes(4, 1);
            }

            package.SaveAs(new FileInfo(filePath));
        }

        private static string GenerateSheetName(HtmlNode row)
        {
            var dataFileIdAttribute = row.Attributes["data-file-id"];
            return dataFileIdAttribute != null ? dataFileIdAttribute.Value : "Unknown";
        }

        private static string FormatComments(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var severityNode = doc.DocumentNode.SelectSingleNode("//div[@style='white-space: nowrap; background-color: #DFDFFF; text-align: left; color: Black;margin-bottom: 1px;']/span[1]");
            var dateNode = doc.DocumentNode.SelectSingleNode("//div[@style='white-space: nowrap; background-color: #DFDFFF; text-align: left; color: Black;margin-bottom: 1px;']/span[2]");
            var authorNode = doc.DocumentNode.SelectSingleNode("//div[@style='white-space: nowrap; background-color: #DFDFFF; text-align: left; color: Black;margin-bottom: 1px;']/span[3]");
            var commentNode = doc.DocumentNode.SelectSingleNode("//p[@style='margin: 0px; padding: 3;']");

            var severity = severityNode != null ? HttpUtility.HtmlDecode(severityNode.InnerText.Trim()) : string.Empty;
            var date = dateNode != null ? HttpUtility.HtmlDecode(dateNode.InnerText.Trim()) : string.Empty;
            var author = authorNode != null ? HttpUtility.HtmlDecode(authorNode.InnerText.Trim()) : string.Empty;
            var comment = commentNode != null ? HttpUtility.HtmlDecode(commentNode.InnerText.Trim()) : string.Empty;

            return $"{comment}\n{severity}\n{date}\n{author}";
        }

        private static void AddTitle(ExcelWorksheet ws, string projectName)
        {
            ws.Cells[1, 1, 1, 11].Merge = true;
            ws.Cells[1, 1].Value = $"Post-Edit Comparison Report - {projectName}";
            ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Font.Size = 20;
            ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 230, 241));
            ws.Cells[1, 1].Style.Font.Color.SetColor(Color.Black);
            ws.Cells[1, 1].Style.WrapText = true;
            ws.Row(1).Height = 30;
        }

        private static void AddAdditionalInfo(ExcelWorksheet ws, (string Processed, string Compared, string Errors) additionalInfo)
        {
            var additionalInfoFormatted = $"{additionalInfo.Processed}, {additionalInfo.Compared}, {additionalInfo.Errors}";
            ws.Cells[2, 1, 2, 11].Merge = true;
            ws.Cells[2, 1].Value = additionalInfoFormatted;
            ws.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[2, 1].Style.Font.Italic = true;
            ws.Cells[2, 1].Style.Font.Size = 12;
            ws.Cells[2, 1].Style.Font.Color.SetColor(Color.Gray);
            ws.Cells[2, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[2, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 230, 241));
            ws.Cells[2, 1].Style.WrapText = true;
        }

        private static void HighlightRow(ExcelWorksheet ws, int row, Color color, bool bold = false)
        {
            foreach (var cell in ws.Cells[row, 1, row, 11])
            {
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(color);
                cell.Style.Font.Bold = bold;
            }
        }

        private static void AddRowToWorksheet(ExcelWorksheet ws, HtmlNodeCollection cells, int excelRow, HtmlNode headerRow)
        {
            var commentsColumnIndex = GetColumnIndex("Comments", headerRow);
            var statusColumnIndex = GetColumnIndex("Status", headerRow);
            var targetComparisonColumnIndex = GetColumnIndex("Target (Comparison)", headerRow);

            var excelCol = 1;
            foreach (var cell in cells)
            {
                var cellText = HttpUtility.HtmlDecode(cell.InnerText.Trim());
                var cellRef = ws.Cells[excelRow, excelCol];

                if (excelRow > 4 && excelCol == targetComparisonColumnIndex)
                    cellText = cell.InnerHtml;

                if (excelRow > 3 && excelCol == statusColumnIndex) cellText = ExtractStatusText(cell);

                if (excelRow > 4 && excelCol == commentsColumnIndex)
                    cellText = FormatComments(cell.InnerHtml);

                if (cellText.EndsWith("%") && double.TryParse(cellText.TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture, out double percentageValue))
                {
                    cellRef.Value = percentageValue / 100;
                    cellRef.Style.Numberformat.Format = "0.00%";
                }
                else if (double.TryParse(cellText, NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
                {
                    cellRef.Value = numericValue;
                    cellRef.Style.Numberformat.Format = (excelCol == 1 || excelCol == 5) ? "0" : "#,##0.00";
                }
                else
                    cellRef.Value = cellText;

                // Set border style and color
                cellRef.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cellRef.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cellRef.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cellRef.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cellRef.Style.Border.Top.Color.SetColor(Color.Black);
                cellRef.Style.Border.Bottom.Color.SetColor(Color.Black);
                cellRef.Style.Border.Left.Color.SetColor(Color.Black);
                cellRef.Style.Border.Right.Color.SetColor(Color.Black);

                if (excelRow == 1) StyleHeaderCell(cellRef);

                excelCol++;
            }
        }

        private static int GetColumnIndex(string columnName, HtmlNode headerRow) =>
            headerRow.SelectNodes("th").ToList().FindIndex(th =>
                th.InnerText.Trim().Equals(columnName, StringComparison.OrdinalIgnoreCase)) + 1;

        private static string ExtractStatusText(HtmlNode cell)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(cell.InnerHtml);
            var statusParts = doc.DocumentNode.SelectNodes("//span")?
                .Select(span => HttpUtility.HtmlDecode(span.InnerText.Trim()))
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToArray();
            return statusParts != null ? string.Join("\n", statusParts) : string.Empty;
        }

        private static void MergeAndStyleRow(ExcelWorksheet ws, HtmlNodeCollection cells, int excelRow, Color color)
        {
            ws.Cells[excelRow, 1, excelRow, 11].Merge = true;
            ws.Cells[excelRow, 1].Value = string.Join(" ", cells.Select(c => HttpUtility.HtmlDecode(c.InnerText.Trim())));
            ws.Cells[excelRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[excelRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[excelRow, 1].Style.Font.Bold = true;
            ws.Cells[excelRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[excelRow, 1].Style.Fill.BackgroundColor.SetColor(color);
            ws.Cells[excelRow, 1].Style.Font.Color.SetColor(Color.Black);
        }

        private static void StyleHeaderCell(ExcelRange cellRef)
        {
            cellRef.Style.Font.Bold = true;
            cellRef.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cellRef.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 155, 198, 199));
            cellRef.Style.Font.Color.SetColor(Color.White);
            cellRef.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private static void AutoFitColumns(ExcelWorksheet ws)
        {
            if (ws.Dimension != null)
            {
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                for (var col = 1; col <= ws.Dimension.End.Column; col++)
                {
                    if (ws.Column(col).Width > 20)
                    {
                        ws.Column(col).Width = 20;
                    }
                }
            }
        }
    }
}
