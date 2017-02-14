using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Sdl.Community.Qualitivity.Custom
{
    public static class Excel
    {

        /// <summary>
        /// Creates the workbook
        /// </summary>
        /// <returns>Spreadsheet created</returns>
        public static SpreadsheetDocument CreateWorkbook(string fileName)
        {
            SpreadsheetDocument spreadSheet = null;

            try
            {
                // Create the Excel workbook
                spreadSheet = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook, false);

                // Create the parts and the corresponding objects

                // Workbook
                spreadSheet.AddWorkbookPart();
                spreadSheet.WorkbookPart.Workbook = new Workbook();
                spreadSheet.WorkbookPart.Workbook.Save();

                // Shared string table
                var sharedStringTablePart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
                sharedStringTablePart.SharedStringTable = new SharedStringTable();
                sharedStringTablePart.SharedStringTable.Save();

                // Sheets collection
                spreadSheet.WorkbookPart.Workbook.Sheets = new Sheets();
                spreadSheet.WorkbookPart.Workbook.Save();

                // Stylesheet
                var workbookStylesPart = spreadSheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();
                workbookStylesPart.Stylesheet = new Stylesheet();
                workbookStylesPart.Stylesheet.Save();
            }
            catch (Exception exception)
            {
                //System.Windows.MessageBox.Show(exception.Message, "Excel OpenXML basics", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Hand);
            }


            return spreadSheet;
        }

        /// <summary>
        /// Adds a new worksheet to the workbook
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="name">Name of the worksheet</param>
        /// <returns>True if succesful</returns>
        public static Worksheet AddWorksheet(SpreadsheetDocument spreadsheet, string name)
        {
            var sheets = spreadsheet.WorkbookPart.Workbook.GetFirstChild<Sheets>();

            // Add the worksheetpart
            var worksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());
            worksheetPart.Worksheet.Save();

            // Add the sheet and make relation to workbook
            var sheet = new Sheet
            {
                Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = (uint)(spreadsheet.WorkbookPart.Workbook.Sheets.Count() + 1),
                Name = name
            };
            sheets.Append(sheet);
            spreadsheet.WorkbookPart.Workbook.Save();

            return worksheetPart.Worksheet;
        }


        /// <summary>
        /// Adds the basic styles to the workbook
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <returns>True if succesful</returns>
        public static bool AddBasicStyles(SpreadsheetDocument spreadsheet)
        {
            var stylesheet = spreadsheet.WorkbookPart.WorkbookStylesPart.Stylesheet;

            // Numbering formats (x:numFmts)
            stylesheet.InsertAt(new NumberingFormats(), 0);
            // Currency
            stylesheet.GetFirstChild<NumberingFormats>().InsertAt(
               new NumberingFormat
               {
                   NumberFormatId = 164,
                   FormatCode = "#,##0.00"
                   + "\\ \"" + CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol + "\""
               }, 0);

            // Fonts (x:fonts)
            stylesheet.InsertAt(new Fonts(), 1);
            stylesheet.GetFirstChild<Fonts>().InsertAt(
               new Font
               {
                   FontSize = new FontSize
                   {
                       Val = 11
                   },
                   FontName = new FontName
                   {
                       Val = "Calibri"
                   }
               }, 0);

            // Fills (x:fills)
            stylesheet.InsertAt(new Fills(), 2);
            stylesheet.GetFirstChild<Fills>().InsertAt(
               new Fill
               {
                   PatternFill = new PatternFill
                   {
                       PatternType = new EnumValue<PatternValues>
                       {
                           Value = PatternValues.None
                       }
                   }
               }, 0);

            // Borders (x:borders)
            stylesheet.InsertAt(new Borders(), 3);
            stylesheet.GetFirstChild<Borders>().InsertAt(
               new Border
               {
                   LeftBorder = new LeftBorder(),
                   RightBorder = new RightBorder(),
                   TopBorder = new TopBorder(),
                   BottomBorder = new BottomBorder(),
                   DiagonalBorder = new DiagonalBorder()
               }, 0);

            // Cell style formats (x:CellStyleXfs)
            stylesheet.InsertAt(new CellStyleFormats(), 4);
            stylesheet.GetFirstChild<CellStyleFormats>().InsertAt(
               new CellFormat
               {
                   NumberFormatId = 0,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0
               }, 0);

            // Cell formats (x:CellXfs)
            stylesheet.InsertAt(new CellFormats(), 5);
            // General text
            stylesheet.GetFirstChild<CellFormats>().InsertAt(
               new CellFormat
               {
                   FormatId = 0,
                   NumberFormatId = 0
               }, 0);
            // Date
            stylesheet.GetFirstChild<CellFormats>().InsertAt(
               new CellFormat
               {
                   ApplyNumberFormat = true,
                   FormatId = 0,
                   NumberFormatId = 22,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0
               },
                  1);
            // Currency
            stylesheet.GetFirstChild<CellFormats>().InsertAt(
               new CellFormat
               {
                   ApplyNumberFormat = true,
                   FormatId = 0,
                   NumberFormatId = 164,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0
               },
                  2);
            // Percentage
            stylesheet.GetFirstChild<CellFormats>().InsertAt(
               new CellFormat
               {
                   ApplyNumberFormat = true,
                   FormatId = 0,
                   NumberFormatId = 10,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0
               },
                  3);

      

            stylesheet.Save();

            return true;
        }

        /// <summary>
        /// Adds a list of strings to the shared strings table.
        /// </summary>
        /// <param name="spreadsheet">The spreadsheet</param>
        /// <param name="stringList">Strings to add</param>
        /// <returns></returns>
        public static bool AddSharedStrings(SpreadsheetDocument spreadsheet, List<string> stringList)
        {
            foreach (var item in stringList)
            {
                AddSharedString(spreadsheet, item, false);
            }
            spreadsheet.WorkbookPart.SharedStringTablePart.SharedStringTable.Save();

            return true;
        }

        /// <summary>
        /// Add a single string to shared strings table.
        /// Shared string table is created if it doesn't exist.
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="stringItem">string to add</param>
        /// <param name="save">Save the shared string table</param>
        /// <returns></returns>
        public static bool AddSharedString(SpreadsheetDocument spreadsheet, string stringItem, bool save = true)
        {
            var sharedStringTable = spreadsheet.WorkbookPart.SharedStringTablePart.SharedStringTable;

            if (0 != sharedStringTable.Count(item => item.InnerText == stringItem)) return true;
            sharedStringTable.AppendChild(
                new SharedStringItem(
                    new Text(stringItem)));

            // Save the changes
            if (save)
            {
                sharedStringTable.Save();
            }

            return true;
        }
        /// <summary>
        /// Returns the index of a shared string.
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="stringItem">String to search for</param>
        /// <returns>Index of a shared string. -1 if not found</returns>
        public static int IndexOfSharedString(SpreadsheetDocument spreadsheet, string stringItem)
        {
            var sharedStringTable = spreadsheet.WorkbookPart.SharedStringTablePart.SharedStringTable;
            var found = false;
            var index = 0;

            foreach (var sharedString in sharedStringTable.Elements<SharedStringItem>())
            {
                if (sharedString.InnerText == stringItem)
                {
                    found = true;
                    break; ;
                }
                index++;
            }

            return found ? index : -1;
        }

        /// <summary>
        /// Converts a column number to column name (i.e. A, B, C..., AA, AB...)
        /// </summary>
        /// <param name="columnIndex">Index of the column</param>
        /// <returns>Column name</returns>
        public static string ColumnNameFromIndex(uint columnIndex)
        {
            var columnName = "";

            while (columnIndex > 0)
            {
                var remainder = (columnIndex - 1) % 26;
                columnName = Convert.ToChar(65 + remainder) + columnName;
                columnIndex = (columnIndex - remainder) / 26;
            }

            return columnName;
        }

        /// <summary>
        /// Sets a string value to a cell
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="stringValue">String value to set</param>
        /// <param name="useSharedString">Use shared strings? If true and the string isn't found in shared strings, it will be added</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, string stringValue, bool useSharedString, bool save = true)
        {
            var columnValue = stringValue;
            CellValues cellValueType;

            // Add the shared string if necessary
            if (useSharedString)
            {
                if (IndexOfSharedString(spreadsheet, stringValue) == -1)
                {
                    AddSharedString(spreadsheet, stringValue, true);
                }
                columnValue = IndexOfSharedString(spreadsheet, stringValue).ToString();
                cellValueType = CellValues.SharedString;
            }
            else
            {
                cellValueType = CellValues.String;
            }

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, cellValueType, columnValue, null, save);
        }

        /// <summary>
        /// Sets a cell value with a date
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="datetimeValue">DateTime value</param>
        /// <param name="styleIndex">Style to use</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, DateTime datetimeValue, uint? styleIndex, bool save = true)
        {
#if EN_US_CULTURE
         string columnValue = datetimeValue.ToOADate().ToString();
#else
            var columnValue = datetimeValue.ToOADate().ToString(CultureInfo.InvariantCulture).Replace(",", ".");
#endif

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, CellValues.Date, columnValue, styleIndex, save);
        }

        /// <summary>
        /// Sets a cell value with double number
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="doubleValue">Double value</param>
        /// <param name="styleIndex">Style to use</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, double doubleValue, uint? styleIndex, bool save = true)
        {
#if EN_US_CULTURE
         string columnValue = doubleValue.ToString();
#else
            var columnValue = doubleValue.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
#endif

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, CellValues.Number, columnValue, styleIndex, save);
        }

        /// <summary>
        /// Sets a cell value with double number
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="intValue"></param>
        /// <param name="styleIndex">Style to use</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, int intValue, uint? styleIndex, bool save = true)
        {
#if EN_US_CULTURE
         string columnValue = doubleValue.ToString();
#else
            var columnValue = intValue.ToString().Replace(",", ".");
#endif

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, CellValues.Number, columnValue, styleIndex, save);
        }


        /// <summary>
        /// Sets a cell value with double number
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="decimalValue"></param>
        /// <param name="styleIndex">Style to use</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, decimal decimalValue, uint? styleIndex, bool save = true)
        {
#if EN_US_CULTURE
         string columnValue = doubleValue.ToString();
#else
            var columnValue = decimalValue.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
#endif

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, CellValues.Number, columnValue, styleIndex, save);
        }


        /// <summary>
        /// Sets a cell value with boolean value
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="boolValue">Boolean value</param>
        /// <param name="styleIndex">Style to use</param>
        /// <param name="save">Save the worksheet</param>
        /// <returns>True if succesful</returns>
        public static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, bool boolValue, uint? styleIndex, bool save = true)
        {
            var columnValue = boolValue ? "1" : "0";

            return SetCellValue(spreadsheet, worksheet, columnIndex, rowIndex, CellValues.Boolean, columnValue, styleIndex, save);
        }


        /// <summary>
        /// Sets the column width
        /// </summary>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="width">Width to set</param>
        /// <returns>True if succesful</returns>
        public static bool SetColumnWidth(Worksheet worksheet, int columnIndex, int width)
        {
            // Get the column collection exists
            var columns = worksheet.Elements<Columns>().FirstOrDefault();
            if (columns == null)
            {
                return false;
            }
            // Get the column
            var column = columns.Elements<Column>().FirstOrDefault(item => item.Min == columnIndex);
            if (column != null)
            {
                column.Width = width;
                column.CustomWidth = true;
            }

            worksheet.Save();

            return true;
        }

        /// <summary>
        /// Sets a cell value. The row and the cell are created if they do not exist. If the cell exists, the contents of the cell is overwritten
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="worksheet">Worksheet to use</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="valueType">Type of the value</param>
        /// <param name="value">The actual value</param>
        /// <param name="styleIndex">Index of the style to use. Null if no style is to be defined</param>
        /// <param name="save">Save the worksheet?</param>
        /// <returns>True if succesful</returns>
        private static bool SetCellValue(SpreadsheetDocument spreadsheet, Worksheet worksheet, uint columnIndex, uint rowIndex, CellValues valueType, string value, uint? styleIndex, bool save = true)
        {
            var sheetData = worksheet.GetFirstChild<SheetData>();
            Row row;
            Row previousRow = null;
            Cell cell;
            Cell previousCell = null;
            Column previousColumn = null;
            var cellAddress = ColumnNameFromIndex(columnIndex) + rowIndex;
            
            
            // Check if the row exists, create if necessary
            if (sheetData.Elements<Row>().Count(item => item.RowIndex == rowIndex) != 0)
            {
                row = sheetData.Elements<Row>().First(item => item.RowIndex == rowIndex);
            }
            else
            {
                row = new Row { RowIndex = rowIndex };
                //sheetData.Append(row);
                for (var counter = rowIndex - 1; counter > 0; counter--)
                {
                    previousRow = sheetData.Elements<Row>().FirstOrDefault(item => item.RowIndex == counter);
                    if (previousRow != null)
                    {
                        break;
                    }
                }
                sheetData.InsertAfter(row, previousRow);
            }

            // Check if the cell exists, create if necessary
            if (row.Elements<Cell>().Any(item => item.CellReference.Value == cellAddress))
            {
                cell = row.Elements<Cell>().First(item => item.CellReference.Value == cellAddress);
            }
            else
            {
                // Find the previous existing cell in the row
                for (var counter = columnIndex - 1; counter > 0; counter--)
                {
                    previousCell = row.Elements<Cell>().FirstOrDefault(item => item.CellReference.Value == ColumnNameFromIndex(counter) + rowIndex);
                    if (previousCell != null)
                    {
                        break;
                    }
                }
                cell = new Cell { CellReference = cellAddress };
                row.InsertAfter(cell, previousCell);
            }

            // Check if the column collection exists
            var columns = worksheet.Elements<Columns>().FirstOrDefault() ?? worksheet.InsertAt(new Columns(), 0);
            // Check if the column exists
            if (columns.Elements<Column>().All(item => item.Min != columnIndex))
            {
                // Find the previous existing column in the columns
                for (var counter = columnIndex - 1; counter > 0; counter--)
                {
                    previousColumn = columns.Elements<Column>().FirstOrDefault(item => item.Min == counter);
                    if (previousColumn != null)
                    {
                        break;
                    }
                }
                columns.InsertAfter(
                   new Column
                   {
                       Min = columnIndex,
                       Max = columnIndex,
                       CustomWidth = true,
                       Width = 9
                   }, previousColumn);
            }

            // Add the value
            cell.CellValue = new CellValue(value);
            
            if (styleIndex != null)
            {
                cell.StyleIndex = styleIndex.Value;
            }
            if (valueType != CellValues.Date)
            {
                cell.DataType = new EnumValue<CellValues>(valueType);
            }
            
            //Alignment alignment1 = new Alignment() { WrapText = true };
           

            if (save)
            {
                worksheet.Save();
            }

            return true;
        }

        /// <summary>
        /// Adds a predefined style from the given xml
        /// </summary>
        /// <param name="spreadsheet">Spreadsheet to use</param>
        /// <param name="xml">Style definition as xml</param>
        /// <returns>True if succesful</returns>
        public static bool AddPredefinedStyles(SpreadsheetDocument spreadsheet, string xml)
        {
            spreadsheet.WorkbookPart.WorkbookStylesPart.Stylesheet.InnerXml = xml;
            spreadsheet.WorkbookPart.WorkbookStylesPart.Stylesheet.Save();

            return true;
        }
    }
}
