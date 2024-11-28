using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;
using System.Linq;
using FromMarker = DocumentFormat.OpenXml.Drawing.Spreadsheet.FromMarker;

namespace Sdl.Community.TQA.Extensions
{
	public static class SpreadsheetExtension
	{
		public static void RemoveGridLinesFromSheet(this SpreadsheetDocument spreadsheet, string sheetName)
		{
			var workbookPart = spreadsheet.WorkbookPart;
			var versionHistorySheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
			var worksheetPart = (WorksheetPart)(workbookPart.GetPartById(versionHistorySheet.Id));

			// Access the sheet views.
			var sheetViews = worksheetPart.Worksheet.GetFirstChild<SheetViews>();
			sheetViews.Elements<SheetView>().FirstOrDefault().ShowGridLines = false;
		}

		public static void RepositionImageInCell(this SpreadsheetDocument spreadsheetDocument, string sheetName, string imageCell, int xOffset, int yOffset)
		{
			var workbookPart = spreadsheetDocument.WorkbookPart;
			var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
			var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);

			// Access the DrawingsPart
			var drawingsPart = worksheetPart.DrawingsPart;
			if (drawingsPart == null)
				return;

			// Access the WorksheetDrawing
			var worksheetDrawing = drawingsPart.WorksheetDrawing;

			// Locate the picture to move
			foreach (var twoCellAnchor in worksheetDrawing.Descendants<OneCellAnchor>())
			{
				var pic = twoCellAnchor.Descendants<DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture>().FirstOrDefault();

				if (pic != null)
				{
					// Get the position (xdr:from) and size (xdr:ext)
					var fromMarker = twoCellAnchor.Descendants<FromMarker>().FirstOrDefault();
					var ext = twoCellAnchor.Descendants<Extent>().FirstOrDefault();

					if (fromMarker != null && ext != null)
					{
						// Get the cell reference (for simplicity, assume cell is in format A1, B2, etc.)
						var column = imageCell[0] - 'A'; // Get column index (A=0, B=1, etc.)
						var row = int.Parse(imageCell.Substring(1)) - 1; // Get row index (0-based)

						// Get column width and row height (approximate in points, need to convert to EMUs)
						var colWidth = GetColumnWidthInEMUs(worksheetPart, column); // In EMUs
						var rowHeight = GetRowHeightInEMUs(worksheetPart, row);     // In EMUs

						// Calculate offsets to center the image
						var offsetX = (long)((colWidth - (xOffset * 9525)) / 2); // Horizontal offset in EMUs
						var offsetY = (long)((rowHeight - (yOffset * 9525)) / 2); // Vertical offset in EMUs

						// Update the starting point (xdr:from) with the offsets
						fromMarker.ColumnId.Text = column.ToString();
						fromMarker.RowId.Text = row.ToString();
						fromMarker.ColumnOffset.Text = offsetX.ToString(); // Set column offset to center image horizontally
						fromMarker.RowOffset.Text = offsetY.ToString();    // Set row offset to center image vertically

						// Save the changes
						worksheetDrawing.Save();
					}
				}
			}
		}

