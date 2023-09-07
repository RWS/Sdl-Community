using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using LanguageMappingProvider.Database;
using LanguageMappingProvider.Database.Interface;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.LanguageMappingProvider;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.View;
using LanguageWeaverProvider.ViewModel.Cloud;
using LanguageWeaverProvider.ViewModel.Interface;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.ViewModel
{
	public class PairMappingViewModel : BaseViewModel
	{
		private readonly ITranslationOptions _translationOptions;
		private readonly ILanguageMappingDatabase _languageMappingDatabase;
		private readonly LanguagePair[] _languagePairs;

		private IPairMappingViewModel _pairMappingView;
		private SettingsViewModel _settingsView;
		private bool _showSettingsView;

		public PairMappingViewModel(ITranslationOptions translationOptions, LanguagePair[] languagePairs)
		{
			_languagePairs = languagePairs;
			_translationOptions = translationOptions;
			_languageMappingDatabase = SetDatabase();
			_pairMappingView = SetPairMappingView();
			_settingsView = SetSettingsView();
			InitializeCommands();
		}

		public IPairMappingViewModel PairMappingView
		{
			get => _pairMappingView;
			set
			{
				_pairMappingView = value;
				OnPropertyChanged();
			}
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

			OpenFeedbackCommand = new RelayCommand(OpenFeedback);
		}

		private IPairMappingViewModel SetPairMappingView()
		{
			var isCloudSelected = _translationOptions.Version == PluginVersion.LanguageWeaverCloud;
			IPairMappingViewModel pairMappingView = isCloudSelected
												  ? new CloudMappingViewModel(_translationOptions, _languageMappingDatabase, _languagePairs)
												  : null; // modify this to add EdgeVM
			return pairMappingView;
		}

		private SettingsViewModel SetSettingsView()
		{
			var settingsViewModel = new SettingsViewModel(_translationOptions);
			settingsViewModel.BackCommandExecuted += ChangeSettingsViewState;
			return settingsViewModel;
		}

		private ILanguageMappingDatabase SetDatabase()
		{
			var isCloudSelected = _translationOptions.Version == PluginVersion.LanguageWeaverCloud;
			var targetDatabase = isCloudSelected ? "cloud" : "edge";
			var supportedLanguages = isCloudSelected ? DatabaseControl.GetCloudLanguageCodes() : null;
			return new LanguageMappingDatabase(targetDatabase, supportedLanguages);
		}

		private void Save(object parameter)
		{
			SaveChanges = true;
			_translationOptions.PairMappings = PairMappingView.PairMappings.ToList();
			_translationOptions.ProviderSettings.ResendDrafts = SettingsView.ResendDrafts;
			_translationOptions.ProviderSettings.IncludeTags = SettingsView.IncludeTags;
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
			ShowSettingsView = true;
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
			_pairMappingView.UpdateLanguageMapping();
		}

		private void ChangeSettingsViewState(object sender, EventArgs e)
		{
			ShowSettingsView = false;
		}

		public ICommand OpenFeedbackCommand { get; private set; }

		private void OpenFeedback(object parameter)
		{
			var fbViewModel = new FeedbackViewModel();
			var fbview = new FeedbackView() { DataContext = fbViewModel };
			fbview.ShowDialog();
		}
	}
}