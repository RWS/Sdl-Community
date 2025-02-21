using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using InterpretBank.Commands;

namespace InterpretBank.SettingsService.Model
{
    public class LanguageModelsListBoxItem : InterpretBank.Model.NotifyChangeModel, IDataErrorInfo
	{
		private List<LanguageModel> _languageModels;
		private ICommand _resetCommand;
		private int _selectedIndex = -1;
		public string Error { get; set; }
		private string _errorMessage;

		public bool IsEditable { get; set; } = true;

		public List<LanguageModel> LanguageModels
		{
			get => _languageModels;
			set => SetField(ref _languageModels, value);
		}

		public ICommand ResetCommand => _resetCommand ??= new RelayCommand(Reset, _ => IsEditable);

		public int SelectedIndex
		{
			get => _selectedIndex;
			set => SetField(ref _selectedIndex, value);
		}

		public string this[string columnName]
		{
			get => _errorMessage;
			set => _errorMessage = value;
		}

		private void Reset(object obj)
		{
			SelectedIndex = 0;
		}
	}
}