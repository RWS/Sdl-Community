using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using GoogleCloudTranslationProvider.Commands;
using GoogleCloudTranslationProvider.GoogleAPI;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using GoogleCloudTranslationProvider.View;
using GoogleCloudTranslationProvider.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace GoogleCloudTranslationProvider.ViewModels
{
	public class MainWindowViewModel : BaseViewModel
	{
		private const string ViewDetails_Provider = nameof(ProviderViewModel);
		private const string ViewDetails_Settings = nameof(SettingsViewModel);

		private readonly ITranslationProviderCredentialStore _credentialStore;
		private readonly IEnumerable<LanguagePair> _languagePairs;
		private readonly HtmlUtil _htmlUtil;

		private ViewDetails _selectedView;
		private List<ViewDetails> _availableViews;
		private IProviderControlViewModel _providerViewModel;
		private ISettingsControlViewModel _settingsViewModel;
		private bool _isLanguageMappingProviderEnabled;

		private string _translatorErrorResponse;
		private string _multiButtonContent;
		private bool _dialogResult;
		private bool _isProviderViewSelected;
		private bool _isSettingsViewSelected;
		private bool _editProvider;
		private bool _showMultiButton;

		private string _jsonFilePath;
		private string _projectId;
		private string _projectLocation;

		private ICommand _navigateToCommand;
		private ICommand _switchViewCommand;
		private ICommand _saveCommand;
		private ICommand _openLanguageMappingCommand;

		public MainWindowViewModel(ITranslationOptions options,
								   ITranslationProviderCredentialStore credentialStore,
								   IEnumerable<LanguagePair> languagePairs,
								   bool editProvider = false)
		{
			TranslationOptions = options;
			EditProvider = editProvider;
			ShowMultiButton = !(EditProvider && TranslationOptions.SelectedGoogleVersion == ApiVersion.V2);
			_credentialStore = credentialStore;
			_languagePairs = languagePairs;
			_htmlUtil = new HtmlUtil();
			InitializeViews();
			SwitchView(ShowMultiButton ? ViewDetails_Provider : ViewDetails_Settings);
		}

		public string JsonFilePath
		{
			get => _jsonFilePath;
			set
			{
				if (_jsonFilePath == value) return;
				_jsonFilePath = value;
				OnPropertyChanged(nameof(JsonFilePath));
			}
		}
		public string ProjectId
		{
			get => _projectId;
			set
			{
				if (_projectId == value) return;
				_projectId = value;
				OnPropertyChanged(nameof(ProjectId));
			}
		}

		public string ProjectLocation
		{
			get => _projectLocation;
			set
			{
				if (_projectLocation == value) return;
				_projectLocation = value;
				OnPropertyChanged(nameof(ProjectLocation));
			}
		}

		public bool EditProvider
		{
			get => _editProvider;
			set
			{
				if (_editProvider == value) return;
				_editProvider = value;
				OnPropertyChanged(nameof(EditProvider));
			}
		}

		public ITranslationOptions TranslationOptions { get; set; }

		public ViewDetails SelectedView
		{
			get => _selectedView;
			set
			{
				if (_selectedView == value) return;
				_selectedView = value;
				OnPropertyChanged(nameof(SelectedView));
			}
		}

		public bool IsLanguageMappingProviderEnabled
		{
			get => _isLanguageMappingProviderEnabled;
			set
			{
				if (_isLanguageMappingProviderEnabled == value) return;
				_isLanguageMappingProviderEnabled = value;
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

		public string MultiButtonContent
		{
			get => _multiButtonContent;
			set
			{
				if (_multiButtonContent == value) return;
				_multiButtonContent = value;
				OnPropertyChanged(nameof(MultiButtonContent));
			}
		}

		public bool IsSettingsViewSelected
		{
			get => _isSettingsViewSelected;
			set
			{
				if (_isSettingsViewSelected == value) return;
				_isSettingsViewSelected = value;
				OnPropertyChanged(nameof(IsSettingsViewSelected));
			}
		}

		public bool IsProviderViewSelected
		{
			get => _isProviderViewSelected;
			set
			{
				if (_isProviderViewSelected == value) return;
				_isProviderViewSelected = value;
				OnPropertyChanged(nameof(IsProviderViewSelected));
			}
		}

		public string TranslatorErrorResponse
		{
			get => _translatorErrorResponse;
			set
			{
				if (_translatorErrorResponse == value) return;
				_translatorErrorResponse = value;
				OnPropertyChanged(nameof(TranslatorErrorResponse));
			}
		}

		public bool ShowMultiButton
		{
			get => _showMultiButton;
			set
			{
				if (_showMultiButton == value) return;
				_showMultiButton = value;
				OnPropertyChanged(nameof(ShowMultiButton));
			}
		}


		public ICommand SwitchViewCommand => _switchViewCommand ??= new RelayCommand(SwitchView);

		public ICommand NavigateToCommand => _navigateToCommand ??= new RelayCommand(NavigateTo);

		public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);

		public ICommand OpenLanguageMappingCommand => _openLanguageMappingCommand ??= new RelayCommand(OpenLanguageMapping);

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeViews()
		{
			_providerViewModel = new ProviderViewModel(TranslationOptions, _languagePairs?.ToList(), _editProvider)
			{
				SwitchViewExternal = new RelayCommand(SwitchView)
			};
			_providerViewModel.LanguageMappingLoaded += LanguageMappingLoaded;

			_settingsViewModel = new SettingsViewModel(TranslationOptions);
			_availableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = ViewDetails_Provider,
					ViewModel = _providerViewModel.ViewModel
				},
				new ViewDetails
				{
					Name = ViewDetails_Settings,
					ViewModel = _settingsViewModel.ViewModel
				}
			};
		}

		public bool IsWindowValid()
		{
			return ValidGoogleOptions().Result && _settingsViewModel.SettingsAreValid();
		}

		private async Task<bool> ValidGoogleOptions()
		{
			return _providerViewModel.IsV2Checked ? await _providerViewModel.CanConnectToGoogleV2(_htmlUtil)
												  : _providerViewModel.CanConnectToGoogleV3(_languagePairs);
		}

		private void Save(object o)
		{
			if (!IsWindowValid())
			{
				return;
			}

			SetGoogleProviderOptions();
			SetGeneralProviderOptions();
			DeleteCredentialsIfNecessary();
			DialogResult = true;
			CloseEventRaised?.Invoke();
		}

		private void SetGoogleProviderOptions()
		{
			TranslationOptions.ApiKey = _providerViewModel.ApiKey;
			TranslationOptions.PersistGoogleKey = _providerViewModel.PersistGoogleKey;
			TranslationOptions.SelectedGoogleVersion = _providerViewModel.SelectedGoogleApiVersion.Version;
			TranslationOptions.JsonFilePath = _providerViewModel.JsonFilePath;
			TranslationOptions.ProjectId = _providerViewModel.ProjectId;
			TranslationOptions.ProjectLocation = _providerViewModel.ProjectLocation;
			TranslationOptions.LanguageMappingPairs = _providerViewModel.LanguageMappingPairs;
		}

		private void SetGeneralProviderOptions()
		{
			if (_settingsViewModel is not null)
			{
				var providerName = _settingsViewModel.CustomProviderName;
				providerName = providerName is null ? null : Regex.Replace(providerName.Trim(), @"\s+", " ");
				TranslationOptions.SendPlainTextOnly = _settingsViewModel.SendPlainText;
				TranslationOptions.ResendDrafts = _settingsViewModel.ReSendDraft;
				TranslationOptions.UsePreEdit = _settingsViewModel.DoPreLookup;
				TranslationOptions.PreLookupFilename = _settingsViewModel.PreLookupFileName;
				TranslationOptions.UsePostEdit = _settingsViewModel.DoPostLookup;
				TranslationOptions.PostLookupFilename = _settingsViewModel.PostLookupFileName;
				TranslationOptions.CustomProviderName = providerName;
				TranslationOptions.UseCustomProviderName = _settingsViewModel.UseCustomProviderName && !string.IsNullOrEmpty(TranslationOptions.CustomProviderName);
			}

			if (TranslationOptions is not null && TranslationOptions.LanguagesSupported is null)
			{
				TranslationOptions.LanguagesSupported = new();
			}

			if (_languagePairs is null)
			{
				return;
			}

			foreach (var languagePair in _languagePairs)
			{
				if (TranslationOptions.LanguagesSupported.Contains(languagePair.TargetCultureName))
				{
					continue;
				}

				TranslationOptions.LanguagesSupported.Add(languagePair.TargetCultureName);
			}

			if (TranslationOptions.SelectedGoogleVersion.Equals(ApiVersion.V3)
			 && (TranslationOptions.V3SupportedLanguages is null || !TranslationOptions.V3SupportedLanguages.Any()))
			{
				var v3Connector = new V3Connector(TranslationOptions);
				TranslationOptions.V3SupportedLanguages = v3Connector.GetLanguages();
			}
		}

		private void DeleteCredentialsIfNecessary()
		{
			if (TranslationOptions.PersistGoogleKey)
			{
				return;
			}

			var providerUri = new Uri(Constants.GoogleTranslationFullScheme);
			if (_credentialStore?.GetCredential(providerUri) is null)
			{
				return;
			}

			_credentialStore.RemoveCredential(providerUri);
		}

		private void SwitchView(object o)
		{
			try
			{
				var destination = IsProviderViewSelected ? ViewDetails_Settings
														 : ViewDetails_Provider;
				TrySwitchView(o as string ?? destination);
				UpdateLanguageMappingButton();
			}
			catch (Exception e)
			{
				ErrorHandler.HandleError(e);
			}
		}

		private void TrySwitchView(string requestedType)
		{
			SelectedView = _availableViews.FirstOrDefault(x => x.Name == requestedType);
			UpdateLayout(requestedType);
		}

		private void UpdateLayout(string selectedViewType)
		{
			IsProviderViewSelected = selectedViewType == ViewDetails_Provider;
			IsSettingsViewSelected = selectedViewType == ViewDetails_Settings;
			MultiButtonContent = IsProviderViewSelected ? PluginResources.MultiButton_Settings
														: PluginResources.MultiButton_Provider;
		}

		private void UpdateLanguageMappingButton()
		{
			IsLanguageMappingProviderEnabled = _providerViewModel.IsV2Checked
											 ? File.Exists(string.Format(Constants.DatabaseFilePath, PluginResources.Database_PluginName_V2))
											 : File.Exists(string.Format(Constants.DatabaseFilePath, PluginResources.Database_PluginName_V3));
		}

		private void NavigateTo(object o)
		{
			var currentVersion = _providerViewModel?.SelectedGoogleApiVersion?.Version;
			var uriTarget = currentVersion switch
			{
				ApiVersion.V3 => Constants.V3Documentation,
				_ => Constants.FullDocumentation,
			};

			Process.Start(uriTarget);
		}

		private void OpenLanguageMapping(object parameter)
		{
			TranslationOptions.SelectedGoogleVersion = _providerViewModel.IsV2Checked ? ApiVersion.V2 : ApiVersion.V3;

			var lmpViewModel = new LanguageMappingProviderViewModel(TranslationOptions, EditProvider);
			lmpViewModel.LanguageMappingUpdated += LanguageMappingUpdated;

			var lmpView = new LanguageMappingProviderView() { DataContext = lmpViewModel };
			lmpViewModel.CloseEventRaised += lmpView.Close;

			var dialog = lmpView.ShowDialog();
		}

		private void LanguageMappingUpdated(object sender, EventArgs e)
		{
			_providerViewModel.UpdateLanguageMapping();
		}

		private void LanguageMappingLoaded(object sender, EventArgs e)
		{
			UpdateLanguageMappingButton();
		}
	}
}