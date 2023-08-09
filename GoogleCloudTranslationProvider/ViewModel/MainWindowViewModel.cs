using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using GoogleCloudTranslationProvider.Commands;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using GoogleCloudTranslationProvider.Service;
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
		private readonly LanguagePair[] _languagePairs;
		private readonly HtmlUtil _htmlUtil;

		private ViewDetails _selectedView;
		private List<ViewDetails> _availableViews;
		private IProviderControlViewModel _providerViewModel;
		private ISettingsControlViewModel _settingsViewModel;

		private string _translatorErrorResponse;
		private string _multiButtonContent;
		private bool _dialogResult;
		private bool _isProviderViewSelected;
		private bool _isSettingsViewSelected;
		private bool _showSettingsView;
		private bool _showMultiButton;

		private string _jsonFilePath;
		private string _projectId;
		private string _projectLocation;

		private ICommand _navigateToCommand;
		private ICommand _switchViewCommand;
		private ICommand _saveCommand;

		public MainWindowViewModel(ITranslationOptions options,
								   ITranslationProviderCredentialStore credentialStore,
								   LanguagePair[] languagePairs,
								   bool editProvider = false)
		{
			Options = options;
			EditProvider = editProvider;
			ShowMultiButton = !(EditProvider && Options.SelectedGoogleVersion == ApiVersion.V2);
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
			get => _showSettingsView;
			set
			{
				if (_showSettingsView == value) return;
				_showSettingsView = value;
				OnPropertyChanged(nameof(EditProvider));
			}
		}

		public ITranslationOptions Options { get; set; }

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

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeViews()
		{
			_providerViewModel = new ProviderViewModel(Options, _languagePairs.ToList(), EditProvider);
			_settingsViewModel = new SettingsViewModel(Options);

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
			return ValidGoogleOptions() && _settingsViewModel.SettingsAreValid();
		}

		private bool ValidGoogleOptions()
		{
			return _providerViewModel.IsV2Checked ? _providerViewModel.CanConnectToGoogleV2(_htmlUtil)
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
			Options.ApiKey = _providerViewModel.ApiKey;
			Options.PersistGoogleKey = _providerViewModel.PersistGoogleKey;
			Options.SelectedGoogleVersion = _providerViewModel.SelectedGoogleApiVersion.Version;
			Options.JsonFilePath = _providerViewModel.JsonFilePath;
			Options.ProjectId = _providerViewModel.ProjectId;
			Options.ProjectLocation = _providerViewModel.ProjectLocation;
			Options.LanguageMappingPairs = _providerViewModel.LanguageMappingPairs;
		}

		private void SetGeneralProviderOptions()
		{
			if (_settingsViewModel is not null)
			{
				var providerName = _settingsViewModel.CustomProviderName;
				providerName = providerName is null ? null : Regex.Replace(providerName.Trim(), @"\s+", " ");
				Options.SendPlainTextOnly = _settingsViewModel.SendPlainText;
				Options.ResendDrafts = _settingsViewModel.ReSendDraft;
				Options.UsePreEdit = _settingsViewModel.DoPreLookup;
				Options.PreLookupFilename = _settingsViewModel.PreLookupFileName;
				Options.UsePostEdit = _settingsViewModel.DoPostLookup;
				Options.PostLookupFilename = _settingsViewModel.PostLookupFileName;
				Options.CustomProviderName = providerName;
				Options.UseCustomProviderName = _settingsViewModel.UseCustomProviderName && !string.IsNullOrEmpty(Options.CustomProviderName);
			}

			if (Options is not null && Options.LanguagesSupported is null)
			{
				Options.LanguagesSupported = new();
			}

			if (_languagePairs is null)
			{
				return;
			}

			foreach (var languagePair in _languagePairs)
			{
				if (Options.LanguagesSupported.Contains(languagePair.TargetCultureName))
				{
					continue;
				}

				Options.LanguagesSupported.Add(languagePair.TargetCultureName);
			}
		}

		private void DeleteCredentialsIfNecessary()
		{
			if (Options.PersistGoogleKey)
			{
				return;
			}

			var providerUri = new Uri(Constants.GoogleTranslationFullScheme);
			if (_credentialStore.GetCredential(providerUri) is null)
			{
				return;
			}

			_credentialStore.RemoveCredential(providerUri);
		}

		private void ClearMessageRaised()
		{
			TranslatorErrorResponse = "<html><body></html></body>";
		}

		private void SwitchView(object o)
		{
			try
			{
				var destination = IsProviderViewSelected ? ViewDetails_Settings
														 : ViewDetails_Provider;
				TrySwitchView(o as string ?? destination);
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
	}
}