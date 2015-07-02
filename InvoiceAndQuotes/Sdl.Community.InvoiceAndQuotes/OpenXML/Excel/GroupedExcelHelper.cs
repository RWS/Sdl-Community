using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.InvoiceAndQuotes.Projects;

namespace Sdl.Community.InvoiceAndQuotes.OpenXML.Excel
{
    class GroupedExcelHelper : ExcelHelperBase
    {
        public GroupedExcelHelper()
        {
            AddToLinesValue = 10;
        }
        private Row GenerateRowForProperty(UInt32Value rowIndex, int linesToAdd, ProjectFile projectFile, String property)
        {
            Row row = new Row() { RowIndex = (UInt32Value)(rowIndex + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };
            ProjectProperty projectProperty = projectFile.ProjectProperties.FirstOrDefault(prop => prop.Type == property);
            if (projectProperty != null)
            {
                int sharedStringIndexForType = -1;

                for (int i = 0; i < SharedStrings.Count; i++)
                {
                    if (SharedStrings[i].Text.Text == projectProperty.Type)
                    {
                        sharedStringIndexForType = i;
                        break;
                    }
                }

                if (sharedStringIndexForType == -1)
                {
                    for (int i = 0; i < AdditionalSharedStrings.Count; i++)
                    {
                        if (SharedStrings[i].Text.Text == projectProperty.Type)
                        {
                            sharedStringIndexForType = SharedStrings.Count + i + 1;
                            break;
                        }
                    }
                }

                List<Cell> cells = new List<Cell>()
                    {
                        new Cell(){CellReference = GetCell(String.Format("C{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 7U,DataType = CellValues.SharedString, CellValue = new CellValue(sharedStringIndexForType.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("D{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 8U,DataType = CellValues.Number, CellValue = new CellValue(projectProperty.Rate.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("F{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.Number, CellValue = new CellValue(projectProperty.Words.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("G{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.Number, CellValue = new CellValue(projectProperty.ValueByWords.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("H{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.SharedString, CellValue = new CellValue("15")}
                    };
                row.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            }
            return row;
        }

        // Generates content of worksheet.
        protected override void GenerateWorksheetDataContent(SheetData sheetData, int linesToAdd, ProjectFile projectFile)
        {
            List<Row> rows = new List<Row>();
            List<Cell> cells = new List<Cell>();

            #region file name
            Row fileNameRow = new Row() { RowIndex = (UInt32Value)(2U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };
            if (FileNamesSharedStrings == null)
                FileNamesSharedStrings = new List<SharedStringItem>();
            FileNamesSharedStrings.Add(new SharedStringItem() { Text = new Text() { Text = String.Format("File: {0}", Path.GetFileName(projectFile.FileName)) } });

// ReSharper disable PossiblyMistakenUseOfParamsMethod
            fileNameRow.Append(new Cell()
            {
                CellReference = GetCell("C2", linesToAdd),
                StyleIndex = (UInt32Value)3U,
                DataType = CellValues.SharedString,
                CellValue = new CellValue((AdditionalSharedStrings.Count + SharedStringsCount + FileNamesSharedStrings.Count).ToString(CultureInfo.InvariantCulture))
            }); rows.Add(fileNameRow);
// ReSharper restore PossiblyMistakenUseOfParamsMethod
            #endregion

            #region analysis results text
            Row arRow = new Row() { RowIndex = (UInt32Value)(3U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };

            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("F3", linesToAdd), StyleIndex = (UInt32Value)1U, DataType = CellValues.SharedString, CellValue = new CellValue("0") },
                    new Cell() {CellReference = GetCell("G3", linesToAdd), StyleIndex = (UInt32Value) 1U},
                    new Cell() {CellReference = GetCell("H3", linesToAdd), StyleIndex = (UInt32Value) 1U},
                };
            arRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(arRow);
            #endregion

            #region header row
            Row headerRow = new Row() { RowIndex = (UInt32Value)(4U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };
            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("C4", linesToAdd), StyleIndex = (UInt32Value) 3U, DataType = CellValues.SharedString, CellValue = new CellValue("1")},
                    new Cell() {CellReference = GetCell("D4", linesToAdd), StyleIndex = (UInt32Value) 4U, DataType = CellValues.SharedString, CellValue = new CellValue("2")},
                    new Cell() {CellReference = GetCell("F4", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("3")},
                    new Cell() {CellReference = GetCell("G4", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("5")},
                    new Cell() {CellReference = GetCell("H4", linesToAdd), StyleIndex = (UInt32Value) 2U}
                };

            headerRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(headerRow);
            #endregion

            #region value rows
            rows.Add(GenerateRowForProperty(5U, linesToAdd, projectFile, Templates.Templates.RepsAnd100Percent));
            rows.Add(GenerateRowForProperty(6U, linesToAdd, projectFile, Templates.Templates.FuzzyMatches));
            rows.Add(GenerateRowForProperty(7U, linesToAdd, projectFile, Templates.Templates.NoMatch));
            rows.Add(GenerateRowForProperty(8U, linesToAdd, projectFile, Templates.Templates.Tags));
            #endregion

            #region total rows
            Row emptyRow = new Row() { RowIndex = (UInt32Value)(9U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };
            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("F9", linesToAdd), StyleIndex = (UInt32Value) 12U},
                    new Cell() {CellReference = GetCell("G9", linesToAdd), StyleIndex = (UInt32Value) 12U},
                    new Cell() {CellReference = GetCell("H9", linesToAdd), StyleIndex = (UInt32Value) 12U},
                };
            emptyRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(emptyRow);

            Row totalRow = new Row() { RowIndex = (UInt32Value)(10U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };

            Decimal totalWords = projectFile.ProjectProperties.Where(property => property.StandardType != StandardType.Standard).Sum(property => property.Words);
            Decimal totalValue = projectFile.ProjectProperties.Where(property => property.StandardType != StandardType.Standard).Sum(property => property.ValueByWords);

            cells = new List<Cell>()
                {
                    new Cell(){CellReference = GetCell("F10", linesToAdd),StyleIndex = (UInt32Value) 13U ,DataType = CellValues.Number, CellValue = new CellValue(totalWords.ToString(CultureInfo.InvariantCulture))},
                    new Cell(){CellReference = GetCell("G10", linesToAdd),StyleIndex = (UInt32Value) 13U ,DataType = CellValues.Number, CellValue = new CellValue(totalValue.ToString(CultureInfo.InvariantCulture))},
                    new Cell(){CellReference = GetCell("H10", linesToAdd), StyleIndex = (UInt32Value) 13U ,DataType = CellValues.SharedString, CellValue = new CellValue("15")}
                };
            totalRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(totalRow);
            #endregion

            sheetData.Append(rows.ConvertAll(c => (OpenXmlElement)c));
        }

        protected override List<SharedStringItem> GetAdditionalSharedStrings()
        {
            return AdditionalSharedStrings;
        }

        private List<SharedStringItem> AdditionalSharedStrings
        {
            get
            {
                return new List<SharedStringItem>()
                    {
                        new SharedStringItem() {Text = new Text() {Text = "Reps & 100% matches"}}, //16
                        new SharedStringItem() {Text = new Text() {Text = "Fuzzy matches"}}, //17
                        new SharedStringItem() {Text = new Text() {Text = "No match"}}, //18
                    };
            }
        }

        protected override MergeCell GenerateMergedCell(int linesToAdd)
        {
            return new MergeCell()
            {
                Reference = String.Format("{0}:{1}", GetCell("F3", linesToAdd), GetCell("H3", linesToAdd))
            };
        }
    }
}
