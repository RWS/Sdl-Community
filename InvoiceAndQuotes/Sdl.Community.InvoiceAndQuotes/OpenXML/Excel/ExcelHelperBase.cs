using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.Community.InvoiceAndQuotes.Customers;
using Sdl.Community.InvoiceAndQuotes.Projects;

using Color = DocumentFormat.OpenXml.Spreadsheet.Color;
using DifferentialFormats = DocumentFormat.OpenXml.Spreadsheet.DifferentialFormats;

namespace Sdl.Community.InvoiceAndQuotes.OpenXML.Excel
{
    class ExcelSheet
    {
        public SheetData SheetData { get; set; }
        public List<Row> Rows { get; set; }
    }

    class ExcelHelperBase
    {
        // Creates a SpreadsheetDocument.
        public void CreatePackage(string filePath, List<ProjectFile> projectFiles, Customer customer, User user)
        {
            using(SpreadsheetDocument package = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                CreateParts(package, projectFiles);
            }
        }

        // Updates a SpreadsheetDocument.
        public void UpdatePackage(string templateFile, string filePath, List<ProjectFile> projectFiles, Customer customer, User user)
        {
            File.Copy(templateFile, filePath, true);//.Replace("xlsx", "xltx"));
            Thread.Sleep(100);
            using (SpreadsheetDocument package = SpreadsheetDocument.Open(filePath, true, new OpenSettings {AutoSave = true}))
            {
                var worksheetsWithRows = package.WorkbookPart.WorksheetParts.ToDictionary(workSheet => workSheet, GetRowsForWorksheet);
                UpdateParts(package, worksheetsWithRows, projectFiles, customer, user);
            }
        }

        private List<ExcelSheet> GetRowsForWorksheet(WorksheetPart workSheetPart)
        {
            var sheetsData = workSheetPart.Worksheet.ChildElements.OfType<SheetData>();
            return sheetsData.Select(sheetData => new ExcelSheet { SheetData = sheetData, Rows = sheetData.ChildElements.OfType<Row>().ToList() }).ToList();
        }

        private void UpdateParts(SpreadsheetDocument document, Dictionary<WorksheetPart, List<ExcelSheet>> worksheetsWithRows, List<ProjectFile> projectFiles, Customer customer, User user)
        {
            var workSheets = document.WorkbookPart.WorksheetParts;

            foreach (var workSheet in workSheets)
            {
                UpdateWorksheetContent(workSheet, worksheetsWithRows[workSheet], document.WorkbookPart.SharedStringTablePart.SharedStringTable, projectFiles, customer, user);
            }
        }

        private void UpdateWorksheetContent(WorksheetPart workSheetPart, List<ExcelSheet> templateRows, SharedStringTable sharedStrings, List<ProjectFile> projectFiles, Customer customer, User user)
        {
            IEnumerable<SheetData> sheets = workSheetPart.Worksheet.ChildElements.OfType<SheetData>();

            foreach (var sheetData in sheets)
            {
                sheetData.RemoveAllChildren<Row>();

                List<String> strings = sharedStrings.ChildElements.OfType<SharedStringItem>().Where(shItem => shItem.ChildElements.OfType<Text>().Any()).Select(shItem =>
                    {
                        var firstOrDefault = shItem.ChildElements.OfType<Text>().FirstOrDefault();
                        return firstOrDefault != null ? firstOrDefault.Text : String.Empty;
                    }).ToList();

                //from this point is the content
                int linesToAdd = 0;
                var templateSheet = templateRows.FirstOrDefault(row => row.SheetData.Equals(sheetData));
                if (templateSheet != null)
                {
                    foreach (var projectFile in projectFiles)
                    {
                        GenerateWorksheetDataContent(sheetData, templateSheet.Rows, linesToAdd, new List<TokensProvider> {projectFile, customer, user}, strings);
                        linesToAdd += (templateSheet.Rows.Count + 5);
                    }
                }
            }
        }

