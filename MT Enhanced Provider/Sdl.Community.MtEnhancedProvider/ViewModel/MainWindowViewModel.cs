using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Commands;
using Sdl.Community.MtEnhancedProvider.Helpers;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.MstConnect;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class MainWindowViewModel: ModelBase, IMainWindow
	{
		private ViewDetails _selectedView;
		private bool _dialogResult;
		private string _errorMessage;
		private string _translatorErrorResponse;
		private readonly IProviderControlViewModel _providerControlViewModel;
		private readonly ISettingsControlViewModel _settingsControlViewModel;
		private readonly LanguagePair[] _languagePairs;

		public delegate void CloseWindowEventRaiser();
		public event CloseWindowEventRaiser CloseEventRaised;

		public MainWindowViewModel(IMtTranslationOptions options,IProviderControlViewModel providerControlViewModel,ISettingsControlViewModel settingsControlViewModel, 
			LanguagePair[] languagePairs)
		{
			Options = options;
			_providerControlViewModel = providerControlViewModel;
			_settingsControlViewModel = settingsControlViewModel;
			_languagePairs = languagePairs;
			SaveCommand = new RelayCommand(Save);
			ShowSettingsViewCommand =  new CommandHandler(ShowSettingsPage, true);
			ShowMainViewCommand = new CommandHandler(ShowProvidersPage,true);

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
		public IMtTranslationOptions Options { get;set; }

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

			switch (_providerControlViewModel.SelectedTranslationOption.ProviderType)
			{
				case MtTranslationOptions.ProviderType.MicrosoftTranslator:
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
			return  false;
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
			}
			else
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
				}
			}
			return true;
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
		}

		private void Save(object window)
		{
			if (IsWindowValid())
			{
				SetMicrosoftProviderOptions();
				SetGoogleProviderOptions();

				SetGeneralProviderOptions();

				DialogResult = true;
				CloseEventRaised?.Invoke();
			}
		}

		private bool AreMicrosoftCredentialsValid()
		{
			try
			{
				if (_providerControlViewModel.SelectedTranslationOption.ProviderType ==
				    MtTranslationOptions.ProviderType.MicrosoftTranslator)
				{

					var apiConnecter = new ApiConnecter(_providerControlViewModel.ClientId);

					if (!string.IsNullOrEmpty(Options?.ClientId))
					{
						if (!Options.ClientId.Equals(_providerControlViewModel.ClientId))
						{
							apiConnecter.RefreshAuthToken();
						}
					}

					if (Options == null) return true;
					var allSupportedLanguages = ApiConnecter.SupportedLangs;
					var correspondingLanguages = _languagePairs
						.Where(lp => allSupportedLanguages.Contains(lp.TargetCultureName.Substring(0, 2))).ToList();

					Options.LanguagesSupported = correspondingLanguages.ToDictionary(lp => lp.TargetCultureName,
						lp => Options.SelectedProvider.ToString());

					return true;
				}
			}
			catch (Exception e)
			{
				AddEncriptionMetaToResponse(e.Message);
			}
			return false;
		}

		private void SetGeneralProviderOptions()
		{
			Options.SendPlainTextOnly = _settingsControlViewModel.SendPlainText;
			Options.ResendDrafts = _settingsControlViewModel.ReSendDraft;
			Options.UsePreEdit = _settingsControlViewModel.DoPreLookup;
			Options.PreLookupFilename = _settingsControlViewModel.PreLookupFileName;
			Options.UsePostEdit = _settingsControlViewModel.DoPostLookup;
			Options.PostLookupFilename = _settingsControlViewModel.PostLookupFileName;
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
		}

		private void SetMicrosoftProviderOptions()
		{
			Options.ClientId = _providerControlViewModel.ClientId;
			Options.UseCatID = _providerControlViewModel.UseCatId;
			Options.CatId = _providerControlViewModel.CatId;
			Options.PersistMicrosoftCreds = _providerControlViewModel.PersistMicrosoftKey;
		}
	}
}
