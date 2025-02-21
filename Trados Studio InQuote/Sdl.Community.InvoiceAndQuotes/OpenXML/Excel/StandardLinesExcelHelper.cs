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
    class StandardLinesExcelHelper : ExcelHelperBase
    {
        public StandardLinesExcelHelper()
        {
            AddToLinesValue = 23;
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
                        new Cell(){CellReference = GetCell(String.Format("F{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.Number, CellValue = new CellValue(projectProperty.LinesByCharacters.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("G{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.Number, CellValue = new CellValue(projectProperty.ValueByLbC.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("H{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.SharedString, CellValue = new CellValue("15")},
                        new Cell(){CellReference = GetCell(String.Format("J{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.Number, CellValue = new CellValue(projectProperty.LinesByKeyStrokes.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("K{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.Number, CellValue = new CellValue(projectProperty.ValueByLbK.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("L{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.SharedString, CellValue = new CellValue("15")}
                    };
                row.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            }
            return row;
        }

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
                    StyleIndex = (UInt32Value) 3U,
                    DataType = CellValues.SharedString,
                    CellValue = new CellValue((AdditionalSharedStrings.Count + SharedStringsCount + FileNamesSharedStrings.Count).ToString(CultureInfo.InvariantCulture))
                });
            rows.Add(fileNameRow);
// ReSharper restore PossiblyMistakenUseOfParamsMethod
            #endregion

            #region Rate
            Row rateRow1 = new Row() { RowIndex = (UInt32Value)(4U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };

            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("C4", linesToAdd), StyleIndex = (UInt32Value)1U, DataType = CellValues.SharedString, CellValue = new CellValue("16") },
                    new Cell() {CellReference = GetCell("D4", linesToAdd), StyleIndex = (UInt32Value) 1U, DataType = CellValues.Number, CellValue = new CellValue(projectFile.LineCharacters.ToString(CultureInfo.InvariantCulture))},
                    new Cell() {CellReference = GetCell("E4", linesToAdd), StyleIndex = (UInt32Value) 1U, DataType = CellValues.SharedString, CellValue = new CellValue("4")},
                };
            rateRow1.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(rateRow1);

            Row rateRow2 = new Row() { RowIndex = (UInt32Value)(5U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };

            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("C5", linesToAdd), StyleIndex = (UInt32Value)1U, DataType = CellValues.SharedString, CellValue = new CellValue("17") },
                    new Cell() {CellReference = GetCell("D5", linesToAdd), StyleIndex = (UInt32Value) 1U, DataType = CellValues.Number, CellValue = new CellValue(projectFile.RatePerLine.ToString(CultureInfo.InvariantCulture))},
                    new Cell() {CellReference = GetCell("E5", linesToAdd), StyleIndex = (UInt32Value) 1U, DataType = CellValues.SharedString, CellValue = new CellValue("15")},
                };
            rateRow2.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(rateRow2);
            #endregion

            #region lines payment infos
            Row justLinesRow = new Row() { RowIndex = (UInt32Value)(7U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };
            justLinesRow.Append(new Cell() { CellReference = GetCell("C7", linesToAdd), StyleIndex = (UInt32Value)5U, DataType = CellValues.SharedString, CellValue = new CellValue("18") });
            rows.Add(justLinesRow);
            
            Row charsRow = new Row() { RowIndex = (UInt32Value)(8U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };
            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("C8", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("19")},
                    new Cell() {CellReference = GetCell("D8", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.Number, CellValue = new CellValue(projectFile.LinesByCharacters.ToString(CultureInfo.InvariantCulture))},
                    new Cell() {CellReference = GetCell("F8", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("20")},
                    new Cell() {CellReference = GetCell("G8", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.Number,  CellValue = new CellValue((projectFile.LinesByCharacters*projectFile.RatePerLine).ToString(CultureInfo.InvariantCulture))},
                    new Cell() {CellReference = GetCell("H8", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("15")}
                };

            charsRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(charsRow);

            Row keysRow = new Row() { RowIndex = (UInt32Value)(9U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };
            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("C9", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("21")},
                    new Cell() {CellReference = GetCell("D9", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.Number, CellValue = new CellValue(projectFile.LinesByKeyStrokes.ToString(CultureInfo.InvariantCulture))},
                    new Cell() {CellReference = GetCell("F9", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("20")},
                    new Cell() {CellReference = GetCell("G9", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.Number, CellValue = new CellValue((projectFile.LinesByKeyStrokes*projectFile.RatePerLine).ToString(CultureInfo.InvariantCulture))},
                    new Cell() {CellReference = GetCell("H9", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("15")}
                };

            keysRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(keysRow);
            #endregion

            #region header row
            Row headerRow = new Row() { RowIndex = (UInt32Value)(11U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };
            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("C11", linesToAdd), StyleIndex = (UInt32Value) 3U, DataType = CellValues.SharedString, CellValue = new CellValue("1")},
                    new Cell() {CellReference = GetCell("D11", linesToAdd), StyleIndex = (UInt32Value) 4U, DataType = CellValues.SharedString, CellValue = new CellValue("2")},
                    new Cell() {CellReference = GetCell("F11", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("22")},
                    new Cell() {CellReference = GetCell("G11", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("5")},
                    new Cell() {CellReference = GetCell("H11", linesToAdd), StyleIndex = (UInt32Value) 2U},
                    new Cell() {CellReference = GetCell("J11", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("23")},
                    new Cell() {CellReference = GetCell("K11", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("5")},
                    new Cell() {CellReference = GetCell("L11", linesToAdd), StyleIndex = (UInt32Value) 2U}
                };

            headerRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(headerRow);
            #endregion

            #region value rows
            rows.Add(GenerateRowForProperty(12U, linesToAdd, projectFile, Templates.Templates.PerfectMatch));
            rows.Add(GenerateRowForProperty(13U, linesToAdd, projectFile, Templates.Templates.ContextMatch));
            rows.Add(GenerateRowForProperty(14U, linesToAdd, projectFile, Templates.Templates.Repetitions));
            rows.Add(GenerateRowForProperty(15U, linesToAdd, projectFile, Templates.Templates.Percent100));
            rows.Add(GenerateRowForProperty(16U, linesToAdd, projectFile, Templates.Templates.Percent95));
            rows.Add(GenerateRowForProperty(17U, linesToAdd, projectFile, Templates.Templates.Percent85));
            rows.Add(GenerateRowForProperty(18U, linesToAdd, projectFile, Templates.Templates.Percent75));
            rows.Add(GenerateRowForProperty(19U, linesToAdd, projectFile, Templates.Templates.Percent50));
            rows.Add(GenerateRowForProperty(20U, linesToAdd, projectFile, Templates.Templates.New));
            rows.Add(GenerateRowForProperty(21U, linesToAdd, projectFile, Templates.Templates.Tags));
            #endregion

            #region total rows
            Row emptyRow = new Row() { RowIndex = (UInt32Value)(22U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };
            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("F22", linesToAdd), StyleIndex = (UInt32Value) 12U},
                    new Cell() {CellReference = GetCell("G22", linesToAdd), StyleIndex = (UInt32Value) 12U},
                    new Cell() {CellReference = GetCell("H22", linesToAdd), StyleIndex = (UInt32Value) 12U},
                    new Cell() {CellReference = GetCell("J22", linesToAdd), StyleIndex = (UInt32Value) 12U},
                    new Cell() {CellReference = GetCell("K22", linesToAdd), StyleIndex = (UInt32Value) 12U},
                    new Cell() {CellReference = GetCell("L22", linesToAdd), StyleIndex = (UInt32Value) 12U},
                };
            emptyRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(emptyRow);

            Row totalRow = new Row() { RowIndex = (UInt32Value)(23U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };

            Decimal totalValueC = projectFile.ProjectProperties.Where(property => property.StandardType != StandardType.Global).Sum(property => property.ValueByLbC);
            Decimal totalValueK = projectFile.ProjectProperties.Where(property => property.StandardType != StandardType.Global).Sum(property => property.ValueByLbK);

            cells = new List<Cell>()
                {
                    new Cell(){CellReference = GetCell("F23", linesToAdd),StyleIndex = (UInt32Value) 12U },
                    new Cell(){CellReference = GetCell("G23", linesToAdd),StyleIndex = (UInt32Value) 13U ,DataType = CellValues.Number, CellValue = new CellValue(totalValueC.ToString(CultureInfo.InvariantCulture))},
                    new Cell(){CellReference = GetCell("H23", linesToAdd), StyleIndex = (UInt32Value) 13U ,DataType = CellValues.SharedString, CellValue = new CellValue("15")},
                    new Cell(){CellReference = GetCell("J23", linesToAdd),StyleIndex = (UInt32Value) 12U },
                    new Cell(){CellReference = GetCell("K23", linesToAdd),StyleIndex = (UInt32Value) 13U ,DataType = CellValues.Number, CellValue = new CellValue(totalValueK.ToString(CultureInfo.InvariantCulture))},
                    new Cell(){CellReference = GetCell("L23", linesToAdd), StyleIndex = (UInt32Value) 13U ,DataType = CellValues.SharedString, CellValue = new CellValue("15")}
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
                        new SharedStringItem() {Text = new Text() {Text = "Standard line:"}}, //16
                        new SharedStringItem() {Text = new Text() {Text = "Rate/line:"}}, //17
                        new SharedStringItem() {Text = new Text() {Text = "Just lines"}}, //18
                        new SharedStringItem() {Text = new Text() {Text = "Payment by Characters"}}, //19
                        new SharedStringItem() {Text = new Text() {Text = "standard lines"}}, //20
                        new SharedStringItem() {Text = new Text() {Text = "Payment by Keystrokes"}}, //21
                        new SharedStringItem() {Text = new Text() {Text = "Lines by Characters"}}, //22
                        new SharedStringItem() {Text = new Text() {Text = "Lines by Keystrokes"}}, //23
                    };
            }
        }
    }
}
