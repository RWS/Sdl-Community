using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LanguageMappingProvider.Database;
using LanguageMappingProvider.Database.Interface;
using LanguageMappingProvider.Model;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Extensions;
using Sdl.Core.Globalization;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class MTCodesViewModel : BaseModel
	{
		private readonly ILanguageMappingDatabase _database;

		private ObservableCollection<MappedLanguage> _filteredMappedLanguages;
		private ObservableCollection<MappedLanguage> _mappedLanguages;
		private MappedLanguage _selectedMappedLanguage;

		private string _filter;
		private string _languagesCountMessage;

		private ICommand _applyChangesCommand;
		private ICommand _cancelChangesCommand;
		private ICommand _resetToDefaultCommand;
		private ICommand _clearCommand;

		public MTCodesViewModel()
		{
			_database = new LanguageMappingDatabase("testlw", DatabaseExtensions.GetLWSupportedLanguages());
			RetrieveMappedLanguagesFromDatabase();
			FilteredMappedLanguages = MappedLanguages;
			PropertyChanged += FilterPropertyChangedHandler;
		}

		public ObservableCollection<MappedLanguage> MappedLanguages
		{
			get => _mappedLanguages;
			set
			{
				if (_mappedLanguages == value) return;
				_mappedLanguages = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<MappedLanguage> FilteredMappedLanguages
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

		public MappedLanguage SelectedMappedLanguage
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

		public ICommand ApplyChangesCommand => _applyChangesCommand ??= new RelayCommand(ApplyChanges, CanApplyChanges);
		public ICommand CancelChangesCommand => _cancelChangesCommand ??= new RelayCommand(CancelChanges);
		public ICommand ResetToDefaultCommand => _resetToDefaultCommand ??= new RelayCommand(ResetToDefault);
		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		private void RetrieveMappedLanguagesFromDatabase()
		{
			var mappedLanguages = _database.GetMappedLanguages();
			MappedLanguages = new ObservableCollection<MappedLanguage>(mappedLanguages);
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
				FilteredMappedLanguages = new ObservableCollection<MappedLanguage>(MappedLanguages);
				return;
			}

			var filterLower = Filter.ToLower();
			var filteredContent = MappedLanguages.Where(
				language => IsMatchingOnFilter(language.Name, filterLower)
						 || IsMatchingOnFilter(language.Region, filterLower)
						 || IsMatchingOnFilter(language.TradosCode, filterLower)
						 || IsMatchingOnFilter(language.LanguageCode, filterLower));

			FilteredMappedLanguages = new ObservableCollection<MappedLanguage>(filteredContent);
		}

		private bool IsMatchingOnFilter(string field, string filter)
		{
			return !string.IsNullOrEmpty(field) && field.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
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
				case "filter":
					Filter = string.Empty;
					return;

				default:
					break;
			}
		}

		private void ApplyChanges(object parameter)
		{
			_database.UpdateAll(MappedLanguages);
			RetrieveMappedLanguagesFromDatabase();
			ShutDownApp();
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