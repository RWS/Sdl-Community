using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace ExportToExcel
{
    public class ExcelSuperWriter
    {
        #region "Members"
        private GeneratorSettings ConversionSettings
        {
            get;
            set;
        }

        private SpreadsheetDocument ExcelDocument
        {
            get;
            set;
        }

        private WorkbookPart ExcelWorkbookPart
        {
            get;
            set;
        }

        private WorksheetPart ExcelWorkSheetPart
        {
            get;
            set;
        }

        private SharedStringTablePart ExcelSharedStringsTable
        {
            get;
            set;
        }

        private Stylesheet ExcelStyleSheet
        {
            get;
            set;
        }

        private UInt32Value _headerStyle;
        public UInt32Value HeaderStyle
        {
            get
            {
                if (_headerStyle == null)
                {
                    _headerStyle = SetCellStyle(System.Drawing.Color.LightBlue, true);
                }
                return _headerStyle;
            }
            set { _headerStyle = value; }
        }

        private UInt32Value _noMatch;
        public UInt32Value NoMatch
        {
            get
            {
                if (_noMatch == null)
                {
                    _noMatch = SetCellStyle(ConversionSettings.NoMatchColor, ConversionSettings.IsNoMatchLocked);
                }
                return _noMatch;
            }
            set { _noMatch = value; }
        }

        private UInt32Value _exactMatch;
        public UInt32Value ExactMatch
        {
            get
            {
                if (_exactMatch == null)
                {
                    _exactMatch = SetCellStyle(ConversionSettings.ExactMatchColor, ConversionSettings.IsExactMatchLocked);
                }
                return _exactMatch;
            }
            set { _exactMatch = value; }
        }

        private UInt32Value _fuzzyMatch;
        public UInt32Value FuzzyMatch
        {
            get
            {
                if (_fuzzyMatch == null)
                {
                    _fuzzyMatch = SetCellStyle(ConversionSettings.FuzzyMatchColor, ConversionSettings.IsFuzzyMatchLocked);
                }
                return _fuzzyMatch;
            }
            set { _fuzzyMatch = value; }
        }

        private UInt32Value _contextMatch;
        public UInt32Value ContextMatch
        {
            get
            {
                if (_contextMatch == null)
                {
                    _contextMatch = SetCellStyle(ConversionSettings.ContextMatchColor, ConversionSettings.IsContextMatchLocked);
                }
                return _contextMatch;
            }
            set { _contextMatch = value; }
        }

        private UInt32Value _currentLine = 0;
        #endregion

        /// <summary>
        /// Creates a SharedStringItem with the specified text 
        /// and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private int InsertSharedStringItem(string text)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (ExcelSharedStringsTable.SharedStringTable == null)
            {
                ExcelSharedStringsTable.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in ExcelSharedStringsTable.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }
                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            ExcelSharedStringsTable.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            return i;
        }

        private void CreateHeader()
        {
            _currentLine++;
            InsertTextToCell("A", _currentLine, "Segment ID", HeaderStyle);
            InsertTextToCell("B", _currentLine, "Source text", HeaderStyle);
            InsertTextToCell("C", _currentLine, "Target text", HeaderStyle);
            InsertTextToCell("D", _currentLine, "Comments text", HeaderStyle);
        }

        /// <summary>
        /// Add text to specified cell, the specified color set
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellContent"></param>
        /// <param name="cellStyleId"></param>
        private void InsertTextToCell(string columnName, uint rowIndex, string cellContent, UInt32Value cellStyleId)
        {
            // Insert cell A1 into the new worksheet.
            Cell cell = GetNewCell(columnName, rowIndex);

            //insert cell content into shared strings table
            int index = InsertSharedStringItem(cellContent);

            cell.CellValue = new CellValue(index.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
            cell.StyleIndex = cellStyleId.Value;

        }

        private Cell GetNewCell(string columnName, uint rowIndex)
        {
            Worksheet worksheet = ExcelWorkSheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);
                return newCell;
            }
        }

        public void Initialize(string outputFile, GeneratorSettings settings)
        {
            //reset main settings
            ConversionSettings = settings;

            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            ExcelDocument = SpreadsheetDocument.Create(outputFile, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            ExcelWorkbookPart = ExcelDocument.AddWorkbookPart();
            ExcelWorkbookPart.Workbook = new Workbook();
            ExcelWorkbookPart.Workbook.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

            // Add a WorksheetPart to the WorkbookPart.
            ExcelWorkSheetPart = ExcelWorkbookPart.AddNewPart<WorksheetPart>();
            ExcelWorkSheetPart.Worksheet = SetWorksheetData();

            // Add Sheets to the Workbook.
            Sheets sheets = ExcelDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = ExcelDocument.WorkbookPart.GetIdOfPart(ExcelWorkSheetPart), SheetId = 1, Name = "Exported data" };
            sheets.Append(sheet);

            // Get the SharedStringTablePart.
            ExcelSharedStringsTable = ExcelDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();

            //Set custom styles
            WorkbookStylesPart workbookStylesPart = ExcelDocument.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            workbookStylesPart.Stylesheet = GenerateStylesheet();

            ExcelStyleSheet = ExcelDocument.WorkbookPart.WorkbookStylesPart.Stylesheet;

            CreateHeader();

            // Save the new worksheet.
            ExcelWorkSheetPart.Worksheet.Save();
        }


        /// <summary>
        /// Initialize the worksheet with required details
        /// </summary>
        /// <returns></returns>
        private Worksheet SetWorksheetData()
        {
            Worksheet worksheet = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            worksheet.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            Columns columns = new Columns();
            Column column1 = new Column() { Min = (UInt32Value)1U, Max = (UInt32Value)1U, Width = 15U, CustomWidth = true };
            Column column2 = new Column() { Min = (UInt32Value)2U, Max = (UInt32Value)2U, Width = (Double)ConversionSettings.ColumnWidth, CustomWidth = true };
            Column column3 = new Column() { Min = (UInt32Value)3U, Max = (UInt32Value)3U, Width = (Double)ConversionSettings.ColumnWidth, CustomWidth = true };
            Column column4 = new Column() { Min = (UInt32Value)4U, Max = (UInt32Value)4U, Width = (Double)ConversionSettings.ColumnWidth, CustomWidth = true };

            columns.Append(column1);
            columns.Append(column2);
            columns.Append(column3);
            columns.Append(column4);

            SheetData sheetData = new SheetData();

            SheetProtection sheetProtection = new SheetProtection() { Sheet = true, Objects = true, Scenarios = true };

            worksheet.Append(columns);
            worksheet.Append(sheetData);
            worksheet.Append(sheetProtection);
            return worksheet;
        }

        // Creates an Stylesheet instance and adds its children.
        public Stylesheet GenerateStylesheet()
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");


            Fonts fonts1 = new Fonts() { Count = (UInt32Value)1U };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            FontName fontName1 = new FontName() { Val = "Calibri" };

            font1.Append(fontSize1);
            font1.Append(fontName1);

            fonts1.Append(font1);

            Fills fills1 = new Fills() { Count = (UInt32Value)2U };

            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };

            fill1.Append(patternFill1);

            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };

            fill2.Append(patternFill2);

            fills1.Append(fill1);
            fills1.Append(fill2);

            Borders borders1 = new Borders() { Count = (UInt32Value)1U };
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
            LeftBorder leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
            Color color1 = new Color() { Indexed = (UInt32Value)64U };
            leftBorder2.Append(color1);
            RightBorder rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
            Color color2 = new Color() { Indexed = (UInt32Value)64U };
            rightBorder2.Append(color2);
            TopBorder topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
            Color color3 = new Color() { Indexed = (UInt32Value)64U };
            topBorder2.Append(color3);
            BottomBorder bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
            Color color4 = new Color() { Indexed = (UInt32Value)64U };
            bottomBorder2.Append(color4);
            DiagonalBorder diagonalBorder2 = new DiagonalBorder() { Style = BorderStyleValues.Thin };
            Color color5 = new Color() { Indexed = (UInt32Value)64U };
            diagonalBorder2.Append(color5);

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            borders1.Append(border1);
            borders1.Append(border2);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };

            cellFormats1.Append(cellFormat2);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            DocumentFormat.OpenXml.Office2010.Excel.SlicerStyles slicerStyles1 = new DocumentFormat.OpenXml.Office2010.Excel.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

            stylesheetExtension1.Append(slicerStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);
            return stylesheet1;
        }

        private UInt32Value SetCellStyle(System.Drawing.Color fillColor, bool locked)
        {
            Fill fill = new Fill();
            PatternFill patternFill = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = System.Drawing.Color.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B).Name };
            BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)64U };

            patternFill.Append(foregroundColor1);
            patternFill.Append(backgroundColor1);
            fill.Append(patternFill);
            ExcelStyleSheet.Fills.Count++;
            ExcelStyleSheet.Fills.Append(fill);

            CellFormat myCellFormat =
              new CellFormat
              {
                  FontId = (UInt32Value)0U,
                  FillId = (UInt32Value)ExcelStyleSheet.Fills.Count - 1,
                  ApplyFill = true,
                  NumberFormatId = (UInt32Value)0U,
                  FormatId = (UInt32Value)0U,
                  BorderId = (UInt32Value)1U,
                  ApplyBorder = true,
                  ApplyProtection = true
              };

            Alignment alignment = new Alignment() { WrapText = true };
            myCellFormat.Append(alignment);

            if (!locked)
            {
                Protection protection = new Protection() { Locked = false };
                myCellFormat.Append(protection);
            }

            ExcelStyleSheet.CellFormats.Count++;
            ExcelStyleSheet.CellFormats.Append(myCellFormat);
            return (UInt32Value)ExcelStyleSheet.CellFormats.Count - 1;
        }

        public void WriteEntry(string segmentId, string source, string target, List<string> comments, ISegmentPairProperties segmentProperties)
        {
            //increase current line number
            _currentLine++;
            UInt32Value color = GetSegmentMatchColor(segmentProperties.TranslationOrigin);

            InsertTextToCell("A", _currentLine, segmentId, color);
            InsertTextToCell("B", _currentLine, source, color);
            InsertTextToCell("C", _currentLine, target, color);

            //Extract comments
            if (comments.Count > 0)
            {
                string commentList = "";
                foreach (string item in comments)
                {
                    commentList += item + "\r\n";
                }
                InsertTextToCell("D", _currentLine, commentList, color);
            }
            else
            {
                //Insert empty cell to wrap other cells
                InsertTextToCell("D", _currentLine, " ", color);
            }
        }

        private UInt32Value GetSegmentMatchColor(ITranslationOrigin origin)
        {
            UInt32Value result = NoMatch;
            if (origin == null)
            {
                return NoMatch;
            }

            if (origin.IsStructureContextMatch || origin.TextContextMatchLevel == TextContextMatchLevel.SourceAndTarget)
            {

                result = ContextMatch;
            }
            else
            {
                if (origin.MatchPercent == 100)
                {
                    result = ExactMatch;
                }
                if (origin.MatchPercent > 0 && origin.MatchPercent < 100)
                {
                    result = FuzzyMatch;
                }
                if (origin.MatchPercent == 0)
                {
                    result = NoMatch;
                }
            }

            return result;
        }

        public void Complete()
        {
            //save all parts
            ExcelStyleSheet.Save();
            ExcelSharedStringsTable.SharedStringTable.Save();
            ExcelWorkSheetPart.Worksheet.Save();

            if (ExcelDocument != null)
            {
                ExcelDocument.Close();
            }
        }
    }
}
