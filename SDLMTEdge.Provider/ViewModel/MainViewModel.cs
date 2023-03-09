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

        private bool _dialogResult;
		private ViewDetails _selectedView;
		private List<ViewDetails> _availableViews;
		private ICredentialsViewModel _credentialsViewModel;
		private ILanguageMappingViewModel _languageMappingViewModel;

		private LanguagePair[] _languagePairs;
		private bool _showSettingsView;

		private ICommand _saveCommand;
		private ICommand _signInCommand;

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

		public ICommand SignInCommand => _signInCommand ??= new RelayCommand(SignIn);

        public delegate void CloseWindowEventRaiser();

        public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeViews()
		{
			_credentialsViewModel = new CredentialsViewModel(Options, _languagePairs);
			_languageMappingViewModel = new LanguageMappingViewModel(Options);
			_availableViews = new List<ViewDetails>()
			{
				new ViewDetails()
				{
					Name = ViewsDetails_Credentials,
					ViewModel = _credentialsViewModel.ViewModel
				},
				new ViewDetails()
				{
					Name = ViewsDetails_LanguageMapping,
					ViewModel = _languageMappingViewModel.ViewModel
				}
			};

			SwitchView(_showSettingsView ? ViewsDetails_LanguageMapping : ViewsDetails_Credentials);
		}

		private void SwitchView(object parameter)
		{
			SelectedView = _availableViews.FirstOrDefault(x => x.Name.Equals(parameter))
						?? _availableViews.First();
		}

		private async void SignIn(object parameter)
		{
			Options.ApiVersion = APIVersion.v2;
			try
			{
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
				_languageMappingViewModel.LanguageMapping = languageMapping.ToList();
				SwitchView(ViewsDetails_LanguageMapping);
				ShowSettingsView = true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private GenericCredentials GetCredentals()
			=> new(_credentialsViewModel.UserName, _credentialsViewModel.Password)
			{
				["API-Key"] = _credentialsViewModel.ApiKey,
				["UseApiKey"] = _credentialsViewModel.UseBasicCredentials ? "false" : "true",
				["RequiresSecureProtocol"] = _credentialsViewModel.RequiresSecureProtocol ? "true" : "false"
			};

		private void Save(object parameter)
        {
            DialogResult = true;
			CloseEventRaised?.Invoke();
		}

		private ICommand _cancelCommand;
		public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(Cancel);
		private void Cancel(object parameter)
		{
			CloseEventRaised?.Invoke();
		}
	}
}