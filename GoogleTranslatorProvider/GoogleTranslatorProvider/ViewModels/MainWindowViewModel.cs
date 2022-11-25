using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GoogleTranslatorProvider.Commands;
using GoogleTranslatorProvider.GoogleAPI;
using GoogleTranslatorProvider.Interface;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace GoogleTranslatorProvider.ViewModels
{
	public class MainWindowViewModel : BaseModel
	{
		
		private readonly ITranslationProviderCredentialStore _credentialStore;
		private readonly IProviderControlViewModel _providerViewModel;
		private readonly ISettingsControlViewModel _settingsViewModel;
		private readonly IHelpViewModel _helpViewModel;

		private readonly bool _isTellMeAction;
		private readonly List<ViewDetails> _availableViews;
		private readonly LanguagePair[] _languagePairs;
		private readonly HtmlUtil _htmlUtil;
		
		private ViewDetails _selectedView;
		private ViewDetails _previousView;

		private string _translatorErrorResponse;
		private string _errorMessage;
		private bool _dialogResult;

		private bool _isProviderViewSelected;
		private bool _isSettingsViewSelected;
		private bool _isHelpViewSelected;
		private bool _backButtonIsVisible;

		private ICommand _switchViewCommand;
		private ICommand _goBackCommand;
		private ICommand _saveCommand;

		public MainWindowViewModel(ITranslationOptions options,
								   IProviderControlViewModel providerViewModel,
								   ISettingsControlViewModel settingsViewModel,
								   IHelpViewModel helpViewModel,
								   ITranslationProviderCredentialStore credentialStore,
								   LanguagePair[] languagePairs,
								   HtmlUtil htmlUtil)
		{
			Options = options;
			_providerViewModel = providerViewModel;
			_settingsViewModel = settingsViewModel;
			_helpViewModel = helpViewModel;
			_credentialStore = credentialStore;
			_languagePairs = languagePairs;
			_htmlUtil = htmlUtil;

			providerViewModel.ClearMessageRaised += ClearMessageRaised;

			_availableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = Constants.Views_Provider,
					ViewModel = providerViewModel.ViewModel
				},
				new ViewDetails
				{
					Name = Constants.Views_Settings,
					ViewModel = settingsViewModel.ViewModel
				},
				new ViewDetails()
				{
					Name = Constants.Views_Help,
					ViewModel = helpViewModel.ViewModel
				}
			};

			TrySwitchView(Constants.Views_Provider);
		}

		public MainWindowViewModel(ITranslationOptions options, ISettingsControlViewModel settingsControlViewModel, IHelpViewModel helpViewModel, bool isTellMeAction)
		{
			Options = options;
			_isTellMeAction = isTellMeAction;
			_settingsViewModel = settingsControlViewModel;
			_helpViewModel = helpViewModel;

			_availableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = Constants.Views_Settings,
					ViewModel = settingsControlViewModel.ViewModel
				},
				new ViewDetails()
				{
					Name = Constants.Views_Help,
					ViewModel = helpViewModel.ViewModel
				}
			};

			if (_isTellMeAction)
			{
				IsSettingsViewSelected = true;
				SelectedView = _availableViews[0];
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
				ErrorMessage = string.Empty;
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

		public bool BackButtonIsVisible
		{
			get => _backButtonIsVisible;
			set
			{
				if (_backButtonIsVisible == value) return;
				_backButtonIsVisible = value;
				OnPropertyChanged(nameof(BackButtonIsVisible));
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

		public bool IsHelpViewSelected
		{
			get => _isHelpViewSelected;
			set
			{
				if (_isHelpViewSelected == value) return;
				_isHelpViewSelected = value;
				OnPropertyChanged(nameof(IsHelpViewSelected));
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

		public string ErrorMessage
		{
			get => _errorMessage;
			set
			{
				if (_errorMessage == value) return;
				_errorMessage = value;
				OnPropertyChanged(nameof(ErrorMessage));
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


		public ICommand SwitchViewCommand => _switchViewCommand ??= new RelayCommand(SwitchView);

		public ICommand GoBackCommand => _goBackCommand ??= new RelayCommand(GoBack);

		public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;


		public bool IsWindowValid()
		{
			ErrorMessage = string.Empty;
			var isGoogleProvider = _providerViewModel.SelectedTranslationOption?.ProviderType == ProviderType.GoogleTranslate;
			if (isGoogleProvider && !ValidGoogleOptions())
			{
				return false;
			}

			return ValidSettingsPageOptions();
		}

		private bool ValidGoogleOptions()
		{
			if (_providerViewModel.SelectedGoogleApiVersion.Version == ApiVersion.V3)
			{
				return GoogleV3OptionsAreSet() && AreGoogleV3CredentialsValid();
			}

			if (string.IsNullOrEmpty(_providerViewModel.ApiKey))
			{
				ErrorMessage = PluginResources.ApiKeyError;
				return false;
			}

			return AreGoogleV2CredentialsValid();
		}

		private bool GoogleV3OptionsAreSet()
		{
			if (string.IsNullOrEmpty(_providerViewModel.JsonFilePath))
			{
				ErrorMessage = PluginResources.EmptyJsonFilePathMsg;
				return false;
			}

			if (!File.Exists(_providerViewModel.JsonFilePath))
			{
				ErrorMessage = PluginResources.WrongJsonFilePath;
				return false;
			}

			if (string.IsNullOrEmpty(_providerViewModel.ProjectName))
			{
				ErrorMessage = PluginResources.InvalidProjectName;
				return false;
			}

			var projectLocation = _providerViewModel.ProjectLocation ?? _providerViewModel.SelectedLocation.Key;
			if (string.IsNullOrEmpty(projectLocation))
			{
				ErrorMessage = PluginResources.ProjectLocationValidation;
				return false;
			}

			return true;
		}

		private bool AreGoogleV3CredentialsValid()
		{
			try
			{
				var providerOptions = new GTPTranslationOptions
				{
					ProjectName = _providerViewModel.ProjectName,
					JsonFilePath = _providerViewModel.JsonFilePath,
					GoogleEngineModel = _providerViewModel.GoogleEngineModel,
					ProjectLocation = _providerViewModel.ProjectLocation,
					GlossaryPath = _providerViewModel.GlossaryPath,
					BasicCsv = _providerViewModel.BasicCsvGlossary,
					SelectedProvider = _providerViewModel.SelectedTranslationOption.ProviderType,
					SelectedGoogleVersion = _providerViewModel.SelectedGoogleApiVersion.Version
				};

				var googleV3 = new V3Connector(providerOptions);
				googleV3.TryToAuthenticateUser();
				if (!string.IsNullOrEmpty(providerOptions.GlossaryPath) && _languagePairs is not null)
				{
					googleV3.CreateGoogleGlossary(_languagePairs);
				}

				return true;
			}
			catch (Exception e)
			{
				string message;
				if (e.Message.Contains("Resource type: models"))
				{
					message = PluginResources.GoogleInvalidEngine;
				}
				else if (e.Message.Contains("Invalid resource name"))
				{
					message = PluginResources.InvalidProjectName;
				}
				else
				{
					message = e.Message;
				}

				AddEncriptionMetaToResponse(message);
				return false;
			}
		}

		private bool AreGoogleV2CredentialsValid()
		{
			try
			{
				var v2Connector = new V2Connector(_providerViewModel.ApiKey, _htmlUtil);
				v2Connector.ValidateCredentials();
				return true;
			}
			catch (Exception e)
			{
				AddEncriptionMetaToResponse(e.Message);
				return false;
			}
		}

		private bool ValidSettingsPageOptions()
		{
			if (_settingsViewModel.DoPreLookup)
			{
				if (string.IsNullOrEmpty(_settingsViewModel.PreLookupFileName))
				{
					ErrorMessage = PluginResources.PreLookupEmptyMessage;
					return false;
				}

				if (!File.Exists(_settingsViewModel.PreLookupFileName))
				{
					ErrorMessage = PluginResources.PreLookupWrongPathMessage;
					return false;
				}
			}

			if (!_settingsViewModel.DoPostLookup)
			{
				return true;
			}

			if (string.IsNullOrEmpty(_settingsViewModel.PostLookupFileName))
			{
				ErrorMessage = PluginResources.PostLookupEmptyMessage;
				return false;
			}

			if (!File.Exists(_settingsViewModel.PostLookupFileName))
			{
				ErrorMessage = PluginResources.PostLookupWrongPathMessage;
				return false;
			}

			return true;
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
			Options.ProjectName = _providerViewModel.ProjectName;
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
				Options.SendPlainTextOnly = _settingsViewModel.SendPlainText;
				Options.ResendDrafts = _settingsViewModel.ReSendDraft;
				Options.UsePreEdit = _settingsViewModel.DoPreLookup;
				Options.PreLookupFilename = _settingsViewModel.PreLookupFileName;
				Options.UsePostEdit = _settingsViewModel.DoPostLookup;
				Options.PostLookupFilename = _settingsViewModel.PostLookupFileName;
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
			var isGoogleProvider = _providerViewModel.SelectedTranslationOption.ProviderType == ProviderType.GoogleTranslate;
			if (isGoogleProvider && !Options.PersistGoogleKey)
			{
				RemoveCredentialsFromStore(new Uri(Constants.GoogleTranslationFullScheme));
			}
		}

		private void RemoveCredentialsFromStore(Uri providerUri)
		{
			if (_credentialStore.GetCredential(providerUri) is not null)
			{
				_credentialStore.RemoveCredential(providerUri);
			}
		}

		private void ClearMessageRaised()
		{
			ErrorMessage = string.Empty;
			TranslatorErrorResponse = "<html><body></html></body>";
		}

		private void SwitchView(object o)
		{
			if (o is not string objectStr)
			{
				return;
			}

			try
			{
				TrySwitchView(objectStr, objectStr == Constants.Views_Help ? SelectedView : null);
			}
			catch { }
		}

		private void TrySwitchView(string requestedType, ViewDetails currentView = null)
		{
			_previousView = currentView;
			SelectedView = _availableViews.FirstOrDefault(x => x.Name == requestedType);
			UpdateLayout(requestedType);
		}

		private void UpdateLayout(string selectedViewType)
		{
			IsProviderViewSelected = selectedViewType == Constants.Views_Provider;
			IsSettingsViewSelected = selectedViewType == Constants.Views_Settings;
			IsHelpViewSelected = selectedViewType == Constants.Views_Help;
			BackButtonIsVisible = CanUseBackButtonV();
		}

		private void GoBack(object o)
		{
			if (_isTellMeAction || _previousView?.ViewModel.GetType() == typeof(SettingsViewModel))
			{
				TrySwitchView(Constants.Views_Settings);
				return;
			}

			TrySwitchView(Constants.Views_Provider);
		}

		private bool CanUseBackButtonV()
		{
			if (_isTellMeAction)
			{
				return IsHelpViewSelected;
			}

			return IsSettingsViewSelected || IsHelpViewSelected;
		}

		private void AddEncriptionMetaToResponse(string errorMessage)
		{
			var htmlStart = @"<html> 
 <meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>
 <body style=""font-family:Segoe Ui!important;color:red!important;font-size:13px!important"">
";
			TranslatorErrorResponse = $"{errorMessage.Insert(0, htmlStart)}\n</body></html>";
		}
	}
}