		public static void ResizeImageInExcel(this SpreadsheetDocument spreadsheetDocument, string sheetName, int newWidth, int newHeight)
		{
			{
				var workbookPart = spreadsheetDocument.WorkbookPart;
				var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
				var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);

				// Access the DrawingsPart
				var drawingsPart = worksheetPart.DrawingsPart;
				if (drawingsPart != null)
				{
					// Access the WorksheetDrawing
					var worksheetDrawing = drawingsPart.WorksheetDrawing;

					// Locate the picture to resize
					foreach (var twoCellAnchor in worksheetDrawing.Descendants<OneCellAnchor>())
					{
						var pic = twoCellAnchor.Descendants<DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture>().FirstOrDefault();

						if (pic != null)
						{
							// Get the extent (xdr:ext) element and modify width and height
							var ext = twoCellAnchor.Descendants<Extent>().FirstOrDefault();
							if (ext != null)
							{
								// Update the size in EMUs (1 pixel = 9525 EMUs)
								ext.Cx = newWidth * 9525;  // New width in pixels
								ext.Cy = newHeight * 9525; // New height in pixels
							}
						}
					}
					// Save the changes
					worksheetDrawing.Save();
				}
			}
		}

		public static void SetRangeBackgroundColorWhite(this SpreadsheetDocument document, string sheetName, string startCell, string endCell)
		{
			var workbookPart = document.WorkbookPart;
			var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
			var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
			var worksheet = worksheetPart.Worksheet;

			// Ensure the StylesPart exists
			var stylesPart = workbookPart.WorkbookStylesPart;
			if (stylesPart == null)
			{
				stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
				stylesPart.Stylesheet = new Stylesheet();
				stylesPart.Stylesheet.Save();
			}

			// Add the white fill style
			var whiteFillIndex = AddWhiteFill(stylesPart);

			// Get the range of cells
			var cells = GetCellsInRange(worksheetPart, startCell, endCell);

			foreach (var cell in cells)
			{
				// Apply the white fill style to each cell
				if (cell.StyleIndex == null)
				{
					// Create a new style for the cell if it doesn't have one
					cell.StyleIndex = whiteFillIndex;
				}
				else
				{
					// Update the cell's style index to include the white fill
					cell.StyleIndex = whiteFillIndex;
				}
			}

			worksheet.Save();
		}

		// Adds a white fill to the stylesheet and returns the index
		private static uint AddWhiteFill(WorkbookStylesPart stylesPart)
		{
			var stylesheet = stylesPart.Stylesheet;

			if (stylesheet.Fills == null)
			{
				stylesheet.Fills = new Fills();
				stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.None } });
				stylesheet.Fills.AppendChild(new Fill { PatternFill = new PatternFill { PatternType = PatternValues.Gray125 } });
			}

			var fills = stylesheet.Fills.Elements<Fill>().ToList();

			// Check if the white fill already exists
			foreach (var fill in fills)
			{
				if (fill.PatternFill != null && fill.PatternFill.ForegroundColor != null &&
					fill.PatternFill.ForegroundColor.Rgb == "FFFFFF")
				{
					return (uint)fills.IndexOf(fill);
				}
			}

			// Create a new white fill
			var whiteFill = new Fill
			{
				PatternFill = new PatternFill
				{
					PatternType = PatternValues.Solid,
					ForegroundColor = new ForegroundColor { Rgb = "FFFFFFFF" }, // White in ARGB
					BackgroundColor = new BackgroundColor { Rgb = "FFFFFFFF" }  // White in ARGB
				}
			};

			stylesheet.Fills.Append(whiteFill);
			stylesheet.Fills.Count = (uint)stylesheet.Fills.Count(); // Update fill count
			stylesPart.Stylesheet.Save();

			return (uint)(stylesheet.Fills.Count - 1); // Return the new white fill index
		}

		// Retrieves all cells within the specified range (startCell, endCell)
		private static IEnumerable<Cell> GetCellsInRange(WorksheetPart worksheetPart, string startCell, string endCell)
		{
			var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

			(var startColumn, var startRow) = GetColumnRowIndex(startCell);
			(var endColumn, var endRow) = GetColumnRowIndex(endCell);

			var cellsInRange = new List<Cell>();

			foreach (var row in sheetData.Elements<Row>())
			{
				if (row.RowIndex >= startRow && row.RowIndex <= endRow)
				{
					foreach (var cell in row.Elements<Cell>())
					{
						(var columnIndex, var rowIndex) = GetColumnRowIndex(cell.CellReference);
						if (columnIndex >= startColumn && columnIndex <= endColumn)
						{
							cellsInRange.Add(cell);
						}
					}
				}
			}

			return cellsInRange;
		}

		// Converts an Excel cell reference (e.g., "A1") to a tuple of column and row indices
		private static (int column, int row) GetColumnRowIndex(string cellReference)
		{
			var columnReference = new string(cellReference.Where(c => char.IsLetter(c)).ToArray());
			var rowReference = new string(cellReference.Where(c => char.IsDigit(c)).ToArray());

			var columnIndex = 0;
			foreach (var c in columnReference)
			{
				columnIndex = columnIndex * 26 + (c - 'A' + 1);
			}

			var rowIndex = int.Parse(rowReference);
			return (columnIndex, rowIndex);
		}

		private static double GetColumnWidthInEMUs(WorksheetPart worksheetPart, int columnIndex)
		{
			// Get the column width from the worksheet (use a default if not specified)
			// Convert the column width (in characters) to EMUs (approximate)
			var defaultColWidth = 8.43; // Default Excel column width in characters
			var columnWidth = defaultColWidth * 7.5 * 9525; // 7.5 pixels per character, 9525 EMUs per pixel
			return columnWidth;
		}

		private static double GetRowHeightInEMUs(WorksheetPart worksheetPart, int rowIndex)
		{
			// Get the row height from the worksheet (use a default if not specified)
			// Convert the row height (in points) to EMUs (approximate)
			double defaultRowHeight = 15; // Default Excel row height in points
			var rowHeight = defaultRowHeight * 72 / 96 * 9525; // 72 DPI conversion to EMUs
			return rowHeight;
		}
	}
}