        private void GenerateWorksheetDataContent(SheetData sheetData, IEnumerable<Row> templateRows, int linesToAdd, List<TokensProvider> objectsWithTokens, List<string> strings)
        {
            foreach (var templateRow in templateRows)
            {
                var rowToAdd = new Row
                {
                        RowIndex = templateRow.RowIndex + Convert.ToUInt32(linesToAdd),
                        Spans = templateRow.Spans,
                        DyDescent = templateRow.DyDescent
                    };

                var cells = templateRow.ChildElements.OfType<Cell>();
                foreach (var cell in cells)
                {
                    String cellValue = null;
                    {
                        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                        {
                            if (cell.CellValue != null) cellValue = strings[Convert.ToInt32(cell.CellValue.Text)];
                        }
                        else
                            cellValue = cell.CellValue == null ? null : cell.CellValue.Text;
                    }

                    var cellToAdd = new Cell
                    {
                        CellReference = GetCell(cell.CellReference, linesToAdd),
                        StyleIndex = cell.StyleIndex
                    };
                    if (cell.CellFormula != null)
                    {
                        cellToAdd.CellFormula = (CellFormula) cell.CellFormula.Clone();
                        cellToAdd.DataType = null;
                        cellToAdd.CellValue = null;
                    }
                    else
                    {
                        cellToAdd.DataType = GetDataType(cellValue, cell.DataType, objectsWithTokens);
                        cellToAdd.CellValue = cell.CellValue == null ? null : new CellValue(GetCellText(strings, cell.CellValue.Text, objectsWithTokens, cellValue, cell.DataType));
                    }
                    rowToAdd.Append(cellToAdd);
                }

                sheetData.Append(rowToAdd);
            }
        }

        private String GetCellText(List<String> strings, String initialValue, IEnumerable<TokensProvider> objectsWithTokens, string cellValue, EnumValue<CellValues> dataType)
        {
            foreach (var objectsWithToken in objectsWithTokens)
            {
                if (objectsWithToken == null) continue;
                if (objectsWithToken.HasToken(cellValue))
                {
                    return objectsWithToken.GetTokenValue(cellValue).ToString();
                }
            }
            if (dataType != null && dataType == CellValues.SharedString)
                return strings[Convert.ToInt32(initialValue)];
            return initialValue;
        }

        private EnumValue<CellValues> GetDataType(string cellValue, EnumValue<CellValues> dataType, IEnumerable<TokensProvider> objectsWithTokens)
        {
            if (String.IsNullOrEmpty(cellValue))
                return dataType;
            foreach (var objectsWithToken in objectsWithTokens)
            {
                if (objectsWithToken == null) continue;
                if (objectsWithToken.HasToken(cellValue))
                {
                    object value = objectsWithToken.GetTokenValue(cellValue);
                    if (value is string)
                        return CellValues.String;
                    return null;
                }
            }
            if (dataType != null && dataType == CellValues.SharedString) return CellValues.String;
            return dataType;
        }

        // Adds child parts and generates content of the specified part.
        private void CreateParts(SpreadsheetDocument document, List<ProjectFile> projectFiles)
        {
            ExtendedFilePropertiesPart extendedFileProperties = document.AddNewPart<ExtendedFilePropertiesPart>("rId3");
            GenerateExtendedFilePropertiesContent(extendedFileProperties);

            WorkbookPart workbook = document.AddWorkbookPart();
            GenerateWorkbookContent(workbook);

            WorkbookStylesPart workbookStyles = workbook.AddNewPart<WorkbookStylesPart>("rId2");
            GenerateWorkbookStylesContent(workbookStyles);

            //from this point is the content
            WorksheetPart worksheet = workbook.AddNewPart<WorksheetPart>("rId1");
            GenerateWorksheetContent(worksheet, projectFiles);

            SharedStringTablePart sharedStringTablePart = workbook.AddNewPart<SharedStringTablePart>("rId4");
            GenerateSharedStringTablePartContent(sharedStringTablePart);

            SetPackageProperties(document);
        }

