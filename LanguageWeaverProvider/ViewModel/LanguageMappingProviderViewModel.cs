using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LanguageMappingProvider.Database.Interface;
using LanguageMappingProvider.Model;
using LanguageWeaverProvider.Command;
using static LanguageWeaverProvider.ViewModel.PairMappingViewModel;

namespace LanguageWeaverProvider.ViewModel
{
	public class LanguageMappingProviderViewModel : BaseViewModel
	{
		readonly ILanguageMappingDatabase _languageMappingDatabase;

		ObservableCollection<LanguageMapping> _filteredMappedLanguages;
		ObservableCollection<LanguageMapping> _mappedLanguages;
		LanguageMapping _selectedMappedLanguage;

		string _loadingAction;
		string _filter;

		ICommand _applyChangesCommand;
		ICommand _closeLanguageMappingProviderCommand;
		ICommand _resetToDefaultCommand;
		ICommand _clearCommand;

		public LanguageMappingProviderViewModel(ILanguageMappingDatabase languageMappingDatabase)
		{
			_languageMappingDatabase = languageMappingDatabase;
			RetrieveMappedLanguagesFromDatabase();
			FilteredMappedLanguages = MappedLanguages;
			PropertyChanged += FilterPropertyChangedHandler;
		}

		public ObservableCollection<LanguageMapping> MappedLanguages
		{
			get => _mappedLanguages;
			set
			{
				_mappedLanguages = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<LanguageMapping> FilteredMappedLanguages
		{
			get => _filteredMappedLanguages;
			set
			{
				_filteredMappedLanguages = value;
				OnPropertyChanged();
			}
		}

		public LanguageMapping SelectedMappedLanguage
		{
			get => _selectedMappedLanguage;
			set
			{
				_selectedMappedLanguage = value;
				OnPropertyChanged();
			}
		}

		public string Filter
		{
			get => _filter ??= string.Empty;
			set
			{
				_filter = value?.ToLower();
				OnPropertyChanged();
				ApplyFilter();
			}
		}

		public string LoadingAction
		{
			get => _loadingAction;
			set
			{
				_loadingAction = value;
				OnPropertyChanged();
			}
		}

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);
		public ICommand ResetToDefaultCommand => _resetToDefaultCommand ??= new RelayCommand(ResetToDefault);
		public ICommand ApplyChangesCommand => _applyChangesCommand ??= new RelayCommand(ApplyChanges, CanApplyChanges);
		public ICommand CloseLanguageMappingProviderCommand => _closeLanguageMappingProviderCommand ??= new RelayCommand(CloseLanguageMappingProvider);

		public event EventHandler LanguageMappingUpdated;

		public event CloseWindowEventRaiser CloseEventRaised;

		private void ApplyFilter()
		{
			if (string.IsNullOrWhiteSpace(Filter))
			{
				FilteredMappedLanguages = new ObservableCollection<LanguageMapping>(MappedLanguages);
				return;
			}

			var filterLower = Filter.ToLower();
			var filteredContent = MappedLanguages.Where(language =>
				(!string.IsNullOrEmpty(language.Name) && language.Name.IndexOf(filterLower, StringComparison.OrdinalIgnoreCase) >= 0)
			 || (!string.IsNullOrEmpty(language.Region) && language.Region.IndexOf(filterLower, StringComparison.OrdinalIgnoreCase) >= 0)
			 || (!string.IsNullOrEmpty(language.TradosCode) && language.TradosCode.IndexOf(filterLower, StringComparison.OrdinalIgnoreCase) >= 0)
			 || (!string.IsNullOrEmpty(language.LanguageCode) && language.LanguageCode.IndexOf(filterLower, StringComparison.OrdinalIgnoreCase) >= 0));

			FilteredMappedLanguages = new ObservableCollection<LanguageMapping>(filteredContent);
		}

		private void RetrieveMappedLanguagesFromDatabase()
		{
			var mappedLanguages = _languageMappingDatabase.GetMappedLanguages();
			var newMappedLanguages = mappedLanguages.Select(pair => new LanguageMapping
			{
				Index = pair.Index,
				Name = pair.Name,
				Region = pair.Region,
				TradosCode = pair.TradosCode,
				LanguageCode = pair.LanguageCode
			});

			MappedLanguages = new ObservableCollection<LanguageMapping>(newMappedLanguages);
			ApplyFilter();
		}

		private void ApplyChanges(object parameter)
		{
			LoadingAction = PluginResources.Loading_LMP_Apply;
			_languageMappingDatabase.UpdateAll(MappedLanguages);
			RetrieveMappedLanguagesFromDatabase();
			LanguageMappingUpdated?.Invoke(this, EventArgs.Empty);
			LoadingAction = null;
		}

		private bool CanApplyChanges(object parameter)
		{
			return _languageMappingDatabase.HasMappedLanguagesChanged(MappedLanguages);
		}

		private async void ResetToDefault(object parameter)
		{
			LoadingAction = PluginResources.Loading_LMP_RestoreDefaults;
			if (ExecuteAction(PluginResources.LMPViewModel_ResetWarning, PluginResources.LMPViewModel_ResetTitle))
			{
				await Task.Delay(50);
				_languageMappingDatabase.ResetToDefault();
				RetrieveMappedLanguagesFromDatabase();
				LanguageMappingUpdated?.Invoke(this, EventArgs.Empty);
				await Task.Delay(50);
			}
			LoadingAction = null;
		}

		private bool ExecuteAction(string message, string title)
		{
			var dialogResult = MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
			return dialogResult == MessageBoxResult.OK;
		}

		private void CloseLanguageMappingProvider(object parameter)
		{
			CloseEventRaised?.Invoke();
		}

		private void Clear(object parameter)
		{
			switch (parameter as string)
			{
				case nameof(Filter):
					Filter = string.Empty;
					return;

				default:
					break;
			}
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