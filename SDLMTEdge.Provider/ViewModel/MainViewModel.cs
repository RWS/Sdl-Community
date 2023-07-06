using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.MTEdge.Provider.Command;
using Sdl.Community.MTEdge.Provider.Helpers;
using Sdl.Community.MTEdge.Provider.Interface;
using Sdl.Community.MTEdge.Provider.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTEdge.Provider.ViewModel
{
	public class MainViewModel : BaseModel
	{
		private const string ViewsDetails_Credentials = nameof(CredentialsViewModel);
		private const string ViewsDetails_LanguageMapping = nameof(LanguageMappingViewModel);

		private readonly ITranslationProviderCredentialStore _credentialStore;
		private readonly LanguagePair[] _languagePairs;

		private bool _dialogResult;
		private ViewDetails _selectedView;
		private List<ViewDetails> _availableViews;
		private ICredentialsViewModel _credentialsViewModel;
		private ILanguageMappingViewModel _languageMappingViewModel;

		private bool _showSettingsView;
		private string _headerTitle;

		private ICommand _saveCommand;
		private ICommand _signInCommand;
		private ICommand _cancelCommand;

		public MainViewModel(ITranslationOptions options,
							 ITranslationProviderCredentialStore credentialStore,
							 LanguagePair[] languagePairs,
							 bool showSettingsView = false)
		{
			Options = options;
			ShowSettingsView = showSettingsView;
			_credentialStore = credentialStore;
			_languagePairs = languagePairs;
			InitializeViews();
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

		public string HeaderTitle
		{
			get => _headerTitle;
			set
			{
				if (_headerTitle == value) return;
				_headerTitle = value;
				OnPropertyChanged(nameof(HeaderTitle));
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

		public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);

		public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(Cancel);

		public ICommand SignInCommand => _signInCommand ??= new RelayCommand(SignIn);


		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeViews()
		{
			_credentialsViewModel = new CredentialsViewModel(Options, ShowSettingsView);
			_languageMappingViewModel = new LanguageMappingViewModel(Options);
			_availableViews = new List<ViewDetails>()
			{
				new ViewDetails()
				{
					Name = ViewsDetails_Credentials,
					ViewModel = _credentialsViewModel as BaseModel
				},
				new ViewDetails()
				{
					Name = ViewsDetails_LanguageMapping,
					ViewModel = _languageMappingViewModel as BaseModel
				}
			};

			SwitchView(_showSettingsView ? ViewsDetails_LanguageMapping : ViewsDetails_Credentials);
			SetCredentialsOnUI();
			if (ShowSettingsView)
			{
				SignIn(null);
			}
		}

		private void SwitchView(object parameter)
		{
			SelectedView = _availableViews.FirstOrDefault(x => x.Name.Equals(parameter))
						?? _availableViews.First();
			HeaderTitle = SelectedView.Name.Equals(ViewsDetails_Credentials) ? "Authentication" : "Language Pairs";
		}

		private void Save(object parameter)
		{
			var uriIsValid = _credentialsViewModel.UriIsValid();
			var credentialsAreValid = _credentialsViewModel.CredentialsAreValid(_showSettingsView);
			if (!uriIsValid || !credentialsAreValid)
			{
				if (!credentialsAreValid)
				{
					Options.ApiToken = string.Empty;
					ShowSettingsView = false;
					SwitchView(ViewsDetails_Credentials);
				}

				return;
			}

			DialogResult = true;
			Options.LanguageMapping = new(_languageMappingViewModel.LanguageMapping);
			Options.SetLanguageMapping(new(_languageMappingViewModel.LanguageMapping));
			CloseEventRaised?.Invoke();
		}

		private void Cancel(object parameter)
		{
			CloseEventRaised?.Invoke();
		}

		private async void SignIn(object parameter)
		{
			Options.ApiVersion = APIVersion.v2;
			try
			{
				if (!_credentialsViewModel.UriIsValid()
				 || !_credentialsViewModel.CredentialsAreValid())
				{
					return;
				}

				if (_credentialsViewModel.UseBasicCredentials)
				{
					Options.ApiToken = SDLMTEdgeTranslatorHelper.GetAuthToken(Options as TranslationOptions, GetCredentals());
				}
				else if (_credentialsViewModel.UseAuth0SSO)
				{
					Options.ApiToken = await SDLMTEdgeTranslatorHelper.SignInAuthAsync(Options as TranslationOptions);
				}
				else if (_credentialsViewModel.UseApiKey)
				{
					Options.ApiToken = _credentialsViewModel.ApiKey;
					SDLMTEdgeTranslatorHelper.VerifyBasicAPIToken(Options as TranslationOptions, GetCredentals());
				}

				var languageMapping = Options.SetPreferredLanguages(_languagePairs);
				
				Options.SetDictionaries(languageMapping);
				_languageMappingViewModel.LanguageMapping = new(languageMapping);
				if (ShowSettingsView)
				{
					for (var i = 0; i < Options.LanguagePairPreferences.Count; i++)
					{
						var selectedModel = _languageMappingViewModel.LanguageMapping[i].MtEdgeLanguagePairs.First(x => x.LanguagePairId == Options.LanguagePairPreferences.Skip(i).First().Value.LanguagePairId);
						_languageMappingViewModel.LanguageMapping[i].SelectedModelIndex = _languageMappingViewModel.LanguageMapping[i].MtEdgeLanguagePairs.IndexOf(selectedModel);

						var selectedDictionary = _languageMappingViewModel.LanguageMapping[i].Dictionaries.First(x => x.DictionaryId == Options.LanguagePairPreferences.Skip(i).First().Value.DictionaryId);
						_languageMappingViewModel.LanguageMapping[i].SelectedDictionaryIndex = _languageMappingViewModel.LanguageMapping[i].Dictionaries.IndexOf(selectedDictionary);
					}
				}
				
				SwitchView(ViewsDetails_LanguageMapping);
				ShowSettingsView = true;
				
				SaveCredentials();
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleError(ex.Message, "Connection failed");
			}
		}

		private GenericCredentials GetCredentals()
			=> new(_credentialsViewModel.UserName, _credentialsViewModel.Password)
			{
				["API-Key"] = _credentialsViewModel.ApiKey,
				["UseApiKey"] = _credentialsViewModel.UseBasicCredentials ? "false" : "true",
				["RequiresSecureProtocol"] = _credentialsViewModel.RequiresSecureProtocol ? "true" : "false"
			};

		private TranslationProviderCredential SaveCredentials()
		{
			try
			{
				var uri = new TranslationProviderUriBuilder(Constants.TranslationProviderScheme);

				var credential = _credentialStore.GetCredential(uri.Uri);
				if (credential?.Credential != null)
				{
					_credentialStore.RemoveCredential(uri.Uri);
				}

				var persistsHost = _credentialsViewModel.PersistsHost;
				var persistsCredentials = _credentialsViewModel.PersistsCredentials;
				var persistsApiKey = _credentialsViewModel.PersistsApiKey;
				var currentCredentials = new GenericCredentials(_credentialsViewModel.UserName, _credentialsViewModel.Password)
				{
					["Persists-Host"] = persistsHost.ToString().ToLower(),
					["Host"] = persistsHost ? _credentialsViewModel.Host : string.Empty,
					["Port"] = persistsHost ? _credentialsViewModel.Port : string.Empty,

					["Persists-Credentials"] = persistsCredentials.ToString().ToLower(),
					["UserName"] = persistsCredentials ? _credentialsViewModel.UserName : string.Empty,
					["Password"] = persistsCredentials ? _credentialsViewModel.Password : string.Empty,

					["Persists-ApiKey"] = persistsApiKey.ToString().ToLower(),
					["API-Key"] = _credentialsViewModel.ApiKey,

					["UseApiKey"] = _credentialsViewModel.UseApiKey.ToString().ToLower(),
					["RequiresSecureProtocol"] = _credentialsViewModel.RequiresSecureProtocol.ToString().ToLower(),

					["Token"] = Options.ApiToken
				};

				var credentials = new TranslationProviderCredential(currentCredentials.ToString(), true);
				_credentialStore.AddCredential(uri.Uri, credentials);

				return credential;
			}
			catch
			{
				// catch all; ignore
			}

			return null;
		}

		private void SetCredentialsOnUI()
		{
			try
			{
				var uri = new TranslationProviderUriBuilder(Constants.TranslationProviderScheme);
				var credential = _credentialStore.GetCredential(uri.Uri);
				if (credential?.Credential == null)
				{
					credential = SaveCredentials();

					if (credential?.Credential == null)
					{
						return;
					}
				}

				var genericCredentials = new GenericCredentials(credential.Credential);

				bool.TryParse(genericCredentials["Persists-Host"], out var persistsHost);
				_credentialsViewModel.PersistsHost = persistsHost;
				_credentialsViewModel.Host = GetCredential(genericCredentials, "Host", persistsHost);
				var port = GetCredential(genericCredentials, "Port", persistsHost);
				_credentialsViewModel.Port = string.IsNullOrEmpty(port) ? Options.Port.ToString() : port;

				bool.TryParse(genericCredentials["Persists-Credentials"], out var persistsCredentials);
				_credentialsViewModel.PersistsCredentials = persistsCredentials;
				_credentialsViewModel.UserName = GetCredential(genericCredentials, "UserName", persistsCredentials);
				_credentialsViewModel.Password = GetCredential(genericCredentials, "Password", persistsCredentials);

				bool.TryParse(genericCredentials["Persists-ApiKey"], out var persistsApiKey);
				_credentialsViewModel.PersistsApiKey = persistsApiKey;
				_credentialsViewModel.ApiKey = GetCredential(genericCredentials, "API-Key", persistsApiKey);

				bool.TryParse(genericCredentials["RequiresSecureProtocol"], out var requiresSecureProtocol);
				_credentialsViewModel.RequiresSecureProtocol = requiresSecureProtocol;

				Options.ApiToken = _showSettingsView ? genericCredentials["Token"] : string.Empty;
			}
			catch
			{
				// catch all; ignore
			}
		}

		private string GetCredential(GenericCredentials credentials, string target, bool persists)
		{
			return _showSettingsView ? credentials[target] : persists ? credentials[target] : string.Empty;
		}
	}
}