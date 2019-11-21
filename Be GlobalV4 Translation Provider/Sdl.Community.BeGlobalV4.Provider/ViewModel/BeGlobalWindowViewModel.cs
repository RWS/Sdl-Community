using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Windows.Input;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
    public class BeGlobalWindowViewModel : BaseViewModel
	{
		private MessageBoxService _messageBoxService;
		private TranslationProviderCredential _credentials;
		private int _selectedTabIndex;

		private ICommand _okCommand;

        public event EventHandler OkSelected;

        public BeGlobalWindowViewModel(BeGlobalTranslationOptions options, TranslationProviderCredential credentials, LoginViewModel loginViewModel, LanguageMappingsViewModel languageMappingsViewModel)
		{
			SelectedTabIndex = 0;
			Options = options;
			_credentials = credentials;
			_messageBoxService = new MessageBoxService();
            LanguageMappingsViewModel = languageMappingsViewModel;
			LoginViewModel = loginViewModel;

            if (_credentials == null) return;
			var credential = !string.IsNullOrEmpty(_credentials.Credential) ? _credentials.Credential.Replace("sdlmachinetranslationcloudprovider:///", string.Empty) : string.Empty;
			if (credential.Contains("#"))
			{
				var splitedCredentials = credentials.Credential.Split('#');
				if (splitedCredentials.Length == 2 && !string.IsNullOrEmpty(splitedCredentials[0]) && !string.IsNullOrEmpty(splitedCredentials[1]))
				{
					var beGlobalTranslator = new BeGlobalV4Translator(Options, _messageBoxService, _credentials);
					var userInfo = beGlobalTranslator.GetUserInformation(true);
					var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(userInfo.AccountId.ToString());
					LoginViewModel.GetEngineModels(subscriptionInfo);
					LoginViewModel.SetEngineModel();
                }
			}
		}	
		
        protected virtual void OnOkSelected(EventArgs e)
        {
            var handler = OkSelected;
            handler?.Invoke(this, e);
        }

        public LoginViewModel LoginViewModel { get; set; }
		public LanguageMappingsViewModel LanguageMappingsViewModel { get; set; }
		public BeGlobalTranslationOptions Options { get; set; }			

		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set
			{
				_selectedTabIndex = value;				
				OnPropertyChanged();
			}
		}
		
		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));
		
		private void Ok(object parameter)
		{            
            var loginTab = parameter as Login;
            if (loginTab != null)
            {
                var isValid = IsWindowValid();
                if (isValid)
                {
                    OnOkSelected(EventArgs.Empty);
                }
            }
        }

		private bool IsWindowValid()
		{
			try
			{
				Options.ResendDrafts = LanguageMappingsViewModel.ReSendChecked;
				if (LoginViewModel.LoginMethod.Equals(Constants.APICredentials))
				{
					var clientIdPass = Options?.ClientId;
					var clientSecretPass = Options?.ClientSecret;
					if (string.IsNullOrEmpty(clientIdPass) || string.IsNullOrEmpty(clientSecretPass))
					{
						LoginViewModel.Message = Constants.CredentialsValidation;
						return false;
					}
				}

				var beGlobalTranslator = new BeGlobalV4Translator(Options, _messageBoxService, _credentials);
				var userInfo = beGlobalTranslator.GetUserInformation(false);
				if (userInfo.AccountId != 0)
				{
					if (Options?.Model == null)
					{
						var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(userInfo.AccountId.ToString());
						LoginViewModel.GetEngineModels(subscriptionInfo);
						LoginViewModel.SetEngineModel();
					}
					return true;
				}
				else
				{
					if (userInfo.Errors.Count > 0)
					{
						foreach (var error in userInfo.Errors)
						{
							LoginViewModel.Message += $@"{error.Code}: {error.Description}";
						}
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.IsWindowValid} {ex.Message}\n {ex.StackTrace}");
			}
			return false;
		}
    }
}