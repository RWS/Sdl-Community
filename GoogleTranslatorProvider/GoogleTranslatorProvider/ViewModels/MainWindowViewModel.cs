using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using GoogleTranslatorProvider.Commands;
using GoogleTranslatorProvider.GoogleAPI;
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
		private readonly IProviderControlViewModel _providerControlViewModel;
		private readonly ISettingsControlViewModel _settingsControlViewModel;

		private readonly bool _isTellMeAction;
		private readonly List<ViewDetails> _availableViews;
		private readonly LanguagePair[] _languagePairs;
		private readonly HtmlUtil _htmlUtil;
		
		private ViewDetails _selectedView;
		private string _translatorErrorResponse;
		private string _errorMessage;
		private bool _dialogResult;


		public MainWindowViewModel(ITranslationOptions options,
								   IProviderControlViewModel providerControlViewModel,
								   ISettingsControlViewModel settingsControlViewModel,
								   ITranslationProviderCredentialStore credentialStore,
								   LanguagePair[] languagePairs,
								   HtmlUtil htmlUtil)
		{
			Options = options;
			_providerControlViewModel = providerControlViewModel;
			_settingsControlViewModel = settingsControlViewModel;
			_credentialStore = credentialStore;
			_languagePairs = languagePairs;
			_htmlUtil = htmlUtil;

			SaveCommand = new RelayCommand(Save);
			ShowSettingsViewCommand = new CommandHandler(ShowSettingsPage, true);
			ShowMainViewCommand = new CommandHandler(ShowProvidersPage, true);

			providerControlViewModel.ShowSettingsCommand = ShowSettingsViewCommand;
			providerControlViewModel.ClearMessageRaised += ClearMessageRaised;
			settingsControlViewModel.ShowMainWindowCommand = ShowMainViewCommand;

			_availableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = PluginResources.PluginsView,
					ViewModel = providerControlViewModel.ViewModel
				},
				new ViewDetails
				{
					Name = PluginResources.SettingsView,
					ViewModel = settingsControlViewModel.ViewModel
				}
			};

			ShowProvidersPage();
		}

		public MainWindowViewModel(ITranslationOptions options, ISettingsControlViewModel settingsControlViewModel, bool isTellMeAction)
		{
			Options = options;
			_isTellMeAction = isTellMeAction;
			_settingsControlViewModel = settingsControlViewModel;
			SaveCommand = new RelayCommand(Save);

			_availableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = PluginResources.SettingsView,
					ViewModel = settingsControlViewModel.ViewModel
				}
			};

			if (_isTellMeAction)
			{
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


		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;


		public ICommand ShowSettingsViewCommand { get; set; }

		public ICommand ShowMainViewCommand { get; set; }

		public ICommand SaveCommand { get; set; }


		public bool IsWindowValid()
		{
			ErrorMessage = string.Empty;
			var isGoogleProvider = _providerControlViewModel.SelectedTranslationOption?.ProviderType == ProviderType.GoogleTranslate;
			if (isGoogleProvider && !ValidGoogleOptions())
			{
				return false;
			}

			return ValidSettingsPageOptions();
		}

		private bool ValidGoogleOptions()
		{
			if (_providerControlViewModel.SelectedGoogleApiVersion.Version != ApiVersion.V2)
			{
				return GoogleV3OptionsAreSet() && AreGoogleV3CredentialsValid();
			}

			if (string.IsNullOrEmpty(_providerControlViewModel.ApiKey))
			{
				ErrorMessage = PluginResources.ApiKeyError;
				return false;
			}

			return AreGoogleV2CredentialsValid();
		}

		private bool GoogleV3OptionsAreSet()
		{
			if (string.IsNullOrEmpty(_providerControlViewModel.JsonFilePath))
			{
				ErrorMessage = PluginResources.EmptyJsonFilePathMsg;
				return false;
			}

			if (!File.Exists(_providerControlViewModel.JsonFilePath))
			{
				ErrorMessage = PluginResources.WrongJsonFilePath;
				return false;
			}

			if (string.IsNullOrEmpty(_providerControlViewModel.ProjectName))
			{
				ErrorMessage = PluginResources.InvalidProjectName;
				return false;
			}

			if (string.IsNullOrEmpty(_providerControlViewModel.ProjectLocation))
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
					ProjectName = _providerControlViewModel.ProjectName,
					JsonFilePath = _providerControlViewModel.JsonFilePath,
					GoogleEngineModel = _providerControlViewModel.GoogleEngineModel,
					ProjectLocation = _providerControlViewModel.ProjectLocation,
					GlossaryPath = _providerControlViewModel.GlossaryPath,
					BasicCsv = _providerControlViewModel.BasicCsvGlossary,
					SelectedProvider = _providerControlViewModel.SelectedTranslationOption.ProviderType,
					SelectedGoogleVersion = _providerControlViewModel.SelectedGoogleApiVersion.Version
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
				var v2Connector = new V2Connector(_providerControlViewModel.ApiKey, _htmlUtil);
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
			if (_settingsControlViewModel.DoPreLookup)
			{
				if (string.IsNullOrEmpty(_settingsControlViewModel.PreLookupFileName))
				{
					ErrorMessage = PluginResources.PreLookupEmptyMessage;
					return false;
				}

				if (!File.Exists(_settingsControlViewModel.PreLookupFileName))
				{
					ErrorMessage = PluginResources.PreLookupWrongPathMessage;
					return false;
				}
			}

			if (!_settingsControlViewModel.DoPostLookup)
			{
				return true;
			}

			if (string.IsNullOrEmpty(_settingsControlViewModel.PostLookupFileName))
			{
				ErrorMessage = PluginResources.PostLookupEmptyMessage;
				return false;
			}

			if (!File.Exists(_settingsControlViewModel.PostLookupFileName))
			{
				ErrorMessage = PluginResources.PostLookupWrongPathMessage;
				return false;
			}

			return true;
		}

		private void Save(object window)
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
			Options.ApiKey = _providerControlViewModel.ApiKey;
			Options.PersistGoogleKey = _providerControlViewModel.PersistGoogleKey;
			Options.SelectedGoogleVersion = _providerControlViewModel.SelectedGoogleApiVersion.Version;
			Options.JsonFilePath = _providerControlViewModel.JsonFilePath;
			Options.ProjectName = _providerControlViewModel.ProjectName;
			Options.SelectedProvider = _providerControlViewModel.SelectedTranslationOption.ProviderType;
			Options.GoogleEngineModel = _providerControlViewModel.GoogleEngineModel;
			Options.ProjectLocation = _providerControlViewModel.ProjectLocation;
			Options.GlossaryPath = _providerControlViewModel.GlossaryPath;
			Options.BasicCsv = _providerControlViewModel.BasicCsvGlossary;
		}
		private void SetGeneralProviderOptions()
		{
			if (_settingsControlViewModel is not null)
			{
				Options.SendPlainTextOnly = _settingsControlViewModel.SendPlainText;
				Options.ResendDrafts = _settingsControlViewModel.ReSendDraft;
				Options.UsePreEdit = _settingsControlViewModel.DoPreLookup;
				Options.PreLookupFilename = _settingsControlViewModel.PreLookupFileName;
				Options.UsePostEdit = _settingsControlViewModel.DoPostLookup;
				Options.PostLookupFilename = _settingsControlViewModel.PostLookupFileName;
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
				Options.LanguagesSupported.Add(languagePair.TargetCultureName, _providerControlViewModel.SelectedTranslationOption.Name);
			}
		}

		private void DeleteCredentialsIfNecessary()
		{
			var isGoogleProvider = _providerControlViewModel.SelectedTranslationOption.ProviderType == ProviderType.GoogleTranslate;
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

		private void ShowSettingsPage()
		{
			SelectedView = _availableViews[1];
		}

		private void ShowProvidersPage()
		{
			SelectedView = _availableViews[0];
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