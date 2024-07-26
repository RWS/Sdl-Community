using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LanguageMappingProvider.Database;
using LanguageMappingProvider.Database.Interface;
using LanguageMappingProvider.Model;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Extensions;
using MicrosoftTranslatorProvider.Interfaces;
using static MicrosoftTranslatorProvider.ViewModel.MainWindowViewModel;
using MessageBox = System.Windows.MessageBox;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class LanguageMappingProviderViewModel : Model.BaseModel
    {
		private readonly ILanguageMappingDatabase _database;
		private readonly ITranslationOptions _translationOptions;

		private ObservableCollection<LanguageMapping> _filteredMappedLanguages;
		private ObservableCollection<LanguageMapping> _mappedLanguages;
		private LanguageMapping _selectedMappedLanguage;

		private bool _dialogResult;
		private bool _canResetToDefaults;

		private string _filter;
		private string _languagesCountMessage;

		private ICommand _applyChangesCommand;
		private ICommand _cancelChangesCommand;
		private ICommand _resetToDefaultCommand;
		private ICommand _clearCommand;

		public LanguageMappingProviderViewModel(ITranslationOptions translationOptions, bool editProvider)
		{
			_translationOptions = translationOptions;
			CanResetToDefaults = editProvider;
			var defaultMapping = CanResetToDefaults ? DatabaseExtensions.GetDefaultMapping(_translationOptions) : null;
			_database = new LanguageMappingDatabase(Constants.DatabaseName, defaultMapping);
			RetrieveMappedLanguagesFromDatabase();
			FilteredMappedLanguages = MappedLanguages;
			PropertyChanged += FilterPropertyChangedHandler;
		}

		public ObservableCollection<LanguageMapping> MappedLanguages
		{
			get => _mappedLanguages;
			set
			{
				if (_mappedLanguages == value) return;
				_mappedLanguages = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<LanguageMapping> FilteredMappedLanguages
		{
			get => _filteredMappedLanguages;
			set
			{
				if (_filteredMappedLanguages == value) return;
				_filteredMappedLanguages = value;
				OnPropertyChanged();
				RefreshLanguagesCountMessage();
			}
		}

		public LanguageMapping SelectedMappedLanguage
		{
			get => _selectedMappedLanguage;
			set
			{
				if (_selectedMappedLanguage == value) return;
				_selectedMappedLanguage = value;
				OnPropertyChanged();
			}
		}

		public string Filter
		{
			get => _filter;
			set
			{
				if (_filter == value) return;
				_filter = value?.ToLower();
				OnPropertyChanged();
				ApplyFilter();
				RefreshLanguagesCountMessage();
			}
		}

		public string LanguagesCountMessage
		{
			get => _languagesCountMessage;
			set
			{
				if (_languagesCountMessage == value) return;
				_languagesCountMessage = value;
				OnPropertyChanged();
			}
		}

		public bool DialogResult
		{
			get => _dialogResult;
			set
			{
				if (_dialogResult == value) return;
				_dialogResult = value;
				OnPropertyChanged(nameof(DialogResult));
			}
		}

		public bool CanResetToDefaults
		{
			get => _canResetToDefaults;
			set
			{
				if (_canResetToDefaults == value) return;
				_canResetToDefaults = value;
				OnPropertyChanged();
			}
		}

		public event CloseWindowEventRaiser CloseEventRaised;

		public ICommand ApplyChangesCommand => _applyChangesCommand ??= new RelayCommand(ApplyChanges, CanApplyChanges);
		public ICommand CancelChangesCommand => _cancelChangesCommand ??= new RelayCommand(CancelChanges);
		public ICommand ResetToDefaultCommand => _resetToDefaultCommand ??= new RelayCommand(ResetToDefault);
		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		private void RetrieveMappedLanguagesFromDatabase()
		{
			var mappedLanguages = _database.GetMappedLanguages();
			var newMappedLanguages = mappedLanguages.Select(pair => new LanguageMapping
			{
				Index = pair.Index,
				Name = pair.Name,
				Region = pair.Region,
				TradosCode = pair.TradosCode,
				LanguageCode = pair.LanguageCode
			});

			MappedLanguages = new ObservableCollection<LanguageMapping>(newMappedLanguages);
			FilteredMappedLanguages = MappedLanguages;
			Filter = string.Empty;
		}

		private void RefreshLanguagesCountMessage()
		{
			var totalLanguagesCount = MappedLanguages.Count;
			var filteredLanguagesCount = FilteredMappedLanguages.Count;
			LanguagesCountMessage = string.IsNullOrWhiteSpace(Filter)
								  ? $"Total languages: {totalLanguagesCount}"
								  : $"Total languages: {totalLanguagesCount}; Filtered: {filteredLanguagesCount}";
		}

		private void ApplyFilter()
		{
			if (string.IsNullOrWhiteSpace(Filter))
			{
				FilteredMappedLanguages = new ObservableCollection<LanguageMapping>(MappedLanguages);
				return;
			}

			var filterLower = Filter.ToLower();
			var filteredContent = MappedLanguages.Where(language =>
				(!string.IsNullOrEmpty(language.Name) && language.Name.IndexOf(filterLower, StringComparison.OrdinalIgnoreCase) >= 0) ||
				(!string.IsNullOrEmpty(language.Region) && language.Region.IndexOf(filterLower, StringComparison.OrdinalIgnoreCase) >= 0) ||
				(!string.IsNullOrEmpty(language.TradosCode) && language.TradosCode.IndexOf(filterLower, StringComparison.OrdinalIgnoreCase) >= 0) ||
				(!string.IsNullOrEmpty(language.LanguageCode) && language.LanguageCode.IndexOf(filterLower, StringComparison.OrdinalIgnoreCase) >= 0));

			FilteredMappedLanguages = new ObservableCollection<LanguageMapping>(filteredContent);
		}

		private void ResetToDefault(object parameter)
		{
			if (ExecuteAction("Warning: Resetting to default values!\nAll changes will be lost and the database will be restored to its original state.\n\nThis action cannot be undone.", "Reset to default"))
			{
				_database.ResetToDefault();
				RetrieveMappedLanguagesFromDatabase();
			}
		}

		private bool ExecuteAction(string message, string title)
		{
			var dialogResult = MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
			return dialogResult == MessageBoxResult.OK;
		}

		private void Clear(object parameter)
		{
			if (parameter is not string parameterString)
			{
				return;
			}

			switch (parameterString)
			{
				case nameof(Filter):
					Filter = string.Empty;
					return;

				default:
					break;
			}
		}

		private void ApplyChanges(object parameter)
		{
			var originalFilter = Filter;
			_database.UpdateAll(MappedLanguages);
			RetrieveMappedLanguagesFromDatabase();
			Filter = originalFilter;
		}

		private bool CanApplyChanges(object parameter)
		{
			return _database.HasMappedLanguagesChanged(MappedLanguages);
		}

		private void CancelChanges(object parameter)
		{
			ShutDownApp();
		}

		private void ShutDownApp()
		{
			DialogResult = true;
			CloseEventRaised?.Invoke();
		}

		private void FilterPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Filter))
			{
				ApplyFilter();
			}
		}
	}
}