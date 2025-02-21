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
    class SimpleWordExcelHelper : ExcelHelperBase
    {
        public SimpleWordExcelHelper() 
        {
            AddToLinesValue = 16;
        }

        private Row GenerateRowForProperty(UInt32Value rowIndex, int linesToAdd, ProjectFile projectFile, String property)
        {
            Row row = new Row() { RowIndex = (UInt32Value)(rowIndex + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };
            ProjectProperty projectProperty = projectFile.ProjectProperties.FirstOrDefault(prop => prop.Type == property);
            if (projectProperty != null)
            {
                int sharedStringIndexForType = 0;

                for (int i = 0; i < SharedStrings.Count; i++)
                {
                    if (SharedStrings[i].Text.Text == projectProperty.Type)
                    {
                        sharedStringIndexForType = i;
                        break;
                    }
                }

                List<Cell> cells = new List<Cell>()
                    {
                        new Cell(){CellReference = GetCell(String.Format("C{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 7U,DataType = CellValues.SharedString, CellValue = new CellValue(sharedStringIndexForType.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("D{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 8U,DataType = CellValues.Number, CellValue = new CellValue(projectProperty.Rate.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("F{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.Number, CellValue = new CellValue(projectProperty.Words.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("G{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.Number, CellValue = new CellValue(projectProperty.Characters.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("H{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.Number, CellValue = new CellValue(projectProperty.ValueByWords.ToString(CultureInfo.InvariantCulture))},
                        new Cell(){CellReference = GetCell(String.Format("I{0}", rowIndex.Value), linesToAdd),StyleIndex = (UInt32Value) 9U,DataType = CellValues.SharedString, CellValue = new CellValue("15")}
                    };
                row.Append(cells.ConvertAll(c => (OpenXmlElement) c));
            }
            return row;
        }

        // Generates content of worksheet.
        protected override void GenerateWorksheetDataContent(SheetData sheetData, int linesToAdd, ProjectFile projectFile)
        {
            List<Row> rows = new List<Row>();
            List<Cell> cells = new List<Cell>();

            #region file name
            Row fileNameRow = new Row(){RowIndex = (UInt32Value)(2U + linesToAdd),Spans = new ListValue<StringValue>() { InnerText = "3:9" },DyDescent = 0.25D};

            if (FileNamesSharedStrings == null)
                FileNamesSharedStrings = new List<SharedStringItem>();
            FileNamesSharedStrings.Add(new SharedStringItem() { Text = new Text() { Text = String.Format("File: {0}", Path.GetFileName(projectFile.FileName)) } });

// ReSharper disable PossiblyMistakenUseOfParamsMethod
            fileNameRow.Append(new Cell()
            {
                CellReference = GetCell("C2", linesToAdd),
                StyleIndex = (UInt32Value)3U,
                DataType = CellValues.SharedString,
                CellValue = new CellValue((SharedStringsCount + FileNamesSharedStrings.Count).ToString(CultureInfo.InvariantCulture))
            }); rows.Add(fileNameRow);
// ReSharper restore PossiblyMistakenUseOfParamsMethod
            #endregion

            #region analysis results text
            Row arRow = new Row(){RowIndex = (UInt32Value) (3U+linesToAdd),Spans = new ListValue<StringValue>() {InnerText = "3:9"},DyDescent = 0.25D};

            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("F3", linesToAdd), StyleIndex = (UInt32Value)1U, DataType = CellValues.SharedString, CellValue = new CellValue("0") },
                    new Cell() {CellReference = GetCell("G3", linesToAdd), StyleIndex = (UInt32Value) 1U},
                    new Cell() {CellReference = GetCell("H3", linesToAdd), StyleIndex = (UInt32Value) 1U},
                    new Cell() {CellReference = GetCell("I3", linesToAdd), StyleIndex = (UInt32Value) 1U}
                };
            arRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(arRow);
            #endregion 

            #region header row
            Row headerRow = new Row(){RowIndex = (UInt32Value) (4U+linesToAdd),Spans = new ListValue<StringValue>() {InnerText = "3:9"},DyDescent = 0.25D};
            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("C4", linesToAdd), StyleIndex = (UInt32Value) 3U, DataType = CellValues.SharedString, CellValue = new CellValue("1")},
                    new Cell() {CellReference = GetCell("D4", linesToAdd), StyleIndex = (UInt32Value) 4U, DataType = CellValues.SharedString, CellValue = new CellValue("2")},
                    new Cell() {CellReference = GetCell("F4", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("3")},
                    new Cell() {CellReference = GetCell("G4", linesToAdd), StyleIndex = (UInt32Value) 5U, DataType = CellValues.SharedString, CellValue = new CellValue("4")},
                    new Cell() {CellReference = GetCell("H4", linesToAdd), StyleIndex = (UInt32Value) 6U, DataType = CellValues.SharedString, CellValue = new CellValue("5")},
                    new Cell() {CellReference = GetCell("I4", linesToAdd), StyleIndex = (UInt32Value) 2U}
                };

            headerRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(headerRow);
            #endregion

            #region value rows
            rows.Add(GenerateRowForProperty(5U, linesToAdd, projectFile, Templates.Templates.PerfectMatch));
            rows.Add(GenerateRowForProperty(6U, linesToAdd, projectFile, Templates.Templates.ContextMatch));
            rows.Add(GenerateRowForProperty(7U, linesToAdd, projectFile, Templates.Templates.Repetitions));
            rows.Add(GenerateRowForProperty(8U, linesToAdd, projectFile, Templates.Templates.Percent100));
            rows.Add(GenerateRowForProperty(9U, linesToAdd, projectFile, Templates.Templates.Percent95));
            rows.Add(GenerateRowForProperty(10U, linesToAdd, projectFile, Templates.Templates.Percent85));
            rows.Add(GenerateRowForProperty(11U, linesToAdd, projectFile, Templates.Templates.Percent75));
            rows.Add(GenerateRowForProperty(12U, linesToAdd, projectFile, Templates.Templates.Percent50));
            rows.Add(GenerateRowForProperty(13U, linesToAdd, projectFile, Templates.Templates.New));
            rows.Add(GenerateRowForProperty(14U, linesToAdd, projectFile, Templates.Templates.Tags));
            #endregion

            #region total rows
            Row emptyRow = new Row() { RowIndex = (UInt32Value)(15U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };
            cells = new List<Cell>()
                {
                    new Cell() {CellReference = GetCell("F15", linesToAdd), StyleIndex = (UInt32Value) 12U},
                    new Cell() {CellReference = GetCell("G15", linesToAdd), StyleIndex = (UInt32Value) 12U},
                    new Cell() {CellReference = GetCell("H15", linesToAdd), StyleIndex = (UInt32Value) 12U},
                    new Cell() {CellReference = GetCell("I15", linesToAdd), StyleIndex = (UInt32Value) 12U}
                };
            emptyRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(emptyRow);

            Row totalRow = new Row() { RowIndex = (UInt32Value)(16U + linesToAdd), Spans = new ListValue<StringValue>() { InnerText = "3:9" }, DyDescent = 0.25D };

            Decimal totalWords = projectFile.ProjectProperties.Where(property => property.StandardType != StandardType.Global).Sum(property => property.Words);
            Decimal totalChars = projectFile.ProjectProperties.Where(property => property.StandardType != StandardType.Global).Sum(property => property.Characters);
            Decimal totalValue = projectFile.ProjectProperties.Where(property => property.StandardType != StandardType.Global).Sum(property => property.ValueByWords);

            cells = new List<Cell>()
                {
                    new Cell(){CellReference = GetCell("F16", linesToAdd),StyleIndex = (UInt32Value) 13U, DataType = CellValues.Number, CellValue = new CellValue(totalWords.ToString(CultureInfo.InvariantCulture))},
                    new Cell(){CellReference = GetCell("G16", linesToAdd),StyleIndex = (UInt32Value) 13U, DataType = CellValues.Number, CellValue = new CellValue(totalChars.ToString(CultureInfo.InvariantCulture))},
                    new Cell(){CellReference = GetCell("H16", linesToAdd),StyleIndex = (UInt32Value) 13U, DataType = CellValues.Number, CellValue =new CellValue(totalValue.ToString(CultureInfo.InvariantCulture))},
                    new Cell(){CellReference = GetCell("I16", linesToAdd), StyleIndex = (UInt32Value) 13U ,DataType = CellValues.SharedString, CellValue = new CellValue("15")}
                };
            totalRow.Append(cells.ConvertAll(c => (OpenXmlElement)c));
            rows.Add(totalRow);
            #endregion

            sheetData.Append(rows.ConvertAll(c => (OpenXmlElement)c));
        }

        protected override MergeCell GenerateMergedCell(int linesToAdd)
        {
            return new MergeCell()
                {
                    Reference = String.Format("{0}:{1}", GetCell("F3", linesToAdd), GetCell("I3", linesToAdd))
                };
        }
    }
}