        // Generates content of extendedFileProperties.
        private void GenerateExtendedFilePropertiesContent(ExtendedFilePropertiesPart extendedFileProperties)
        {
            extendedFileProperties.Properties = new DocumentFormat.OpenXml.ExtendedProperties.Properties();
            extendedFileProperties.Properties.AddNamespaceDeclaration("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
            extendedFileProperties.Properties.Append(new DocumentFormat.OpenXml.ExtendedProperties.Application() {Text = "Microsoft Excel"});
        }

        // Generates content of workbook.
        private void GenerateWorkbookContent(WorkbookPart workbook)
        {
            workbook.Workbook = new Workbook { MCAttributes = new MarkupCompatibilityAttributes { Ignorable = "x15" } };
            workbook.Workbook.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            workbook.Workbook.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            workbook.Workbook.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
            FileVersion fileVersion = new FileVersion { ApplicationName = "xl", LastEdited = "6", LowestEdited = "6", BuildVersion = "14420" };

            Sheets sheets = new Sheets();
            Sheet sheet = new Sheet { Name = "Sheet1", SheetId = (UInt32Value)1U, Id = "rId1" };

            sheets.Append(sheet);

            workbook.Workbook.Append(fileVersion);
            workbook.Workbook.Append(sheets);
        }

        // Generates content of workbookStyles.
        private void GenerateWorkbookStylesContent(WorkbookStylesPart workbookStyles)
        {
            Stylesheet stylesheet1 = new Stylesheet(){ MCAttributes = new MarkupCompatibilityAttributes(){ Ignorable = "x14ac" }  };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            NumberingFormats numberingFormats1 = new NumberingFormats(){ Count = (UInt32Value)1U };
            NumberingFormat numberingFormat1 = new NumberingFormat(){ NumberFormatId = (UInt32Value)43U, FormatCode = "_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)" };

            numberingFormats1.Append(numberingFormat1);

            Fonts fonts1 = new Fonts(){ Count = (UInt32Value)4U, KnownFonts = true };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize(){ Val = 11D };
            Color color1 = new Color(){ Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName(){ Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering(){ Val = 2 };
            FontScheme fontScheme1 = new FontScheme(){ Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);

            Font font2 = new Font();
            Bold bold1 = new Bold();
            FontSize fontSize2 = new FontSize(){ Val = 11D };
            Color color2 = new Color(){ Theme = (UInt32Value)1U };
            FontName fontName2 = new FontName(){ Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering(){ Val = 2 };
            FontScheme fontScheme2 = new FontScheme(){ Val = FontSchemeValues.Minor };

            font2.Append(bold1);
            font2.Append(fontSize2);
            font2.Append(color2);
            font2.Append(fontName2);
            font2.Append(fontFamilyNumbering2);
            font2.Append(fontScheme2);

            Font font3 = new Font();
            Bold bold2 = new Bold();
            Italic italic1 = new Italic();
            FontSize fontSize3 = new FontSize(){ Val = 11D };
            Color color3 = new Color(){ Rgb = "FFFF0000" };
            FontName fontName3 = new FontName(){ Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering3 = new FontFamilyNumbering(){ Val = 2 };
            FontScheme fontScheme3 = new FontScheme(){ Val = FontSchemeValues.Minor };

            font3.Append(bold2);
            font3.Append(italic1);
            font3.Append(fontSize3);
            font3.Append(color3);
            font3.Append(fontName3);
            font3.Append(fontFamilyNumbering3);
            font3.Append(fontScheme3);

            Font font4 = new Font();
            Italic italic2 = new Italic();
            FontSize fontSize4 = new FontSize(){ Val = 11D };
            Color color4 = new Color(){ Rgb = "FFFF0000" };
            FontName fontName4 = new FontName(){ Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering4 = new FontFamilyNumbering(){ Val = 2 };
            FontScheme fontScheme4 = new FontScheme(){ Val = FontSchemeValues.Minor };

            font4.Append(italic2);
            font4.Append(fontSize4);
            font4.Append(color4);
            font4.Append(fontName4);
            font4.Append(fontFamilyNumbering4);
            font4.Append(fontScheme4);

            fonts1.Append(font1);
            fonts1.Append(font2);
            fonts1.Append(font3);
            fonts1.Append(font4);

            Fills fills1 = new Fills(){ Count = (UInt32Value)4U };

            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill(){ PatternType = PatternValues.None };

            fill1.Append(patternFill1);

            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill(){ PatternType = PatternValues.Gray125 };

            fill2.Append(patternFill2);

            Fill fill3 = new Fill();

            PatternFill patternFill3 = new PatternFill(){ PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor(){ Theme = (UInt32Value)0U, Tint = -4.9989318521683403E-2D };
            BackgroundColor backgroundColor1 = new BackgroundColor(){ Indexed = (UInt32Value)64U };

            patternFill3.Append(foregroundColor1);
            patternFill3.Append(backgroundColor1);

            fill3.Append(patternFill3);

            Fill fill4 = new Fill();

            PatternFill patternFill4 = new PatternFill(){ PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor2 = new ForegroundColor(){ Rgb = "FFFFFFCC" };
            BackgroundColor backgroundColor2 = new BackgroundColor(){ Indexed = (UInt32Value)64U };

            patternFill4.Append(foregroundColor2);
            patternFill4.Append(backgroundColor2);

            fill4.Append(patternFill4);

            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);
            fills1.Append(fill4);

            Borders borders1 = new Borders(){ Count = (UInt32Value)2U };

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            Border border2 = new Border();
            LeftBorder leftBorder2 = new LeftBorder();
            RightBorder rightBorder2 = new RightBorder();

            TopBorder topBorder2 = new TopBorder(){ Style = BorderStyleValues.Thin };
            Color color5 = new Color(){ Indexed = (UInt32Value)64U };

            topBorder2.Append(color5);
            BottomBorder bottomBorder2 = new BottomBorder();
            DiagonalBorder diagonalBorder2 = new DiagonalBorder();

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            borders1.Append(border1);
            borders1.Append(border2);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats(){ Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats(){ Count = (UInt32Value)16U };
            CellFormat cellFormat2 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };

            CellFormat cellFormat3 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyAlignment = true };
            Alignment alignment1 = new Alignment(){ Horizontal = HorizontalAlignmentValues.Center };

            cellFormat3.Append(alignment1);
            CellFormat cellFormat4 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };

            CellFormat cellFormat5 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyAlignment = true };
            Alignment alignment2 = new Alignment(){ Horizontal = HorizontalAlignmentValues.Left };

            cellFormat5.Append(alignment2);

            CellFormat cellFormat6 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyAlignment = true };
            Alignment alignment3 = new Alignment(){ Horizontal = HorizontalAlignmentValues.Center };

            cellFormat6.Append(alignment3);

            CellFormat cellFormat7 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyAlignment = true };
            Alignment alignment4 = new Alignment(){ Horizontal = HorizontalAlignmentValues.Center };

            cellFormat7.Append(alignment4);

            CellFormat cellFormat8 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyAlignment = true };
            Alignment alignment5 = new Alignment(){ Horizontal = HorizontalAlignmentValues.Right };

            cellFormat8.Append(alignment5);

            CellFormat cellFormat9 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true, ApplyAlignment = true };
            Alignment alignment6 = new Alignment(){ Horizontal = HorizontalAlignmentValues.Left };

            cellFormat9.Append(alignment6);

            CellFormat cellFormat10 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true, ApplyAlignment = true };
            Alignment alignment7 = new Alignment(){ Horizontal = HorizontalAlignmentValues.Center };

