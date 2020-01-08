using System;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using Controls = System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace Sdl.Community.BeGlobalV4.Provider.Service
{
	public class PrintService
	{
		/// <summary>
		/// Create a new Excel file
		/// </summary>
		/// <param name="filePath">filePath</param>
		public void CreateExcelFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			var app = new Excel.Application();
			var wb = app.Workbooks.Add("Workbook");
			var ws = (Excel.Worksheet)wb.Worksheets[1];

			ws.Cells[1, 1] = "Language";
			ws.Cells[1, 2] = "Region";
			ws.Cells[1, 3] = "Trados Code";
			ws.Cells[1, 4] = "MTCode (main)";
			ws.Cells[1, 5] = "MT Code (if locale is available)";

			ws.SaveAs(filePath, Excel.XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
				Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing);
			wb.Close(true, filePath, Type.Missing);
			app.Quit();
			Marshal.ReleaseComObject(ws);
			Marshal.ReleaseComObject(wb);
			Marshal.ReleaseComObject(app);
		}

		/// <summary>
		/// Print Excel file
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="printDlg"></param>
		public void PrintFile(string filePath)
		{
			var printDlg = new Controls.PrintDialog();
			var printers = PrinterSettings.InstalledPrinters;
			int printerIndex = 0;

			if (printDlg.ShowDialog() == true)

			{
				var excelApp = new Excel.Application();

				// Open the Workbook
				var wb = excelApp.Workbooks.Open(
					filePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
					Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

				// Get the first worksheet which corresponds to MTCodes. (Excel uses base 1 indexing, not base 0.)
				var ws = (Excel.Worksheet)wb.Worksheets[1];
				ws.PageSetup.FitToPagesWide = 1;
				ws.PageSetup.FitToPagesTall = false;
				ws.PageSetup.Zoom = false;
				wb.Save();

				// Identify the printer index based on the selected printer's name
				foreach (var printer in printers)
				{
					if (printer.Equals(printDlg.PrintQueue.FullName))
					{
						break;
					}
					printerIndex++;
				}

				// Print out 1 copy to the default printer:
				ws.PrintOut(Type.Missing, Type.Missing, 1, Type.Missing,
							printers[printerIndex], Type.Missing, Type.Missing, Type.Missing);

				// Cleanup
				GC.Collect();
				GC.WaitForPendingFinalizers();

				Marshal.FinalReleaseComObject(ws);

				wb.Close(false, filePath, Type.Missing);
				Marshal.FinalReleaseComObject(wb);

				excelApp.Quit();
				Marshal.FinalReleaseComObject(excelApp);
			}
		}
	}
}
