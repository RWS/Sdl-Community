using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.Toolkit.LanguagePlatform.ExcelParser;
using Sdl.Community.Toolkit.LanguagePlatform.Models;
using Sdl.Core.Globalization;
using Excel = Microsoft.Office.Interop.Excel;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class MTCodesViewModel : BaseViewModel
	{
		private MTCodeModel _selectedMTCode;
		private ObservableCollection<MTCodeModel> _mtCodes;
		private string _message;
		private string _messageColor;
		private static Constants _constants = new Constants();
		private readonly string _excelFilePath = Path.Combine(Environment.GetFolderPath(
			Environment.SpecialFolder.ApplicationData),
			_constants.SDLCommunity,
			_constants.SDLMachineTranslationCloud,
			"MTLanguageCodes.xlsx");

		private int _lastExcelRowNumber;
		private ExcelParser _excelParser = new ExcelParser();
		private string _query;

		private ICommand _updateCellCommand;
		private ICommand _printCommand;

		public MTCodesViewModel(List<ExcelSheet> excelSheetResults)
		{
			MTCodes = new ObservableCollection<MTCodeModel>();
			MTCodes = MapExcelCodes(excelSheetResults);

			Timer.Tick += StartSearch;
			PropertyChanged += StartSearchTimer;
		}

		public Timer Timer { get; } = new Timer { Interval = 500 };

		public ObservableCollection<MTCodeModel> MTCodes
		{
			get => _mtCodes;
			set
			{
				_mtCodes = value;
				OnPropertyChanged(nameof(MTCodes));
			}
		}

		public MTCodeModel SelectedMTCode
		{
			get => _selectedMTCode;
			set
			{
				_selectedMTCode = value;
				OnPropertyChanged(nameof(SelectedMTCode));
			}
		}

		public string Message
		{
			get => _message;
			set
			{
				_message = value;
				OnPropertyChanged(nameof(Message));
			}
		}

		public string MessageColor
		{
			get => _messageColor;
			set
			{
				_messageColor = value;
				OnPropertyChanged(nameof(MessageColor));
			}
		}

		public string Query
		{
			get => _query;
			set
			{
				_query = value;
				OnPropertyChanged(nameof(Query));
			}
		}

		public ICommand UpdateCellCommand => _updateCellCommand ?? (_updateCellCommand = new RelayCommand(UpdateMTCode));
		public ICommand PrintCommand => _printCommand ?? (_printCommand = new RelayCommand<System.Windows.Forms.DataGrid>(Print));

		public void SearchLanguages(string languageName)
		{
			var collectionViewSource = CollectionViewSource.GetDefaultView(MTCodes);

			if (!string.IsNullOrEmpty(languageName))
			{
				collectionViewSource.Filter = p =>
				{
					var mtCodeModel = p as MTCodeModel;
					return mtCodeModel != null && mtCodeModel.Language.ToLower().Contains(languageName.ToLower());
				};
				SelectedMTCode = collectionViewSource.CurrentItem as MTCodeModel;
			}
			else
			{
				collectionViewSource.Filter = null;
			}
		}

		public void Print(System.Windows.Forms.DataGrid dataGrid)
		{
			// update solution to identify if the records are searched (maybe based on MTCodes number if it's < total MTCodes number from excel).
			//If only a few records are returned after search, then use the below print logic, otherwise use the Interop logic to print entire local excel file.
			//var printDlg = new PrintDialog();
			//if (printDlg.ShowDialog() == true)
			//{
			//	var capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);

			//	double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / dataGrid.ActualWidth,
			//							capabilities.PageImageableArea.ExtentHeight / dataGrid.ActualHeight);

			//	var oldTransform = dataGrid.LayoutTransform;

			//	dataGrid.LayoutTransform = new ScaleTransform(scale, scale);

			//	var oldSize = new Size(dataGrid.ActualWidth, dataGrid.ActualHeight);
			//	var sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
			//	dataGrid.Measure(sz);
			//	((UIElement)dataGrid).Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight),
			//		sz));

			//	printDlg.PrintVisual(dataGrid, _constants.PrintMTCodes);
			//	dataGrid.LayoutTransform = oldTransform;
			//	dataGrid.Measure(oldSize);

			//	((UIElement)dataGrid).Arrange(new Rect(new Point(0, 0),
			//		oldSize));
			//}

			var printers = PrinterSettings.InstalledPrinters;
			int printerIndex = 0;

			var printDlg = new System.Windows.Controls.PrintDialog();
			if (printDlg.ShowDialog() == true)

			{
				var excelApp = new Excel.Application();

				// Open the Workbook
				var wb = excelApp.Workbooks.Open(
					_excelFilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
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

				wb.Close(false, _excelFilePath, Type.Missing);
				Marshal.FinalReleaseComObject(wb);

				excelApp.Quit();
				Marshal.FinalReleaseComObject(excelApp);
			}
		}

		/// <summary>
		/// Update the MTCode (main) / MTCode (locale) inside the local excel file after datagrid cell was edited
		/// </summary>
		/// <param name="parameter">parameter</param>
		public void UpdateMTCode(object parameter)
		{
			Message = string.Empty;
			try
			{
				if (SelectedMTCode != null)
				{
					var mtCodeExcel = CreateMTCodeExcelModel();
					_excelParser.UpdateMTCodeExcel(mtCodeExcel);

					SetMessage(_constants.Green, _constants.SuccessfullyUpdatedMessage);
				}				
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{_constants.AddMTCode} {ex.Message}\n {ex.StackTrace}");
				SetMessage(_constants.Red, ex.Message);
			}
		}

		private void StartSearchTimer(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals(nameof(Query)))
			{
				Timer.Stop();
				Timer.Start();
			}
		}
		private void StartSearch(object sender, EventArgs e)
		{
			Timer.Stop();
			SearchLanguages(Query);
		}

		/// <summary>
		/// Set the message text and color
		/// </summary>
		/// <param name="messageColor">message color</param>
		/// <param name="message">message text</param>
		private void SetMessage(string messageColor, string message)
		{
			MessageColor = messageColor;
			Message = message;
		}

		/// <summary>
		/// Create MTCodeExcel model with the needed values to update the Excel file.
		/// Region,Language,TradosCode and column numbers are used to add/update the row inside the Excel local file, in case the information for the selected Studio Language does not exists.
		/// </summary>
		/// <returns>MTCodeExcel object</returns>
		private MTCodeExcel CreateMTCodeExcelModel()
		{
			return new MTCodeExcel
			{
				ExcelPath = _excelFilePath,
				ExcelSheet = _constants.ExcelSheet,
				LocaleValue = SelectedMTCode.MTCodeLocale,
				LocaleColumnNumber = SelectedMTCode.MTCodeLocaleColumnNo,
				MainValue = SelectedMTCode.MTCodeMain,
				MainColumnNumber = SelectedMTCode.MTCodeMainColumnNo,
				Language = SelectedMTCode.Language,
				LanguageColumnNumber = SelectedMTCode.LanguageColumnNo,
				Region = SelectedMTCode.Region,
				RegionColumnNumber = SelectedMTCode.RegionColumnNo,
				TradosCode = SelectedMTCode.TradosCode,
				TradosCodeColumnNumber = SelectedMTCode.TradosCodeColumnNo,
				SheetRowNumber = SelectedMTCode.RowNumber
			};
		}

		/// <summary>
		/// Map values from Excel to MTCodes collection based on the avaialble Languages from Studio
		/// </summary>
		/// <param name="excelSheetResults">results from excel's sheet</param>
		/// <returns>MTCodes collection</returns>
		private ObservableCollection<MTCodeModel> MapExcelCodes(List<ExcelSheet> excelSheetResults)
		{
			// Remove the first row which corresponds to columns name
			excelSheetResults.RemoveRange(0, 1);
			var excelValues = new List<MTCodeModel>();

			// The row numbering starts with 1, because the first row in Excel corresponds to column names
			var rowNumber = 1; 
			foreach (var excelSheetRow in excelSheetResults)
			{
				rowNumber++;
				var mtCodeModel = new MTCodeModel
				{
					Language = excelSheetRow.RowValues[0]?.ToString(),
					Region = excelSheetRow.RowValues[1]?.ToString(),
					TradosCode = excelSheetRow.RowValues[2]?.ToString(),
					MTCodeMain = excelSheetRow.RowValues[3]?.ToString(),
					MTCodeLocale = excelSheetRow.RowValues[4]?.ToString(),
					LanguageColumnNo = 1,
					RegionColumnNo = 2,
					TradosCodeColumnNo = 3,
					MTCodeMainColumnNo = 4,
					MTCodeLocaleColumnNo = 5,					
					RowNumber = rowNumber
				};
				excelValues.Add(mtCodeModel);
			}
			_lastExcelRowNumber = excelValues.Count + 1;
			var studioLanguages = Language.GetAllLanguages();

			foreach (var item in studioLanguages)
			{
				var languageInfo = Utils.FormatLanguageName(item.DisplayName);
				
				// If the language information already exists in Excel, add it to the MTCodes collection, otherwise add the specific MTCode info as new model
				var mtCodeModel = excelValues.FirstOrDefault(e => e.Language.Equals(languageInfo[0]) && e.TradosCode.Equals(item.IsoAbbreviation));
				if(mtCodeModel != null)
				{
					MTCodes.Add(mtCodeModel);
				}
				else
				{
					MTCodes.Add(new MTCodeModel
					{
						Language = languageInfo[0],
						Region = languageInfo[1],
						TradosCode = item.IsoAbbreviation,
						MTCodeMain = string.Empty,
						MTCodeLocale = string.Empty,
						LanguageColumnNo = 1,
						RegionColumnNo = 2,
						TradosCodeColumnNo = 3,
						MTCodeMainColumnNo = 4,
						MTCodeLocaleColumnNo = 5,						
						RowNumber = _lastExcelRowNumber// use a new number based on the last existing RowNumber in Excel (the new MTCodeModel will be added inside excel as new rows)
					});
					_lastExcelRowNumber++;
				}
			}
			return MTCodes;
		}
	}
}