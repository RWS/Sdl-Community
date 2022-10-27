using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Commands;
using Sdl.Community.MtEnhancedProvider.GoogleApi;
using Sdl.Community.MtEnhancedProvider.Helpers;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.MstConnect;
using Sdl.Community.MtEnhancedProvider.Service;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class MainWindowViewModel : ModelBase, IMainWindow
	{
		private ViewDetails _selectedView;
		private bool _dialogResult;
		private readonly bool _isTellMeAction;
		private string _errorMessage;
		private string _translatorErrorResponse;
		private readonly IProviderControlViewModel _providerControlViewModel;
		private readonly ISettingsControlViewModel _settingsControlViewModel;
		private readonly ITranslationProviderCredentialStore _credentialStore;
		private readonly LanguagePair[] _languagePairs;
		private readonly HtmlUtil _htmlUtil;
		public delegate void CloseWindowEventRaiser();
		public event CloseWindowEventRaiser CloseEventRaised;

		public MainWindowViewModel(IMtTranslationOptions options, IProviderControlViewModel providerControlViewModel,
			ISettingsControlViewModel settingsControlViewModel,
			ITranslationProviderCredentialStore credentialStore, LanguagePair[] languagePairs, HtmlUtil htmlUtil)
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

			AvailableViews = new List<ViewDetails>
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

		public MainWindowViewModel(IMtTranslationOptions options, ISettingsControlViewModel settingsControlViewModel, bool isTellMeAction)
		{
			Options = options;
			_isTellMeAction = isTellMeAction;
			_settingsControlViewModel = settingsControlViewModel;
			SaveCommand = new RelayCommand(Save);

			AvailableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = PluginResources.SettingsView,
					ViewModel = settingsControlViewModel.ViewModel
				}
			};

			if (_isTellMeAction)
			{
				SelectedView = AvailableViews[0];
			}
		}

		public ViewDetails SelectedView
		{
			get => _selectedView;
			set
			{
				_selectedView = value;
				ErrorMessage = string.Empty;
				OnPropertyChanged(nameof(SelectedView));
			}
		}

		public List<ViewDetails> AvailableViews { get; set; }
		public ICommand ShowSettingsViewCommand { get; set; }
		public ICommand ShowMainViewCommand { get; set; }
		public ICommand SaveCommand { get; set; }
		public IMtTranslationOptions Options { get; set; }

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

		public void AddEncriptionMetaToResponse(string errorMessage)
		{
			var htmlStart = "<html> \n <meta http-equiv=\'Content-Type\' content=\'text/html;charset=UTF-8\'>\n <body style=\"font-family:Segoe Ui!important;color:red!important;font-size:13px!important\">\n";

			TranslatorErrorResponse = $"{errorMessage.Insert(0, htmlStart)}\n</body></html>";
		}

		public bool IsWindowValid()
		{
			ErrorMessage = string.Empty;
			//TranslatorErrorResponse = "<html><body></html></body>"; //Clear web browser content

			switch (_providerControlViewModel.SelectedTranslationOption.ProviderType)
			{
				case MtTranslationOptions.ProviderType.MicrosoftTranslator:
					if (!ValidMicrosoftOptions()) return false;
					break;
				case MtTranslationOptions.ProviderType.MicrosoftTranslatorWithPe:
					if (!ValidMicrosoftOptions()) return false;
					break;
				case MtTranslationOptions.ProviderType.GoogleTranslate:
					if (!ValidGoogleOptions()) return false;
					break;
			}
			if (ValidSettingsPageOptions())
			{
				return true;
			}
			return false;
		}

		private bool ValidGoogleOptions()
		{
			if (_providerControlViewModel.SelectedGoogleApiVersion.Version == Enums.GoogleApiVersion.V2)
			{
				if (string.IsNullOrEmpty(_providerControlViewModel.ApiKey))
				{
					ErrorMessage = PluginResources.ApiKeyError;
					return false;
				}
				return AreGoogleV2CredentialsValid();
			}
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
			return AreGoogleV3CredentialsValid();
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
			if (!_settingsControlViewModel.DoPostLookup) return true;
			if (string.IsNullOrEmpty(_settingsControlViewModel.PostLookupFileName))
			{
				ErrorMessage = PluginResources.PostLookupEmptyMessage;
				return false;
			}
			if (File.Exists(_settingsControlViewModel.PostLookupFileName)) return true;
			ErrorMessage = PluginResources.PostLookupWrongPathMessage;
			return false;
		}

		private bool ValidMicrosoftOptions()
		{
			if (_providerControlViewModel.SelectedTranslationOption.ProviderType == MtTranslationOptions.ProviderType.MicrosoftTranslatorWithPe
				&& string.IsNullOrEmpty(_providerControlViewModel.PeUrl))
			{
				ErrorMessage = PluginResources.PeUrlError;
				return false;
			}

			if (string.IsNullOrEmpty(_providerControlViewModel.ClientId))
			{
				ErrorMessage = PluginResources.ApiKeyError;
				return false;
			}
			if (_providerControlViewModel.UseCatId && string.IsNullOrEmpty(_providerControlViewModel.CatId))
			{
				ErrorMessage = PluginResources.CatIdError;
				return false;
			}
			return AreMicrosoftCredentialsValid();
		}

		private void ShowSettingsPage()
		{
			SelectedView = AvailableViews[1];
		}

		private void ShowProvidersPage()
		{
			SelectedView = AvailableViews[0];
		}

		private void ClearMessageRaised()
		{
			ErrorMessage = string.Empty;
			TranslatorErrorResponse = "<html><body></html></body>"; //Clear web browser content
		}

		private void Save(object window)
		{
			if (_isTellMeAction)
			{
				SetGeneralProviderOptions();
				DialogResult = true;
				CloseEventRaised?.Invoke();
			}
			else
			{
				if (!IsWindowValid()) return;
				SetMicrosoftProviderOptions();
				SetGoogleProviderOptions();
				SetGeneralProviderOptions();
				DeleteCredentialsIfNecessary();
				DialogResult = true;
				CloseEventRaised?.Invoke();
			}
		}

		private void DeleteCredentialsIfNecessary()
		{
			if (_providerControlViewModel.SelectedTranslationOption.ProviderType ==
				MtTranslationOptions.ProviderType.MicrosoftTranslator && !Options.PersistMicrosoftCreds)
			{
				RemoveCredentialsFromStore(new Uri(PluginResources.UriMs));
			}
			if (_providerControlViewModel.SelectedTranslationOption.ProviderType ==
				MtTranslationOptions.ProviderType.MicrosoftTranslatorWithPe && !Options.PersistMicrosoftCreds)
			{
				RemoveCredentialsFromStore(new Uri(PluginResources.UriMs));
			}
			if (_providerControlViewModel.SelectedTranslationOption.ProviderType ==
				MtTranslationOptions.ProviderType.GoogleTranslate && !Options.PersistGoogleKey)
			{
				RemoveCredentialsFromStore(new Uri(PluginResources.UriGt));
			}
		}

		private void RemoveCredentialsFromStore(Uri providerUri)
		{
			var credentials = _credentialStore.GetCredential(providerUri);
			if (credentials != null)
			{
				_credentialStore.RemoveCredential(providerUri);
			}
		}

		private bool AreMicrosoftCredentialsValid()
		{
			try
			{
				if (_providerControlViewModel.SelectedTranslationOption.ProviderType == MtTranslationOptions.ProviderType.MicrosoftTranslatorWithPe)
				{
					var apiConnecter = new ApiConnecterWithPe(_providerControlViewModel.PeUrl,
						_providerControlViewModel.ClientId, _providerControlViewModel.Region?.Key, _htmlUtil);

					if (!string.IsNullOrEmpty(Options?.PeUrl) && !string.IsNullOrEmpty(Options?.ClientId))
					{
						apiConnecter.EnsureConnectivity();
					}

					return true;
				}
				else if(_providerControlViewModel.SelectedTranslationOption.ProviderType == MtTranslationOptions.ProviderType.MicrosoftTranslator)
				{
					var apiConnecter = new ApiConnecter(
						_providerControlViewModel.ClientId, _providerControlViewModel.Region?.Key, _htmlUtil);

					if (!string.IsNullOrEmpty(Options?.ClientId))
					{
						if (!Options.ClientId.Equals(_providerControlViewModel.ClientId))
						{
							apiConnecter.RefreshAuthToken();
						}
					}

					return true;
				}
			}
			catch (Exception e)
			{
				AddEncriptionMetaToResponse(e.Message);
			}
			return false;
		}

		private bool AreGoogleV3CredentialsValid()
		{
			try
			{
				var providerOptions = new MtTranslationOptions
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
				var googleV3 = new GoogleV3Connecter(providerOptions);
				googleV3.TryToAuthenticateUser();
				if (!string.IsNullOrEmpty(providerOptions.GlossaryPath) && _languagePairs != null)
				{
					googleV3.CreateGoogleGlossary(_languagePairs);
				}
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
			return true;
		}

		private bool AreGoogleV2CredentialsValid()
		{
			try
			{
				var googleApiConecter = new MtTranslationProviderGTApiConnecter(_providerControlViewModel.ApiKey, _htmlUtil);
				googleApiConecter.ValidateCredentials();

				return true;
			}
			catch (Exception e)
			{
				AddEncriptionMetaToResponse(e.Message);
			}
			return false;
		}

		private void SetGeneralProviderOptions()
		{
			if (_settingsControlViewModel != null)
			{
				Options.SendPlainTextOnly = _settingsControlViewModel.SendPlainText;
				Options.ResendDrafts = _settingsControlViewModel.ReSendDraft;
				Options.UsePreEdit = _settingsControlViewModel.DoPreLookup;
				Options.PreLookupFilename = _settingsControlViewModel.PreLookupFileName;
				Options.UsePostEdit = _settingsControlViewModel.DoPostLookup;
				Options.PostLookupFilename = _settingsControlViewModel.PostLookupFileName;
			}

			if (Options != null && Options.LanguagesSupported == null)
			{
				Options.LanguagesSupported = new Dictionary<string, string>();
			}
			if (_languagePairs == null) return;
			foreach (var languagePair in _languagePairs)
			{
				Options?.LanguagesSupported.Add(languagePair.TargetCultureName, _providerControlViewModel.SelectedTranslationOption.Name);
			}
		}

		private void SetGoogleProviderOptions()
		{
			//Google options-V2
			Options.ApiKey = _providerControlViewModel.ApiKey;
			Options.PersistGoogleKey = _providerControlViewModel.PersistGoogleKey;
			Options.SelectedGoogleVersion = _providerControlViewModel.SelectedGoogleApiVersion.Version;

			//Google options-V3
			Options.JsonFilePath = _providerControlViewModel.JsonFilePath;
			Options.ProjectName = _providerControlViewModel.ProjectName;
			Options.SelectedProvider = _providerControlViewModel.SelectedTranslationOption.ProviderType;
			Options.GoogleEngineModel = _providerControlViewModel.GoogleEngineModel;
			Options.ProjectLocation = _providerControlViewModel.ProjectLocation;
			Options.GlossaryPath = _providerControlViewModel.GlossaryPath;
			Options.BasicCsv = _providerControlViewModel.BasicCsvGlossary;
		}

		private void SetMicrosoftProviderOptions()
		{
			Options.ClientId = _providerControlViewModel.ClientId;
			Options.PeUrl = _providerControlViewModel.PeUrl;
			Options.Region = _providerControlViewModel.Region.Key;
			Options.UseCatID = _providerControlViewModel.UseCatId;
			Options.CatId = _providerControlViewModel.CatId;
			Options.PersistMicrosoftCreds = _providerControlViewModel.PersistMicrosoftKey;
		}
	}
}
