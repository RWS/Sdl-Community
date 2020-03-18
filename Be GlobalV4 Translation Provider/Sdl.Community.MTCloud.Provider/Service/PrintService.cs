using System;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using Controls = System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class PrintService
	{
		/// <summary>
		/// Print Excel file
		/// </summary>
		/// <param name="filePath"></param>
		public void PrintFile(string filePath)
		{
			var printers = PrinterSettings.InstalledPrinters;
			int printerIndex = 0;
			var printDlg = new Controls.PrintDialog();

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
