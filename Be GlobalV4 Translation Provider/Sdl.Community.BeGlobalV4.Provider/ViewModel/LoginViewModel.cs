using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Application = System.Windows.Application;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class LoginViewModel : BaseViewModel
	{
		private Authentication _selectedAuthentication;
		private string _loginMethod;
		private readonly StudioCredentials _studioCredentials = new StudioCredentials();
		private string _message;
		private int _selectedIndex;
		private TranslationProviderCredential _credentials;
		private readonly LanguagePair[] _languagePairs;
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;

		private ICommand _passwordChangedCommand;

        public event EventHandler AuthenticationSelected;

        public LoginViewModel(
			BeGlobalTranslationOptions options,
			TranslationProviderCredential credentials,
			LanguagePair[] languagePairs,
			LanguageMappingsViewModel languageMappingsViewModel)
		{
			Options = options;
			_credentials = credentials;
			_languagePairs = languagePairs;
			LanguageMappingsViewModel = languageMappingsViewModel;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
			_loginMethod = !string.IsNullOrEmpty(Options.AuthenticationMethod) ? Options.AuthenticationMethod : Constants.APICredentials;
			SetAuthentications();
		}

        protected virtual void OnAuthenticationSelected(EventArgs e)
        {
            EventHandler handler = AuthenticationSelected;
            handler?.Invoke(this, e);
        }

        public BeGlobalTranslationOptions Options { get; set; }
		public LanguageMappingsViewModel LanguageMappingsViewModel { get; set; }
		public List<Authentication> Authentications { get; set; }
		public Authentication SelectedAuthentication
		{
			get => _selectedAuthentication;
			set
			{
				_selectedAuthentication = value;
				OnPropertyChanged(nameof(SelectedAuthentication));
				if (!string.IsNullOrEmpty(SelectedAuthentication.DisplayName))
				{
					CheckLoginMethod();
                    OnAuthenticationSelected(EventArgs.Empty);
					GetEngines();
				}
			}
		}

        // LoginMethod is used to display/hide the ClientId,ClientSecret fields based on which authentication mode is selected
        public string LoginMethod
		{
			get => _loginMethod;
			set
			{
				if (_loginMethod == value)
				{
					return;
				}
				_loginMethod = value;
				OnPropertyChanged(nameof(LoginMethod));
			}
		}

		public string Message
		{
			get => _message;
			set
			{
				_message = value;
				OnPropertyChanged(nameof(Message));
			}
		}

		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				_selectedIndex = value;
				OnPropertyChanged(nameof(SelectedIndex));
			}
		}

		public ICommand PasswordChangedCommand => _passwordChangedCommand ?? (_passwordChangedCommand = new RelayCommand(ChangePasswordAction));

		private void ChangePasswordAction(object parameter)
		{
			var passwordBox = (PasswordBox)parameter;
			switch (passwordBox.Name)
			{
				case "ClientIdBox":
					Options.ClientId = passwordBox.Password.TrimEnd().TrimStart();
					break;
				case "ClientSecretBox":
					Options.ClientSecret = passwordBox.Password.TrimEnd().TrimStart();
					break;
			}
			if (passwordBox.Password.Length > 0)
			{
				Message = string.Empty;
			}
			if (Options.Model == null && !string.IsNullOrEmpty(Options.ClientId) && !string.IsNullOrEmpty(Options.ClientSecret))
			{
				GetEngines();
			}
		}

		/// <summary>
		///  Set the LoginMethod based on user selection.
		///  If LoginMethod is Studio Authentication, check if user is logged-in in Studio
		/// </summary>
		private void CheckLoginMethod()
		{
			LoginMethod = SelectedAuthentication?.DisplayName;
			if (LoginMethod.Equals(Constants.StudioAuthentication))
			{
				Message = string.Empty;
				AppItializer.EnsureInitializer();
				Application.Current?.Dispatcher?.Invoke(() => { _studioCredentials.GetToken(); });
			}
		}

		private void SetAuthentications()
		{
			SelectedAuthentication = new Authentication();
			Authentications = new List<Authentication>
			{
				new Authentication
				{
					DisplayName = Constants.APICredentials,
					Type = Constants.APICredentialsType,
					Index = 0
				},
				new Authentication
				{   DisplayName = Constants.StudioAuthentication,
					Type = Constants.StudioAuthenticationType,
					Index = 1
				}
			};
			if (!string.IsNullOrEmpty(Options.AuthenticationMethod))
			{
				SelectedAuthentication = Authentications.FirstOrDefault(a => a.DisplayName.Equals(Options.AuthenticationMethod));
				SelectedIndex = SelectedAuthentication != null ? SelectedAuthentication.Index : 0;
			}
			else
			{
                // set by default APICredentials login method
                Options.AuthenticationMethod = Authentications[0].DisplayName;
				SelectedIndex = Authentications[0].Index;
                SelectedAuthentication = Authentications[0];
            }
        }

		private void GetEngines()
		{
			if (Options.AuthenticationMethod.Equals(Constants.APICredentials) && (_credentials == null || _credentials.Credential.Equals("#")))
			{
				RestoreEngines();
			}
			if (_credentials != null && !_credentials.Credential.Contains("#") && Options.AuthenticationMethod.Equals(Constants.APICredentials))
			{
				RestoreEngines();
			}

			var beGlobalTranslator = new BeGlobalV4Translator(Options, new MessageBoxService(), _credentials);
			var userInfo = beGlobalTranslator.GetUserInformation(false);
			if (userInfo.AccountId != 0)
			{
				if (Options?.Model == null || LanguageMappingsViewModel?.TranslationOptions.Count == 0)
				{
					var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(userInfo.AccountId.ToString());
					GetEngineModels(subscriptionInfo);
					SetEngineModel();
				}
			}
		}

		public void GetEngineModels(SubscriptionInfo subscriptionInfo)
		{
			var sourceLanguage = _normalizeSourceTextHelper.GetCorespondingLangCode(_languagePairs?[0].SourceCulture);
			var pairsWithSameSource = subscriptionInfo.LanguagePairs
				.Where(l => l.SourceLanguageId.Equals(sourceLanguage)).ToList();
			if (_languagePairs?.Count() > 0)
			{
				foreach (var languagePair in _languagePairs)
				{
					var targetLanguage = _normalizeSourceTextHelper.GetCorespondingLangCode(languagePair.TargetCulture);

					var serviceLanguagePairs = pairsWithSameSource.Where(t => t.TargetLanguageId.Equals(targetLanguage))
						.ToList();

					foreach (var serviceLanguagePair in serviceLanguagePairs)
					{
						var existingTranslationModel =
							LanguageMappingsViewModel.TranslationOptions.FirstOrDefault(e =>
								e.Model.Equals(serviceLanguagePair.Model));
						TranslationModel newTranslationModel = null;
						if (existingTranslationModel == null)
						{
							newTranslationModel = new TranslationModel
							{
								Model = serviceLanguagePair.Model,
								DisplayName = serviceLanguagePair.DisplayName,
							};
							LanguageMappingsViewModel.TranslationOptions.Add(newTranslationModel);
						}

						var currentTranslationModel = existingTranslationModel ?? newTranslationModel;
						if (!currentTranslationModel.LanguagesSupported.ContainsKey(languagePair.TargetCulture.Name))
						{
							currentTranslationModel.LanguagesSupported.Add(languagePair.TargetCulture.Name,
								serviceLanguagePair.Name);
						}
					}
				}
			}
		}

		public void SetEngineModel()
		{
			if (Options?.Model == null)
			{
				if (LanguageMappingsViewModel.TranslationOptions?.Count > 0)
				{
					LanguageMappingsViewModel.SelectedModelOption = LanguageMappingsViewModel?.TranslationOptions?[0];
					if (Options != null)
					{
						LanguageMappingsViewModel.SetOptions(LanguageMappingsViewModel?.TranslationOptions?[0]);
					}
				}
			}
			else
			{
				var mtModel = LanguageMappingsViewModel?.TranslationOptions?.FirstOrDefault(m => m.Model.Equals(Options.Model));
				if (mtModel != null)
				{
					var selectedModelIndex = LanguageMappingsViewModel.TranslationOptions.IndexOf(mtModel);
					LanguageMappingsViewModel.SelectedModelOption = LanguageMappingsViewModel.TranslationOptions[selectedModelIndex];
				}
			}
		}

		private void RestoreEngines()
		{
			if (string.IsNullOrEmpty(Options.ClientId) && string.IsNullOrEmpty(Options.ClientSecret))
			{
				Options.DisplayName = string.Empty;
				Options.Model = string.Empty;
				LanguageMappingsViewModel.TranslationOptions.Clear();
				LanguageMappingsViewModel.SelectedModelOption = new TranslationModel();
				return;
			}
		}
	}
}