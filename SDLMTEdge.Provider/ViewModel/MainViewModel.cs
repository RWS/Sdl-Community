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
		// private const string ViewsDetails_Settings = nameof(SettingsViewModel);

		private readonly ITranslationProviderCredentialStore _credentialStore;

        private bool _dialogResult;
		private ViewDetails _selectedView;
		private List<ViewDetails> _availableViews;
		private ICredentialsViewModel _credentialsViewModel;

		private bool _showSettingsView;

		private ICommand _saveCommand;
		private ICommand _loginCommand;

        public MainViewModel(ITranslationOptions options,
                             ITranslationProviderCredentialStore credentialStore,
                             LanguagePair[] languagePairs,
                             bool showSettingsView = false)
        {
			Options = options;
			ShowSettingsView = showSettingsView;
			_credentialStore = credentialStore;
			InitializeViews();
			//SwitchView(_showSettingsView ? ViewsDetails_Settings : ViewsDetails_Credentials);
			SwitchView(ViewsDetails_Credentials);
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

		public ICommand LoginCommand => _loginCommand ??= new RelayCommand(Login);

        public delegate void CloseWindowEventRaiser();

        public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeViews()
		{
			_credentialsViewModel = new CredentialsViewModel(Options);
			_availableViews = new List<ViewDetails>()
			{
				new ViewDetails()
				{
					Name = ViewsDetails_Credentials,
					ViewModel = _credentialsViewModel.ViewModel
				}
			};
		}

		private void SwitchView(object parameter)
		{
			_selectedView = _availableViews.First();
		}

		private void Save(object parameter)
        {
            DialogResult = true;
        }

		private void Login(object parameter)
		{
			string token;
			try
			{
				if (_credentialsViewModel.UseRwsCredentials)
				{
					token = SDLMTEdgeTranslatorHelper.GetAuthToken(Options as TranslationOptions, GetCredentals());
				}
				else
				{
					token = _credentialsViewModel.ApiKey;
					SDLMTEdgeTranslatorHelper.VerifyBasicAPIToken(Options as TranslationOptions, GetCredentals());
				}

				Options.ApiToken = token;
				return;
			}
			catch
			{
				//return false;
				return;
			}
		}

		private GenericCredentials GetCredentals()
		{
			return new GenericCredentials(_credentialsViewModel.UserName, _credentialsViewModel.Password)
			{
				["API-Key"] = _credentialsViewModel.ApiKey,
				["UseApiKey"] = _credentialsViewModel.UseRwsCredentials ? "false" : "true",
				["RequiresSecureProtocol"] = _credentialsViewModel.RequiresSecureProtocol ? "true" : "false"
			};
		}
	}
}