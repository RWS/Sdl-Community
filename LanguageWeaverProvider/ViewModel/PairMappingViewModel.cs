using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageMappingProvider.Database.Interface;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.LanguageMappingProvider;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.View;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.ViewModel
{
	public class PairMappingViewModel : BaseViewModel
	{
		readonly ITranslationOptions _translationOptions;
		readonly ILanguageMappingDatabase _languageMappingDatabase;
		readonly LanguagePair[] _languagePairs;

		SettingsViewModel _settingsView;
		bool _showSettingsView;
		string _loadingAction;
		string _windowTitle;

		ObservableCollection<PairMapping> _pairMappings;

		public PairMappingViewModel(ITranslationOptions translationOptions, LanguagePair[] languagePairs)
		{
			WindowTitle = Constants.PairMapping_MainWindow;
			_languagePairs = languagePairs;
			_translationOptions = translationOptions;
			_languageMappingDatabase = DatabaseControl.InitializeDatabase(translationOptions.PluginVersion);
			InitializeSettingsView();
			InitializeCommands();
			LoadPairMapping();
		}

		public SettingsViewModel SettingsView
		{
			get => _settingsView;
			set
			{
				_settingsView = value;
				OnPropertyChanged();
			}
		}

		public bool ShowSettingsView
		{
			get => _showSettingsView;
			set
			{
				_showSettingsView = value;
				OnPropertyChanged();
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

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<PairMapping> PairMappings
		{
			get => _pairMappings;
			set
			{
				if (_pairMappings == value) return;
				_pairMappings = value;
				OnPropertyChanged();
			}
		}

		public bool SaveChanges { get; private set; }

		public ICommand SaveCommand { get; private set; }

		public ICommand CloseCommand { get; private set; }

		public ICommand NavigateToCommand { get; private set; }

		public ICommand OpenSettingsViewCommand { get; private set; }

		public ICommand OpenLanguageMappingProviderViewCommand { get; private set; }

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeCommands()
		{
			SaveCommand = new RelayCommand(Save);
			CloseCommand = new RelayCommand(Close);
			NavigateToCommand = new RelayCommand(NavigateTo);
			OpenSettingsViewCommand = new RelayCommand(OpenSettingsView);
			OpenLanguageMappingProviderViewCommand = new RelayCommand(OpenLanguageMappingProviderView);
		}

		private void InitializeSettingsView()
		{
			var settingsViewModel = new SettingsViewModel(_translationOptions);
			settingsViewModel.BackCommandExecuted += ChangeSettingsViewState;
			_settingsView = settingsViewModel;
		}

		private void Save(object parameter)
		{
			if (!SettingsView.SettingsAreValid())
			{
				return;
			}

			SaveChanges = true;
			_translationOptions.PairMappings = PairMappings.ToList();
			_translationOptions.ProviderSettings.AutosendFeedback = SettingsView.AutosendFeedback;
			_translationOptions.ProviderSettings.ResendDrafts = SettingsView.ResendDrafts;
			_translationOptions.ProviderSettings.IncludeTags = SettingsView.IncludeTags;
			_translationOptions.ProviderSettings.UseCustomName = SettingsView.UseCustomName;
			_translationOptions.ProviderSettings.CustomName = SettingsView.CustomName;
			CloseEventRaised.Invoke();
		}

		private void Close(object parameter)
		{
			SaveChanges = false;
			CloseEventRaised.Invoke();
		}

		private void NavigateTo(object parameter)
		{
			if (parameter is not string uriTarget)
			{
				return;
			}

			Process.Start(uriTarget);
		}

		private void OpenSettingsView(object parameter)
		{
			ChangeSettingsViewState(null, null);
		}

		private void ChangeSettingsViewState(object sender, EventArgs e)
		{
			ShowSettingsView = !ShowSettingsView;
			WindowTitle = ShowSettingsView ? Constants.PairMapping_SettingsWindow : Constants.PairMapping_MainWindow;
		}

		private void OpenLanguageMappingProviderView(object parameter)
		{
			var lmpViewModel = new LanguageMappingProviderViewModel(_languageMappingDatabase);
			lmpViewModel.LanguageMappingUpdated += LanguageMappingUpdated;

			var lmpView = new LanguageMappingProviderView() { DataContext = lmpViewModel };
			lmpViewModel.CloseEventRaised += lmpView.Close;

			var dialog = lmpView.ShowDialog();
		}

		private void LanguageMappingUpdated(object sender, EventArgs e)
		{
			CreatePairMappings();
		}

		private async void LoadPairMapping()
		{
			if (_translationOptions.PairMappings is null || !_translationOptions.PairMappings.Any())
			{
				CreatePairMappings();
				return;
			}

			LoadingAction = "Loading resources...";
			await Task.Delay(50);
			var pairMappings = new ObservableCollection<PairMapping>();
			foreach (var pairMapping in _translationOptions.PairMappings)
			{
				var selectedModelName = pairMapping.SelectedModel.Name;
				var selectedModel = pairMapping.Models.FirstOrDefault(x => x.Name == selectedModelName);
				var newPairMapping = new PairMapping()
				{
					DisplayName = pairMapping.DisplayName,
					SourceCode = pairMapping.SourceCode,
					TargetCode = pairMapping.TargetCode,
					LanguagePair = pairMapping.LanguagePair,
					Models = pairMapping.Models,
					SelectedModel = selectedModel,
					Dictionaries = pairMapping.Dictionaries
				};

				pairMappings.Add(newPairMapping);
			}

			PairMappings = pairMappings;
			LoadingAction = null;
		}

		private async void CreatePairMappings()
		{
			var originalPairMappings = PairMappings;
			PairMappings = new();
			var mappedLanguages = _languageMappingDatabase.GetMappedLanguages();
			LoadingAction = "Getting models...";
			await Task.Delay(50);
			var accountModels = _translationOptions.PluginVersion == PluginVersion.LanguageWeaverCloud
							  ? await CloudService.GetSupportedLanguages(_translationOptions.AccessToken)
							  : await EdgeService.GetLanguagePairs(_translationOptions.AccessToken);
			LoadingAction = "Getting dictionaries...";
			await Task.Delay(50);
			var accountDictionaries = _translationOptions.PluginVersion == PluginVersion.LanguageWeaverCloud
									? await CloudService.GetDictionaries(_translationOptions.AccessToken)
									: await EdgeService.GetDictionaries(_translationOptions.AccessToken);
			LoadingAction = "Loading resources...";
			await Task.Delay(50);
			foreach (var languagePair in _languagePairs)
			{
				var mappedLanguagePairs = mappedLanguages.Where(mappedLang => mappedLang.TradosCode.Equals(languagePair.SourceCultureName) || mappedLang.TradosCode.Equals(languagePair.TargetCultureName));
				var mappedSource = mappedLanguagePairs.FirstOrDefault(mappedLang => mappedLang.TradosCode.Equals(languagePair.SourceCultureName));
				var mappedTarget = mappedLanguagePairs.FirstOrDefault(mappedLang => mappedLang.TradosCode.Equals(languagePair.TargetCultureName));
				var displayName = $"{mappedSource.Name} ({mappedSource.Region}) - {mappedTarget?.Name} ({mappedTarget?.Region})";

				var currentModel = originalPairMappings?.FirstOrDefault(pair => pair.DisplayName.Equals(displayName));
				if (currentModel is not null
				 && mappedSource.LanguageCode.Equals(currentModel.SourceCode)
				 && mappedTarget.LanguageCode.Equals(currentModel.TargetCode))
				{
					PairMappings.Add(currentModel);
					continue;
				}

				var models = accountModels.Where(model => model.SourceLanguageId.Equals(mappedSource.LanguageCode) && model.TargetLanguageId.Equals(mappedTarget.LanguageCode)).ToList();
				if (!models.Any())
				{
					models.Add(new PairModel()
					{
						Name = PluginResources.PairModel_Model_Unavailable,
						DisplayName = PluginResources.PairModel_Model_Unavailable,
						SourceLanguageId = mappedSource.LanguageCode,
						TargetLanguageId = mappedTarget.LanguageCode
					});
				}

				var dictionaries = accountDictionaries.Where(dictionary => dictionary.Source.Equals(mappedSource.LanguageCode) && dictionary.Target.Equals(mappedTarget.LanguageCode))
													  .OrderBy(dictionary => dictionary.Name)
													  .ToList();
				dictionaries.ForEach(x => x.LanguagePair = languagePair);

				PairMappings.Add(new PairMapping
				{
					DisplayName = displayName,
					SourceCode = mappedSource.LanguageCode,
					TargetCode = mappedTarget.LanguageCode,
					LanguagePair = languagePair,
					Models = models,
					SelectedModel = models.FirstOrDefault(),
					Dictionaries = dictionaries
				});
			}

			LoadingAction = null;
		}
	}
}