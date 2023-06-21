﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Input;
using GoogleCloudTranslationProvider.Commands;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using GoogleCloudTranslationProvider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace GoogleCloudTranslationProvider.ViewModels
{
	public class MainWindowViewModel : BaseModel
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
		private string _errorMessage;
		private string _multiButtonContent;
		private bool _dialogResult;
		private bool _isTellMeAction;
		private bool _isProviderViewSelected;
		private bool _isSettingsViewSelected;
		private bool _showSettingsView;
		private bool _showMultiButton;

		private string _jsonFilePath;
		private string _projectId;
		private string _projectLocation;
		private string _glossary;
		private string _customModel;

		private ICommand _navigateToCommand;
		private ICommand _switchViewCommand;
		private ICommand _saveCommand;

		public MainWindowViewModel(ITranslationOptions options,
								   ITranslationProviderCredentialStore credentialStore,
								   LanguagePair[] languagePairs,
								   bool showSettingsView = false)
		{
			Options = options;
			ShowSettingsView = showSettingsView;
			_credentialStore = credentialStore;
			_languagePairs = languagePairs;
			_htmlUtil = new HtmlUtil();
			InitializeViews();
			SwitchView(showSettingsView ? ViewDetails_Settings : ViewDetails_Provider);
			_providerViewModel.ClearMessageRaised += ClearMessageRaised;
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
		public string Glossary
		{
			get => _glossary;
			set
            {
                if (_glossary == value) return;
                _glossary = value;
				OnPropertyChanged(nameof(Glossary));
            }
		}
		public string CustomModel
		{
			get => _customModel;
			set
			{
				if (_customModel == value) return;
				_customModel = value;
				OnPropertyChanged(nameof(CustomModel));
			}
		}
		public bool ShowSettingsView
		{
			get => _showSettingsView;
			set
			{
				if (_showSettingsView == value) return;
				_showSettingsView = value;
				OnPropertyChanged(nameof(ShowSettingsView));
			}
		}
		private bool _showProjectInfo;
		public bool ShowProjectInfo
		{
			get => _showProjectInfo;
			set
			{
				if (_showProjectInfo == value) return;
				_showProjectInfo = value;
				OnPropertyChanged(nameof(ShowProjectInfo));
			}
		}

		public MainWindowViewModel(ITranslationOptions options, ISettingsControlViewModel settingsControlViewModel, bool isTellMeAction)
		{
			Options = options;
			_isTellMeAction = isTellMeAction;
			_settingsViewModel = settingsControlViewModel;

			_availableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = ViewDetails_Settings,
					ViewModel = settingsControlViewModel.ViewModel
				}
			};

			SwitchView(ViewDetails_Settings);
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
			ShowMultiButton = true;
			_providerViewModel = new ProviderViewModel(Options);
			_settingsViewModel = new SettingsViewModel(Options, !_showSettingsView);
			ShowProjectInfo = _showSettingsView && (_providerViewModel.SelectedGoogleApiVersion.Version == ApiVersion.V3);

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

			if (_showSettingsView && _providerViewModel.SelectedGoogleApiVersion.Version == ApiVersion.V2)
			{
				ShowMultiButton = false;
			}



			if (!_showProjectInfo)
			{
				return;
			}

			var uriQuery = HttpUtility.ParseQueryString(Options.Uri.OriginalString);
			JsonFilePath = uriQuery.Get("jsonfilepath") ?? PluginResources.ProjectInfo_PathCorrupted;
			ProjectId = uriQuery.Get("projectid") ?? PluginResources.ProjectInfo_ProjectIdMissing;
			ProjectLocation = uriQuery.Get("projectlocation") ?? PluginResources.ProjectInfo_LocationMissing;
			Glossary = uriQuery.Get("glossarypath") ?? PluginResources.ProjectInfo_GlossaryNotUsed;
			CustomModel = uriQuery.Get("googleenginemodel") ?? PluginResources.ProjectInfo_CustomModelNotUsed;
		}

		public bool IsWindowValid()
		{
			var isGoogleProvider = _providerViewModel?.SelectedTranslationOption?.ProviderType == ProviderType.GoogleTranslate;
			if (isGoogleProvider && !ValidGoogleOptions())
			{
				return false;
			}

			return _settingsViewModel.SettingsAreValid();
		}

		private bool ValidGoogleOptions()
		{
			return _providerViewModel.IsV2Checked ? _providerViewModel.CanConnectToGoogleV2(_htmlUtil)
												  : _providerViewModel.CanConnectToGoogleV3(_languagePairs);
		}

		private void Save(object o)
		{
			if (_isTellMeAction)
			{
				SetGeneralProviderOptions();
				DialogResult = true;
				CloseEventRaised?.Invoke();
				return;
			}

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
			Options.SelectedProvider = _providerViewModel.SelectedTranslationOption.ProviderType;
			Options.GoogleEngineModel = _providerViewModel.GoogleEngineModel;
			Options.ProjectLocation = _providerViewModel.ProjectLocation;
			Options.GlossaryPath = _providerViewModel.GlossaryPath;
			Options.BasicCsv = _providerViewModel.BasicCsvGlossary;
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
				Options.LanguagesSupported = new Dictionary<string, string>();
			}

			if (_languagePairs is null)
			{
				return;
			}

			foreach (var languagePair in _languagePairs)
			{
				Options.LanguagesSupported.Add(languagePair.TargetCultureName, _providerViewModel.SelectedTranslationOption.Name);
			}
		}

		private void DeleteCredentialsIfNecessary()
		{
			if (Options.PersistGoogleKey)
			{
				return;
			}

			var providerUri = new Uri(Constants.GoogleTranslationFullScheme);
			var credentials = _credentialStore.GetCredential(providerUri);
			if (credentials is null)
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