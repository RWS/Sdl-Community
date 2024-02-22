﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LanguageMappingProvider.Database.Interface;
using LanguageMappingProvider.Model;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.ViewModel;
using static LanguageWeaverProvider.ViewModel.PairMappingViewModel;

namespace LanguageWeaverProvider.LanguageMappingProvider.ViewModel
{
	public class LanguageMappingProviderViewModel : BaseViewModel
	{
		readonly ILanguageMappingDatabase _languageMappingDatabase;

		IList<LanguageMapping> _filteredMappedLanguages;
		IList<LanguageMapping> _mappedLanguages;
		LanguageMapping _selectedMappedLanguage;

		string _loadingAction;
		string _filter;

		public LanguageMappingProviderViewModel(ILanguageMappingDatabase languageMappingDatabase)
		{
			_languageMappingDatabase = languageMappingDatabase;
			RetrieveMappedLanguagesFromDatabase();
			FilteredMappedLanguages = MappedLanguages;
			PropertyChanged += FilterPropertyChangedHandler;
			InitializeCommands();
		}

		public IList<LanguageMapping> MappedLanguages
		{
			get => _mappedLanguages;
			set
			{
				_mappedLanguages = value;
				OnPropertyChanged();
			}
		}

		public IList<LanguageMapping> FilteredMappedLanguages
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

		public ICommand ClearCommand { get; private set; }
		public ICommand ResetToDefaultCommand { get; private set; }
		public ICommand ApplyChangesCommand { get; private set; }
		public ICommand CloseLanguageMappingProviderCommand { get; private set; }

		public event EventHandler LanguageMappingUpdated;

		public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeCommands()
		{
			ClearCommand = new RelayCommand(Clear);
			ResetToDefaultCommand = new AsyncRelayCommand(ResetToDefault);
			ApplyChangesCommand = new RelayCommand(ApplyChanges, CanApplyChanges);
			CloseLanguageMappingProviderCommand = new RelayCommand(CloseLanguageMappingProvider);
		}

		private void ApplyFilter()
		{
			if (string.IsNullOrWhiteSpace(Filter))
			{
				FilteredMappedLanguages = new List<LanguageMapping>(MappedLanguages);
				return;
			}

			var filterLower = Filter.ToLower();
			var filterParts = filterLower.Split(' ').Where(x => !string.IsNullOrEmpty(x));

			var filteredContent = MappedLanguages.Where(language =>
				filterParts.All(filterPart => (!string.IsNullOrEmpty(language.Name) && language.Name.IndexOf(filterPart, StringComparison.OrdinalIgnoreCase) >= 0)
										   || (!string.IsNullOrEmpty(language.Region) && language.Region.IndexOf(filterPart, StringComparison.OrdinalIgnoreCase) >= 0)
										   || (!string.IsNullOrEmpty(language.TradosCode) && language.TradosCode.IndexOf(filterPart, StringComparison.OrdinalIgnoreCase) >= 0)
										   || (!string.IsNullOrEmpty(language.LanguageCode) && language.LanguageCode.IndexOf(filterPart, StringComparison.OrdinalIgnoreCase) >= 0)));

			FilteredMappedLanguages = new List<LanguageMapping>(filteredContent);
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

			MappedLanguages = new List<LanguageMapping>(newMappedLanguages);
			ApplyFilter();
		}

		private void ApplyChanges(object parameter)
		{
			_languageMappingDatabase.UpdateAll(MappedLanguages);
			RetrieveMappedLanguagesFromDatabase();
			LanguageMappingUpdated?.Invoke(this, EventArgs.Empty);
		}

		private bool CanApplyChanges(object parameter)
		{
			return _languageMappingDatabase.HasMappedLanguagesChanged(MappedLanguages);
		}

		private async Task ResetToDefault()
		{
			try
			{
				if (!ExecuteAction(PluginResources.LMPViewModel_ResetWarning, PluginResources.LMPViewModel_ResetTitle))
				{
					return;
				}

				LoadingAction = PluginResources.Loading_LMP_RestoreDefaults;
				await Task.Delay(1000);
				_languageMappingDatabase.ResetToDefault();
				RetrieveMappedLanguagesFromDatabase();
				await Task.Delay(1000);
			}
			finally
			{
				LanguageMappingUpdated?.Invoke(this, EventArgs.Empty);
				LoadingAction = null;
			}
		}

		private bool ExecuteAction(string message, string title)
		{
			var dialogResult = MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
			return dialogResult == MessageBoxResult.OK;
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

		private void CloseLanguageMappingProvider(object parameter)
		{
			CloseEventRaised?.Invoke();
		}
	}
}