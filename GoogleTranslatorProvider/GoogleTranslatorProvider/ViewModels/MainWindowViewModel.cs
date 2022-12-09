using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

		private ICommand _navigateToCommand;
		private ICommand _switchViewCommand;
		private ICommand _saveCommand;

		public MainWindowViewModel(ITranslationOptions options,
								   ITranslationProviderCredentialStore credentialStore,
								   LanguagePair[] languagePairs,
								   bool showSettingsView = false)
		{
			Options = options;
			_credentialStore = credentialStore;
			_languagePairs = languagePairs;
			_htmlUtil = new HtmlUtil();

			InitializeViews();
			SwitchView(showSettingsView ? ViewDetails_Settings : ViewDetails_Provider);
			_providerViewModel.ClearMessageRaised += ClearMessageRaised;
		}

		private void InitializeViews()
		{
			_providerViewModel = new ProviderViewModel(Options);
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

		public bool IsTellMeAction
		{
			get => _isTellMeAction;
			set
			{
				if (_isTellMeAction == value) return;
				_isTellMeAction = value;
				OnPropertyChanged(nameof(IsTellMeAction));
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
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
				ErrorMessage = string.Empty;
			}
		}


		public ICommand SwitchViewCommand => _switchViewCommand ??= new RelayCommand(SwitchView);

		public ICommand NavigateToCommand => _navigateToCommand ??= new RelayCommand(NavigateTo);

		public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;


		public bool IsWindowValid()
		{
			ErrorMessage = string.Empty;
			var isGoogleProvider = _providerViewModel?.SelectedTranslationOption?.ProviderType == ProviderType.GoogleTranslate;
			if (isGoogleProvider && !ValidGoogleOptions())
			{
				return false;
			}

			return ValidSettingsPageOptions();
		}

		private bool ValidGoogleOptions()
		{
			return _providerViewModel.IsV2Checked ? AreGoogleV2CredentialsValid()
												  : GoogleV3OptionsAreSet() && AreGoogleV3CredentialsValid();
		}

		private bool GoogleV3OptionsAreSet()
		{
			if (string.IsNullOrEmpty(_providerViewModel.JsonFilePath))
			{
				ErrorMessage = PluginResources.Validation_EmptyJsonFilePath;
				return false;
			}
			else if (!File.Exists(_providerViewModel.JsonFilePath))
			{
				ErrorMessage = PluginResources.Validation_MissingJsonFile;
				return false;
			}
			else if (string.IsNullOrEmpty(_providerViewModel.ProjectId))
			{
				ErrorMessage = PluginResources.Validation_ProjectID_Empty;
				return false;
			}
			else if (string.IsNullOrEmpty(_providerViewModel.ProjectLocation))
			{
				ErrorMessage = PluginResources.Validation_Location_Empty;
				return false;
			}
			else if (_providerViewModel.UseCustomModel
				  && string.IsNullOrEmpty(_providerViewModel.GoogleEngineModel?.Trim()))
			{
				ErrorMessage = PluginResources.Validation_CustomModel_EnabledEmpty;
				return false;
			}
			else if (_providerViewModel.UseGlossary
				  && string.IsNullOrEmpty(_providerViewModel.GlossaryPath?.Trim()))
			{
				ErrorMessage = PluginResources.Validation_Glossaries_EnabledEmpty;
				return false;
			}

			return true;
		}

		private bool AreGoogleV3CredentialsValid()
		{
			try
			{
				var customModel = _providerViewModel.UseCustomModel ? _providerViewModel.GoogleEngineModel : null;
				var glossaryPath = _providerViewModel.UseGlossary ? _providerViewModel.GlossaryPath : null;
				var basicGlossary = _providerViewModel.UseGlossary && _providerViewModel.BasicCsvGlossary;
				var providerOptions = new GTPTranslationOptions
				{
					ProjectId = _providerViewModel.ProjectId,
					JsonFilePath = _providerViewModel.JsonFilePath,
					GoogleEngineModel = customModel,
					ProjectLocation = _providerViewModel.ProjectLocation,
					GlossaryPath = glossaryPath,
					BasicCsv = basicGlossary,
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
					message = PluginResources.Validation_ModelName_Invalid;
				}
				else if (e.Message.Contains("Invalid resource name"))
				{
					message = PluginResources.Validation_ProjectID_Failed;
				}
				else if (e.Message.Contains("The model"))
				{
					message = "Wrong custom model";
				}
				else if (e.Message.Contains("Glossary not found"))
				{
					message = "Wrong glossary";
				}
				else if (e.Message.Contains("PermissionDenied"))
				{
					message = PluginResources.Validation_PermissionDenied;
				}
				else if (e.Message.Contains("Unsupported location"))
				{
					// NOTE: Confirm if we are going to let the user know the available locations for his account.
					/*var availableLocations = e.Message.Substring(e.Message.LastIndexOf("Must be"));
					availableLocations = availableLocations.Substring(availableLocations.IndexOf('\''));
					availableLocations = availableLocations.Substring(0, availableLocations.IndexOf('.')).Replace("\'", "");
					message = string.Format(PluginResources.Validation_Location_Failed, availableLocations);*/
					message = "This location is not valid for your project.";
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
			if (string.IsNullOrEmpty(_providerViewModel.ApiKey))
			{
				ErrorMessage = PluginResources.Validation_ApiKey;
				return false;
			}

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

			if (_settingsViewModel.DoPreLookup && string.IsNullOrEmpty(_settingsViewModel.PreLookupFileName))
			{
				ErrorMessage = PluginResources.PreLookupEmptyMessage;
				return false;
			}

			if (_settingsViewModel.DoPreLookup && !File.Exists(_settingsViewModel.PreLookupFileName))
			{
				ErrorMessage = PluginResources.PreLookupWrongPathMessage;
				return false;
			}

			if (_settingsViewModel.DoPostLookup && string.IsNullOrEmpty(_settingsViewModel.PostLookupFileName))
			{
				ErrorMessage = PluginResources.PostLookupEmptyMessage;
				return false;
			}

			if (_settingsViewModel.DoPostLookup && !File.Exists(_settingsViewModel.PostLookupFileName))
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
			var customModel = _providerViewModel.UseCustomModel ? _providerViewModel.GoogleEngineModel : null;
			var glossaryPath = _providerViewModel.UseGlossary ? _providerViewModel.GlossaryPath : null;
			var basicGlossary = _providerViewModel.UseGlossary && _providerViewModel.BasicCsvGlossary;
			Options.ApiKey = _providerViewModel.ApiKey;
			Options.PersistGoogleKey = _providerViewModel.PersistGoogleKey;
			Options.SelectedGoogleVersion = _providerViewModel.SelectedGoogleApiVersion.Version;
			Options.JsonFilePath = _providerViewModel.JsonFilePath;
			Options.ProjectId = _providerViewModel.ProjectId;
			Options.SelectedProvider = _providerViewModel.SelectedTranslationOption.ProviderType;
			Options.GoogleEngineModel = customModel;
			Options.ProjectLocation = _providerViewModel.ProjectLocation;
			Options.GlossaryPath = glossaryPath;
			Options.BasicCsv = basicGlossary;
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
			ErrorMessage = string.Empty;
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
			catch { }
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

		private void AddEncriptionMetaToResponse(string errorMessage)
		{
			var htmlStart = @"<html> 
 <meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>
 <body style=""font-family:Segoe Ui!important;color:red!important;font-size:13px!important"">
";
			TranslatorErrorResponse = $"{errorMessage.Insert(0, htmlStart)}\n</body></html>";
		}

		private void NavigateTo(object o)
		{
			string uriTarget;
			var currentVersion = _providerViewModel?.SelectedGoogleApiVersion?.Version;
			if (IsTellMeAction)
			{
				uriTarget = Constants.SettingsDocumentation;
			}
			else if (currentVersion == ApiVersion.V2)
			{
				uriTarget = Constants.V2Documentation;
			}
			else if (currentVersion == ApiVersion.V3)
			{
				uriTarget = Constants.V3Documentation;
			}
			else
			{
				uriTarget = Constants.FullDocumentation;
			}

			Process.Start(uriTarget);
		}
	}
}