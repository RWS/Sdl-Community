using System.Collections.Generic;

namespace InterpretBank.SettingsService.Model
{
	public class LanguageModelsListBoxItem : ViewModel.ViewModel
	{
		private List<LanguageModel> _languageModels;
		private int _selectedIndex = -1;

		public List<LanguageModel> LanguageModels
		{
			get => _languageModels;
			set => SetField(ref _languageModels, value);
		}

		public int SelectedIndex
		{
			get => _selectedIndex;
			set => SetField(ref _selectedIndex, value);
		}

		public bool IsEditable { get; set; } = true;
	}
}