using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.Toolkit.LanguagePlatform.ExcelParser;
using Sdl.Community.Toolkit.LanguagePlatform.Models;
using Sdl.Core.Globalization;
using Controls = System.Windows.Controls;

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

		private readonly string _filteredExcelFilePath = Path.Combine(Environment.GetFolderPath(
			Environment.SpecialFolder.ApplicationData),
			_constants.SDLCommunity,
			_constants.SDLMachineTranslationCloud,
			"FilteredMTLanguageCodes.xlsx");

		private int _lastExcelRowNumber;
		private ExcelParser _excelParser = new ExcelParser();
		private List<MTCodeModel> _filteredMTCodes;
		private string _query;
		private int _queriedCodesNumber;
		private PrintService _printService;
		private ICommand _updateCellCommand;
		private ICommand _printCommand;

		public MTCodesViewModel(List<ExcelSheet> excelSheetResults)
		{
			MTCodes = new ObservableCollection<MTCodeModel>();
			MTCodes = MapExcelCodes(excelSheetResults);
			_printService = new PrintService();

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
		public ICommand PrintCommand => _printCommand ?? (_printCommand = new RelayCommand<Controls.DataGrid>(Print));

		/// <summary>
		/// Search records from grid using languageName as query input
		/// </summary>
		/// <param name="languageName">languageName</param>
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
			_queriedCodesNumber = collectionViewSource.Cast<object>().Count();
			_filteredMTCodes = collectionViewSource.Cast<MTCodeModel>().ToList();
		}

		/// <summary>
		/// Print the searched MTCodes records which are exported to new Excel file/entire MTCodes from the original Excel file
		/// </summary>
		/// <param name="dataGrid">MTCodes dataGrid</param>
		public void Print(Controls.DataGrid dataGrid)
		{
			// If the number of queries are less than 500 (aprox number of Studio languages), it means that user searched for a specific language
			// and those searched records will be printed
			if (_queriedCodesNumber < 500)
			{
				_printService.CreateExcelFile(_filteredExcelFilePath);

				// Excel indexing starts with 1, we are starting row numbering to add values with 2, because index 1 corresponds to column names
				var excelRow = 2;
				foreach (var item in _filteredMTCodes)
				{
					var excelModel = CreateMTCodeExcelModel(item, excelRow++, _filteredExcelFilePath);
					_excelParser.UpdateMTCodeExcel(excelModel);
				}
				_printService.PrintFile(_filteredExcelFilePath);
			}
			else
			{
				// Print all records which exists in original excel file
				_printService.PrintFile(_excelFilePath);
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
					var mtCodeExcel = CreateMTCodeExcelModel(SelectedMTCode, SelectedMTCode.RowNumber, _excelFilePath);
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
		private MTCodeExcel CreateMTCodeExcelModel(MTCodeModel mtCodeModel, int sheetRowNumber, string filePath)
		{
			return new MTCodeExcel
			{
				ExcelPath = filePath,
				ExcelSheet = _constants.ExcelSheet,
				LocaleValue = mtCodeModel.MTCodeLocale,
				LocaleColumnNumber = mtCodeModel.MTCodeLocaleColumnNo,
				MainValue = mtCodeModel.MTCodeMain,
				MainColumnNumber = mtCodeModel.MTCodeMainColumnNo,
				Language = mtCodeModel.Language,
				LanguageColumnNumber = mtCodeModel.LanguageColumnNo,
				Region = mtCodeModel.Region,
				RegionColumnNumber = mtCodeModel.RegionColumnNo,
				TradosCode = mtCodeModel.TradosCode,
				TradosCodeColumnNumber = mtCodeModel.TradosCodeColumnNo,
				SheetRowNumber = sheetRowNumber
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
				if (mtCodeModel != null)
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