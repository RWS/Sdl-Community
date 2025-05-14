using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GoogleCloudTranslationProvider.Commands;
using GoogleCloudTranslationProvider.Extensions;
using GoogleCloudTranslationProvider.Interfaces;
using LanguageMappingProvider;
using static GoogleCloudTranslationProvider.ViewModels.MainWindowViewModel;
using MessageBox = System.Windows.MessageBox;

namespace GoogleCloudTranslationProvider.ViewModel
{
	public class LanguageMappingProviderViewModel : BaseViewModel
	{
		private readonly ILanguageMappingDatabase _database;
		private readonly ITranslationOptions _translationOptions;

		private ObservableCollection<LanguageMapping> _filteredMappedLanguages;
		private ObservableCollection<LanguageMapping> _mappedLanguages;
		private LanguageMapping _selectedMappedLanguage;

		private string _filter;
		private string _languagesCountMessage;

		public LanguageMappingProviderViewModel(ITranslationOptions translationOptions, bool canResetToDefaults)
		{
			_translationOptions = translationOptions;
			CanResetToDefaults = canResetToDefaults;
			var defaultMapping = canResetToDefaults ? DatabaseExtensions.GetGoogleDefaultMapping(translationOptions) : null;
			_database = new LanguageMappingDatabase(GetDatabaseName(), defaultMapping);
			RetrieveMappedLanguagesFromDatabase();
			FilteredMappedLanguages = MappedLanguages;
			PropertyChanged += FilterPropertyChangedHandler;
			InitializeCommands();
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

		public bool CanResetToDefaults { get; set; }

		public event EventHandler LanguageMappingUpdated;

		public event CloseWindowEventRaiser CloseEventRaised;

		public ICommand ClearCommand { get; private set; }
		public ICommand ApplyChangesCommand { get; private set; }
		public ICommand OpenHyperlinkCommand { get; private set; }
		public ICommand ResetToDefaultCommand { get; private set; }
		public ICommand CloseApplicationCommand { get; private set; }

		private void InitializeCommands()
		{
			ClearCommand = new RelayCommand(Clear);
			ApplyChangesCommand = new RelayCommand(ApplyChanges, CanApplyChanges);
			OpenHyperlinkCommand = new RelayCommand(OpenHyperlink);
			ResetToDefaultCommand = new RelayCommand(ResetToDefault, (predicate) => CanResetToDefaults);
			CloseApplicationCommand = new RelayCommand(CloseApplication);
		}

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
			ApplyFilter();
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

		private void RefreshLanguagesCountMessage()
		{
			var totalLanguagesCount = MappedLanguages.Count;
			var filteredLanguagesCount = FilteredMappedLanguages.Count;
			LanguagesCountMessage = string.IsNullOrWhiteSpace(Filter)
								  ? $"Total languages: {totalLanguagesCount}"
								  : $"Total languages: {totalLanguagesCount}; Filtered: {filteredLanguagesCount}";
		}

		private void ApplyChanges(object parameter)
		{
			_database.UpdateAll(MappedLanguages);
			RetrieveMappedLanguagesFromDatabase();
			LanguageMappingUpdated?.Invoke(this, EventArgs.Empty);
		}

		private bool CanApplyChanges(object parameter)
		{
			return _database.HasMappedLanguagesChanged(MappedLanguages);
		}

		private void OpenHyperlink(object parameter)
		{
			if (parameter is not string uri
			 || string.IsNullOrEmpty(uri))
			{
				return;
			}

			Process.Start(uri);
		}

		private void ResetToDefault(object parameter)
		{
			if (ExecuteAction(PluginResources.LMP_ResetToDefaults_Warning, PluginResources.LMP_Button_Reset))
			{
				_database.ResetToDefault();
				RetrieveMappedLanguagesFromDatabase();
				LanguageMappingUpdated?.Invoke(this, EventArgs.Empty);
			}
		}

		private void CloseApplication(object parameter)
		{
			CloseEventRaised?.Invoke();
		}

		private bool ExecuteAction(string message, string title)
		{
			var dialogResult = MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
			return dialogResult == MessageBoxResult.OK;
		}

		private string GetDatabaseName()
		{
			return _translationOptions.SelectedGoogleVersion == ApiVersion.V2
				 ? PluginResources.Database_PluginName_V2
				 : PluginResources.Database_PluginName_V3;
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