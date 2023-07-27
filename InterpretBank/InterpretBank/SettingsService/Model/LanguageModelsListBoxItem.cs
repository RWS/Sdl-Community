using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using InterpretBank.Commands;

namespace InterpretBank.SettingsService.Model
{
	public class LanguageModelsListBoxItem : ViewModelBase.ViewModel, IDataErrorInfo
	{
		private List<LanguageModel> _languageModels;
		private ICommand _restoreCommand;
		private int _selectedIndex = -1;
		public string Error { get; set; }
		private string _errorMessage;

		public bool IsEditable { get; set; } = true;

		public List<LanguageModel> LanguageModels
		{
			get => _languageModels;
			set => SetField(ref _languageModels, value);
		}

		public ICommand RestoreCommand => _restoreCommand ??= new RelayCommand(Restore);

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

		private void Restore(object obj)
		{
			SelectedIndex = 0;
		}
	}
}