            cellFormat10.Append(alignment7);

            CellFormat cellFormat11 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)3U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyAlignment = true };
            Alignment alignment8 = new Alignment(){ Horizontal = HorizontalAlignmentValues.Center };

            cellFormat11.Append(alignment8);
            CellFormat cellFormat12 = new CellFormat(){ NumberFormatId = (UInt32Value)43U, FontId = (UInt32Value)2U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyNumberFormat = true, ApplyFont = true, ApplyFill = true };

            CellFormat cellFormat13 = new CellFormat(){ NumberFormatId = (UInt32Value)9U, FontId = (UInt32Value)0U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyNumberFormat = true, ApplyFill = true, ApplyAlignment = true };
            Alignment alignment9 = new Alignment(){ Horizontal = HorizontalAlignmentValues.Left };

            cellFormat13.Append(alignment9);

            CellFormat cellFormat14 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true, ApplyAlignment = true };
            Alignment alignment10 = new Alignment(){ Horizontal = HorizontalAlignmentValues.Center };

            cellFormat14.Append(alignment10);

            CellFormat cellFormat15 = new CellFormat(){ NumberFormatId = (UInt32Value)37U, FontId = (UInt32Value)2U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyNumberFormat = true, ApplyFont = true, ApplyFill = true, ApplyBorder = true, ApplyAlignment = true };
            Alignment alignment11 = new Alignment(){ Horizontal = HorizontalAlignmentValues.Center };

            cellFormat15.Append(alignment11);
            CellFormat cellFormat16 = new CellFormat(){ NumberFormatId = (UInt32Value)43U, FontId = (UInt32Value)2U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyNumberFormat = true, ApplyFont = true, ApplyFill = true, ApplyBorder = true };
            CellFormat cellFormat17 = new CellFormat(){ NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyFill = true, ApplyBorder = true };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);
            cellFormats1.Append(cellFormat5);
            cellFormats1.Append(cellFormat6);
            cellFormats1.Append(cellFormat7);
            cellFormats1.Append(cellFormat8);
            cellFormats1.Append(cellFormat9);
            cellFormats1.Append(cellFormat10);
            cellFormats1.Append(cellFormat11);
            cellFormats1.Append(cellFormat12);
            cellFormats1.Append(cellFormat13);
            cellFormats1.Append(cellFormat14);
            cellFormats1.Append(cellFormat15);
            cellFormats1.Append(cellFormat16);
            cellFormats1.Append(cellFormat17);

            CellStyles cellStyles1 = new CellStyles(){ Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle(){ Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats(){ Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles(){ Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension(){ Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            DocumentFormat.OpenXml.Office2010.Excel.SlicerStyles slicerStyles1 = new DocumentFormat.OpenXml.Office2010.Excel.SlicerStyles(){ DefaultSlicerStyle = "SlicerStyleLight1" };

            stylesheetExtension1.Append(slicerStyles1);

            //StylesheetExtension stylesheetExtension2 = new StylesheetExtension(){ Uri = "{9260A510-F301-46a8-8635-F512D64BE5F5}" };
            //stylesheetExtension2.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
            //X15.TimelineStyles timelineStyles1 = new X15.TimelineStyles(){ DefaultTimelineStyle = "TimeSlicerStyleLight1" };

            //stylesheetExtension2.Append(timelineStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);
            //stylesheetExtensionList1.Append(stylesheetExtension2);

            stylesheet1.Append(numberingFormats1);
            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);

            workbookStyles.Stylesheet = stylesheet1;
        }

        private int _addToLinesValue = 16;

        public int AddToLinesValue
        {
            get { return _addToLinesValue; }
            set { _addToLinesValue = value; }
        }

        // Generates content of worksheet.
        private void GenerateWorksheetContent(WorksheetPart worksheetPart, List<ProjectFile> projectFiles)
        {
            worksheetPart.Worksheet = new Worksheet()
                {
                    MCAttributes = new MarkupCompatibilityAttributes() {Ignorable = "x14ac"}
                };
            worksheetPart.Worksheet.AddNamespaceDeclaration("r",
                                                            "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheetPart.Worksheet.AddNamespaceDeclaration("mc",
                                                            "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheetPart.Worksheet.AddNamespaceDeclaration("x14ac",
                                                            "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            var sheetData = new SheetData();

            //from this point is the content
            int linesToAdd = 0;
            var mergeCells = new MergeCells();
            foreach (var porjectFile in projectFiles)
            {
                GenerateWorksheetDataContent(sheetData, linesToAdd, porjectFile);
                MergeCell mergeCell = GenerateMergedCell(linesToAdd);
                if (mergeCell != null) mergeCells.Append(mergeCell);
                linesToAdd += AddToLinesValue;
            }

            worksheetPart.Worksheet.Append(sheetData);
            if (mergeCells.Any())
                worksheetPart.Worksheet.Append(mergeCells);
        }

        protected virtual MergeCell GenerateMergedCell(int linesToAdd)
        {
            return null;
        }

        protected virtual void GenerateWorksheetDataContent(SheetData sheetData1, int linesToAdd, ProjectFile porjectFile)
        {
        }

        private void GenerateSharedStringTablePartContent(SharedStringTablePart sharedStringTablePart)
        {
            var sharedStringTable = new SharedStringTable();
            sharedStringTable.Append(SharedStrings.ConvertAll(c => (OpenXmlElement)c));
            List<SharedStringItem> additionalStrings = GetAdditionalSharedStrings();
            if (additionalStrings != null) sharedStringTable.Append(additionalStrings.ConvertAll(c => (OpenXmlElement)c));
            if (FileNamesSharedStrings.Any()) sharedStringTable.Append(FileNamesSharedStrings.ConvertAll(c => (OpenXmlElement)c));

            sharedStringTablePart.SharedStringTable = sharedStringTable;
        }

        public List<SharedStringItem> FileNamesSharedStrings { get; set; }
        protected virtual List<SharedStringItem> GetAdditionalSharedStrings()
        {
            return null;
        }

        protected const int SharedStringsCount = 16;
        protected List<SharedStringItem> SharedStrings
        {
            get
            {
                return new List<SharedStringItem>()
                    {
                        new SharedStringItem() {Text = new Text() {Text = "Analysis Results"}}, //0
                        new SharedStringItem() {Text = new Text() {Text = "Type"}}, //1
                        new SharedStringItem() {Text = new Text() {Text = "Rates"}}, //2
                        new SharedStringItem() {Text = new Text() {Text = "Words"}}, //3
                        new SharedStringItem() {Text = new Text() {Text = "Characters"}}, //4
                        new SharedStringItem() {Text = new Text() {Text = "Value"}}, //5
                        new SharedStringItem() {Text = new Text() {Text = "Perfect Match"}}, //6
                        new SharedStringItem() {Text = new Text() {Text = "Context Match"}}, //7
                        new SharedStringItem() {Text = new Text() {Text = "Repetitions"}}, //8
                        new SharedStringItem() {Text = new Text() {Text = "95% - 99%"}}, //9
                        new SharedStringItem() {Text = new Text() {Text = "85% - 94%"}}, //10
                        new SharedStringItem() {Text = new Text() {Text = "75% - 84%"}}, //11
                        new SharedStringItem() {Text = new Text() {Text = "50% - 74%"}}, //12
                        new SharedStringItem() {Text = new Text() {Text = "New"}}, //13
                        new SharedStringItem() {Text = new Text() {Text = "Tags"}}, //14
                        new SharedStringItem() {Text = new Text() {Text = "Euros"}}, //15
                    };
            }
        }
        private void SetPackageProperties(OpenXmlPackage document)
        {
            var windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null)
                document.PackageProperties.LastModifiedBy = document.PackageProperties.Creator = windowsIdentity.Name;
            document.PackageProperties.Modified = document.PackageProperties.Created = DateTime.Now;
        }

        protected String GetCell(String cell, int rowsToAdd)
        {
            String cellLetter = cell.Substring(0, 1);
            String rowNumber = cell.Substring(1, cell.Length - 1);
            return String.Format("{0}{1}", cellLetter, Convert.ToInt32(rowNumber) + rowsToAdd);
        }
    }
}
