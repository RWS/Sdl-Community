using HtmlAgilityPack;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

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

        private static void AddAdditionalInfo(ExcelWorksheet ws, (string Processed, string Compared, string Errors) additionalInfo, int columnCount)
        {
            var additionalInfoFormatted = $"{additionalInfo.Processed}, {additionalInfo.Compared}, {additionalInfo.Errors}";
            ws.Cells[2, 1, 2, columnCount].Merge = true;
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

        private static void AddRowToWorksheet(ExcelWorksheet ws, HtmlNodeCollection cells, int excelRow, HtmlNode headerRow)
        {
            var commentsColumnIndex = GetColumnIndex("Comments", headerRow);
            var statusColumnIndex = GetColumnIndex("Status", headerRow);
            var targetComparisonColumnIndex = GetColumnIndex("Target (Comparison)", headerRow);
            var tuSourceColumnIndex = GetColumnIndex("TU", headerRow);

            var excelCol = 1;
            foreach (var cell in cells)
            {
                var cellText = HttpUtility.HtmlDecode(cell.InnerText.Trim());
                var cellRef = ws.Cells[excelRow, excelCol];

                if (excelRow > 4 && excelCol == tuSourceColumnIndex || excelCol == targetComparisonColumnIndex)
                    ApplyRichTextFormatting(cellRef, cell.InnerHtml);
                else
                {
                    if (excelRow > 3 && excelCol == statusColumnIndex)
                        cellRef.Value = ExtractStatusText(cell);
                    else if (excelRow > 4 && excelCol == commentsColumnIndex)
                        ApplyFormattedComments(cellRef, cell.InnerHtml);
                    else if (TryParsePercentage(cellText, out double percentage))
                        SetPercentage(cellRef, percentage);
                    else if (TryParseNumeric(cellText, out double numericValue))
                        SetNumeric(cellRef, numericValue, excelCol);
                    else
                        cellRef.Value = cellText;
                }

                ApplyBorders(cellRef);

                if (excelRow == 1)
                    StyleHeaderCell(cellRef);

                excelCol++;
            }
        }

        private static void AddTitle(ExcelWorksheet ws, string projectName, int columnCount)
        {
            ws.Cells[1, 1, 1, columnCount].Merge = true;
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

        private static void ApplyBorders(ExcelRange cellRef)
        {
            cellRef.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cellRef.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cellRef.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cellRef.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cellRef.Style.Border.Top.Color.SetColor(Color.Black);
            cellRef.Style.Border.Bottom.Color.SetColor(Color.Black);
            cellRef.Style.Border.Left.Color.SetColor(Color.Black);
            cellRef.Style.Border.Right.Color.SetColor(Color.Black);
        }

        private static void ApplyFormattedComments(ExcelRange cellRef, string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var divNodes = doc.DocumentNode.SelectNodes("//div[starts-with(@style, 'border-style: solid')]");
            if (divNodes is null) return;

            cellRef.Value = null;
            cellRef.RichText.Clear();

            foreach (var divNode in divNodes)
            {
                var severityNode = divNode.SelectSingleNode(".//span[1]");
                var dateNode = divNode.SelectSingleNode(".//span[2]");
                var authorNode = divNode.SelectSingleNode(".//span[3]");
                var commentNode = divNode.SelectSingleNode(".//p[@style='margin: 0px; padding: 3;']");

                // Add comment text (first line)
                if (commentNode != null)
                {
                    var commentText = HttpUtility.HtmlDecode(commentNode.InnerText.Trim());
                    var rt = cellRef.RichText.Add(commentText);
                    rt.Color = Color.Black;
                    rt.Bold = true;
                }

                cellRef.RichText.Add("\n");

                // Add severity (second line) with inline styling
                if (severityNode != null)
                {
                    var severityText = HttpUtility.HtmlDecode(severityNode.InnerText.Trim());
                    var rt = cellRef.RichText.Add(severityText);
                    var style = severityNode.GetAttributeValue("style", "").ToLower();
                    rt.Color = style.Contains("color: red") ? Color.Red : Color.Black;
                    //rt.Bold = style.Contains("font-weight: bold");
                    rt.Bold = false;
                }

                cellRef.RichText.Add("\n");

                // Add date (third line) with italic styling if specified
                if (dateNode != null)
                {
                    var dateText = HttpUtility.HtmlDecode(dateNode.InnerText.Trim());
                    var rt = cellRef.RichText.Add(dateText);
                    if (dateNode.GetAttributeValue("style", "").ToLower().Contains("font-style: italic"))
                        rt.Italic = true;
                    rt.Color = Color.Black;
                }

                cellRef.RichText.Add("\n");

                // Add author (fourth line)
                if (authorNode != null && authorNode.InnerText.Trim() != "null")
                {
                    var authorText = HttpUtility.HtmlDecode(authorNode.InnerText.Trim());
                    var rt = cellRef.RichText.Add(authorText);
                    rt.Color = Color.Black;
                }

                cellRef.RichText.Add("\n");

                // Add a line between comments
                var separator = new string('-', 30);  // You can adjust the length as needed
                var line = cellRef.RichText.Add(separator);
                line.Color = Color.Gray;
                cellRef.RichText.Add("\n");
            }
        }

        private static void ApplyRichTextFormatting(ExcelRange cellRef, string htmlContent)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            cellRef.Value = null; // Clear default value
            cellRef.RichText.Clear(); // Clear any existing rich text formatting

            ProcessHtmlNodes(doc.DocumentNode, cellRef);
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

        private static (string Processed, string Compared, string Errors) ExtractAdditionalInfoFromHtml(string htmlReport)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlReport);
            return (
                GetNodeText(doc, "//tr[1]/td/span[2]/span[1]", "Processed: 0"),
                GetNodeText(doc, "//tr[1]/td/span[2]/span[3]", "Compared: 0"),
                GetNodeText(doc, "//tr[1]/td/span[2]/span[5]", "Errors: 0")
            );
        }

        private static string ExtractStatusText(HtmlNode cell)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(cell.InnerHtml);
            var statusParts = doc.DocumentNode.SelectNodes("//span")?
                .Select(span => HttpUtility.HtmlDecode(span.InnerText.Trim()))
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToArray();
            return statusParts != null ? string.Join(" \n", statusParts) : string.Empty;
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

        private static string GenerateSheetName(HtmlNode row)
        {
            var dataFileIdAttribute = row.Attributes["data-file-id"];
            return dataFileIdAttribute != null ? dataFileIdAttribute.Value : "Unknown";
        }

        private static int GetColumnIndex(string columnName, HtmlNode headerRow) =>
            headerRow.SelectNodes("th").ToList().FindIndex(th =>
                th.InnerText.Trim().Equals(columnName, StringComparison.OrdinalIgnoreCase)) + 1;

        private static string GetNodeText(HtmlDocument doc, string xpath, string defaultValue)
        {
            var node = doc.DocumentNode.SelectSingleNode(xpath);
            return node != null ? HttpUtility.HtmlDecode(node.InnerText.Trim()) : defaultValue;
        }

        private static void HighlightRow(ExcelWorksheet ws, int row, Color color, bool bold, int columnCount)
        {
            foreach (var cell in ws.Cells[row, 1, row, columnCount])
            {
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(color);
                cell.Style.Font.Bold = bold;
            }
        }

        private static void MergeAndStyleRow(ExcelWorksheet ws, HtmlNodeCollection cells, int excelRow, Color color, int columnCount)
        {
            ws.Cells[excelRow, 1, excelRow, columnCount].Merge = true;
            ws.Cells[excelRow, 1].Value = string.Join(" ", cells.Select(c => HttpUtility.HtmlDecode(c.InnerText.Trim())));
            ws.Cells[excelRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[excelRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[excelRow, 1].Style.Font.Bold = true;
            ws.Cells[excelRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[excelRow, 1].Style.Fill.BackgroundColor.SetColor(color);
            ws.Cells[excelRow, 1].Style.Font.Color.SetColor(Color.Black);
        }

        private static void ProcessHtmlNodes(HtmlNode node, ExcelRange cellRef)
        {
            foreach (var child in node.ChildNodes)
            {
                if (child.NodeType == HtmlNodeType.Text)
                    cellRef.RichText.Add(child.InnerText);
                else if (child.Name == "span")
                {
                    var classAttr = child.GetAttributeValue("class", "");
                    var richText = cellRef.RichText.Add(child.InnerText);

                    switch (classAttr)
                    {
                        case "textNew":
                            richText.Color = Color.Green;
                            break;

                        case "textRemoved":
                            richText.Color = Color.Red;
                            richText.Strike = true;
                            break;

                        case "tagNew":
                            richText.Color = Color.Blue;
                            break;

                        case "tagRemoved":
                            richText.Color = Color.Gray;
                            richText.Strike = true;
                            break;

                        case "tag":
                            richText.Color = Color.Purple;
                            break;

                        default:
                            richText.Color = Color.Black;
                            break;
                    }
                }
            }
        }

        private static void SaveTableToExcel(string projectName, (string Processed, string Compared, string Errors) additionalInfo, List<HtmlNode> tableNodes, string filePath)
        {
            using var package = new ExcelPackage();
            foreach (var tableNode in tableNodes)
            {
                var rows = tableNode.SelectNodes(".//tr");
                if (rows == null || rows.Count < 2) continue;

                var sheetName = GenerateSheetName(rows[2]);
                var ws = package.Workbook.Worksheets.Add(sheetName);

                var columnCount = rows.First().SelectNodes("th|td").Count;

                AddTitle(ws, projectName, columnCount);
                AddAdditionalInfo(ws, additionalInfo, columnCount);

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
                        AddRowToWorksheet(ws, cells, excelRow, rows.First());
                        HighlightRow(ws, 3, Color.FromArgb(255, 155, 198, 199), true, columnCount);
                    }
                    else if (excelRow == 4)
                    {
                        MergeAndStyleRow(ws, cells, excelRow, Color.FromArgb(255, 155, 198, 199), columnCount);
                        HighlightRow(ws, excelRow, Color.FromArgb(230, 230, 230), false, columnCount);
                    }
                    else
                    {
                        AddRowToWorksheet(ws, cells, excelRow, rows.First());
                    }

                    excelRow++;
                }

                AutoFitColumns(ws);
                ws.View.FreezePanes(4, 1);
            }

            package.SaveAs(new FileInfo(filePath));
        }

        private static void SetNumeric(ExcelRange cellRef, double value, int excelCol)
        {
            cellRef.Value = value;
            cellRef.Style.Numberformat.Format = excelCol == 1 || excelCol == 5 ? "0" : "#,##0.00";
        }

        private static void SetPercentage(ExcelRange cellRef, double value)
        {
            cellRef.Value = value;
            cellRef.Style.Numberformat.Format = "0.00%";
        }

        private static void StyleHeaderCell(ExcelRange cellRef)
        {
            cellRef.Style.Font.Bold = true;
            cellRef.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cellRef.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 155, 198, 199));
            cellRef.Style.Font.Color.SetColor(Color.White);
            cellRef.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        private static bool TryParseNumeric(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private static bool TryParsePercentage(string text, out double value)
        {
            if (text.EndsWith("%") && double.TryParse(text.TrimEnd('%'), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                value /= 100;
                return true;
            }
            value = 0;
            return false;
        }
    }
}