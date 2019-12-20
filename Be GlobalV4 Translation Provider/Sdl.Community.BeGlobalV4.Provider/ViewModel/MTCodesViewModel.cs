using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.Toolkit.LanguagePlatform.ExcelParser;
using Sdl.Community.Toolkit.LanguagePlatform.Models;
using Sdl.Core.Globalization;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class MTCodesViewModel : BaseViewModel
	{
		private MTCodeModel _selectedMTCode;
		private ObservableCollection<MTCodeModel> _mtCodes;
		private string _message;
		private string _messageColor;
		private readonly string _excelFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.SDLCommunity, Constants.SDLMachineTranslationCloud, "MTLanguageCodes.xlsx");
		private int _lastExcelRowNumber;

		private ICommand _addMTCodeCommand;
		private ICommand _removeMTCodeCommand;

		public static readonly Log Log = Log.Instance;

		public MTCodesViewModel(List<ExcelSheet> excelSheetResults)
		{
			MTCodes = new ObservableCollection<MTCodeModel>();
			MTCodes = MapExcelCodes(excelSheetResults);
		}

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

		public ICommand AddMTCodeCommand => _addMTCodeCommand ?? (_addMTCodeCommand = new RelayCommand(AddMTCode));
		public ICommand RemoveMTCodeCommand => _removeMTCodeCommand ?? (_removeMTCodeCommand = new RelayCommand(RemoveMTCode));

		/// <summary>
		/// Add the MTCodeLocale to the MTLanguageCodes.xlsx Excel file
		/// </summary>
		/// <param name="parameter">parameter</param>
		public void AddMTCode(object parameter)
		{
			Message = string.Empty;
			try
			{
				if (SelectedMTCode != null && !string.IsNullOrEmpty(SelectedMTCode.MTCodeLocale))
				{
					var mtCodeExcel = CreateMTCodeExcelModel(SelectedMTCode.MTCodeLocale);
					ExcelParser.UpdateMTCodeExcel(mtCodeExcel);

					SetMessage(Constants.Green, Constants.SuccessfullyAddedMessage);
				}
				else
				{
					SetMessage(Constants.Red, Constants.MTCodeEmptyValidation);
				}
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.AddMTCode} {ex.Message}\n {ex.StackTrace}");
				SetMessage(Constants.Red, ex.Message);
			}
		}

		/// <summary>
		/// Delete the selected MTCodeLocale from  the MTLanguageCodes.xlsx Excel file
		/// </summary>
		/// <param name="paramter">paramter</param>
		public void RemoveMTCode(object paramter)
		{
			Message = string.Empty;
			try
			{
				if (SelectedMTCode != null && !string.IsNullOrEmpty(SelectedMTCode.MTCodeLocale))
				{
					var mtCodeExcel = CreateMTCodeExcelModel(string.Empty);
					ExcelParser.UpdateMTCodeExcel(mtCodeExcel);

					// Refresh the grid value after removing it from Excel file
					var mtCodeUpdated = MTCodes.FirstOrDefault(m => m.Language.Equals(SelectedMTCode.Language) && m.Region.Equals(SelectedMTCode.Region));
					if (mtCodeUpdated != null)
					{
						mtCodeUpdated.MTCodeLocale = string.Empty;
					}
					SetMessage(Constants.Green, Constants.SuccessfullyRemovedMessage);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.RemoveMTCode} {ex.Message}\n {ex.StackTrace}");
				SetMessage(Constants.Red, ex.Message);			
			}				
		}

		private void SetMessage(string messageColor, string message)
		{
			MessageColor = messageColor;
			Message = message;
		}

		/// <summary>
		/// Create MTCodeExcel model with the needed values to update the Excel file 
		/// </summary>
		/// <param name="mtCodeLocaleValue">MTCodeLocale value</param>
		/// <returns>MTCodeExcel object</returns>
		private MTCodeExcel CreateMTCodeExcelModel(string mtCodeLocaleValue)
		{
			return new MTCodeExcel
			{
				ExcelPath = _excelFilePath,
				ExcelSheet = Constants.ExcelSheet,
				LocaleValue = mtCodeLocaleValue,
				LocaleColumnNumber = SelectedMTCode.MTCodeLocaleColumnNo,
				SheetRowNumber = SelectedMTCode.RowNumber
			};
		}

		/// <summary>
		/// Map values from Excel to MTCodes collection based on avaialble Languages from Studio
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