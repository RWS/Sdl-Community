using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BeGlobalWindowViewModel : BaseViewModel
	{
		private readonly LanguagePair[] _languagePairs;
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;
		private bool _reSendChecked;
		private TranslationModel _selectedModel;
		private MessageBoxService _messageBoxService;
		private TranslationProviderCredential _credentials;
		private int _selectedTabIndex;

		private ICommand _okCommand;

		public BeGlobalWindowViewModel(BeGlobalTranslationOptions options, LanguagePair[] languagePairs, TranslationProviderCredential credentials)
		{
			SelectedTabIndex = 0;
			Options = options;
			LoginViewModel = new LoginViewModel(options);
			LanguageMappingsViewModel = new LanguageMappingsViewModel(options);
			_languagePairs = languagePairs;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
			_credentials = credentials;

			TranslationOptions = new ObservableCollection<TranslationModel>();
			_messageBoxService = new MessageBoxService();
			if (Options != null)
			{
				ReSendChecked = options.ResendDrafts;
			}

			if (credentials == null) return;
			var credential = _credentials.Credential.Replace("sdlmachinetranslationcloudprovider:///", string.Empty);
			if (credential.Contains("#"))
			{
				var splitedCredentials = credentials.Credential.Split('#');
				if (splitedCredentials.Length == 2 && !string.IsNullOrEmpty(splitedCredentials[0]) && !string.IsNullOrEmpty(splitedCredentials[1]))
				{
					var beGlobalTranslator = new BeGlobalV4Translator(Options, _messageBoxService, _credentials);
					var accountId = beGlobalTranslator.GetUserInformation();
					var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
					GetEngineModels(subscriptionInfo);
					SetEngineModel();
					SetAuthenticationOptions();
				}
			}
		}

		public LoginViewModel LoginViewModel { get; set; }
		public LanguageMappingsViewModel LanguageMappingsViewModel { get; set; }
		public BeGlobalTranslationOptions Options { get; set; }

		public bool ReSendChecked
		{
			get => _reSendChecked;
			set
			{
				_reSendChecked = value;
				if (Options?.Model != null)
				{
					Options.ResendDrafts = value;
				}
				OnPropertyChanged(nameof(ReSendChecked));
			}
		}

		public TranslationModel SelectedModelOption
		{
			get => _selectedModel;
			set
			{
				_selectedModel = value;
				if (Options?.Model != null)
				{
					SetOptions(value);
				}
				OnPropertyChanged(nameof(SelectedModelOption));
			}
		}

		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set
			{
				_selectedTabIndex = value;

				if (value.Equals(1) && IsWindowValid())
				{
					LanguageMappingsViewModel.MessageVisibility = "Collapsed";
				}
				OnPropertyChanged();
			}
		}

		public ObservableCollection<TranslationModel> TranslationOptions { get; set; }

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));
			
		private void GetEngineModels(SubscriptionInfo subscriptionInfo)
		{
			var sourceLanguage = _normalizeSourceTextHelper.GetCorespondingLangCode(_languagePairs?[0].SourceCulture);
			var pairsWithSameSource = subscriptionInfo.LanguagePairs.Where(l => l.SourceLanguageId.Equals(sourceLanguage)).ToList();
			if (_languagePairs?.Count() > 0)
			{
				foreach (var languagePair in _languagePairs)
				{
					var targetLanguage = _normalizeSourceTextHelper.GetCorespondingLangCode(languagePair.TargetCulture);

					var serviceLanguagePairs = pairsWithSameSource.Where(t => t.TargetLanguageId.Equals(targetLanguage)).ToList();

					foreach (var serviceLanguagePair in serviceLanguagePairs)
					{
						var existingTranslationModel = TranslationOptions.FirstOrDefault(e => e.Model.Equals(serviceLanguagePair.Model));
						TranslationModel newTranslationModel = null;
						if (existingTranslationModel == null)
						{
							newTranslationModel = new TranslationModel
							{
								Model = serviceLanguagePair.Model,
								DisplayName = serviceLanguagePair.DisplayName,
							};
							TranslationOptions.Add(newTranslationModel);
						}

						(existingTranslationModel ?? newTranslationModel).LanguagesSupported.Add(languagePair.TargetCulture.Name, serviceLanguagePair.Name);
					}
				}
			}
		}

		private void Ok(object parameter)
		{
			var currentWindow = WindowsControlUtils.GetCurrentWindow() as BeGlobalWindow;
			var loginTab = parameter as Login;
			if (loginTab != null)
			{
				var isValid = IsWindowValid();
				if (isValid)
				{
					WindowCloser.SetDialogResult(currentWindow, true);
					currentWindow?.Close();
				}
			}				
		}

		private void SetEngineModel()
		{
			if (Options?.Model == null)
			{
				if (TranslationOptions?.Count > 0)
				{
					SelectedModelOption = TranslationOptions?[0];
					if (Options != null)
					{
						SetOptions(TranslationOptions?[0]);
					}
				}
			}
			else
			{
				var mtModel = TranslationOptions?.FirstOrDefault(m => m.Model.Equals(Options.Model));
				if (mtModel != null)
				{
					var selectedModelIndex = TranslationOptions.IndexOf(mtModel);
					SelectedModelOption = TranslationOptions[selectedModelIndex];
				}
			}
		}

		private void SetAuthenticationOptions()
		{
			var currentWindow = WindowsControlUtils.GetCurrentWindow() as BeGlobalWindow;
			if (string.IsNullOrEmpty(Options.AuthenticationMethod)) return;
			if (Options.AuthenticationMethod.Equals(Constants.APICredentials))
			{
				if (!string.IsNullOrEmpty(Options.ClientId) && !string.IsNullOrEmpty(Options.ClientSecret))
				{
					currentWindow.LoginTab.ClientIdBox.Password = Options.ClientId;
					currentWindow.LoginTab.ClientSecretBox.Password = Options.ClientSecret;
				}
				else
				{
					var splitedCredentials = _credentials?.Credential.Split('#');
					currentWindow.LoginTab.ClientIdBox.Password = splitedCredentials.Length > 0 ? splitedCredentials[0] : string.Empty;
					currentWindow.LoginTab.ClientSecretBox.Password = splitedCredentials.Length > 0 ? splitedCredentials[1] : string.Empty;
				}
			}
			else
			{
				LoginViewModel.SelectedAuthentication = LoginViewModel.Authentications[1];
				LoginViewModel.SelectedIndex = LoginViewModel.SelectedAuthentication.Index;
			}
		}

		private void SetOptions(TranslationModel translationModel)
		{
			Options.Model = translationModel.Model;
			Options.DisplayName = translationModel.DisplayName;
			Options.LanguagesSupported = translationModel.LanguagesSupported;
		}

		private bool IsWindowValid()
		{
			try
			{
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
				if (Options?.Model == null)
				{
					var beGlobalTranslator = new BeGlobalV4Translator(Options, _messageBoxService, _credentials);
					var accountId = beGlobalTranslator.GetUserInformation();
					var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
					GetEngineModels(subscriptionInfo);
					SetEngineModel();
				}
				return true;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.IsWindowValid} {ex.Message}\n {ex.StackTrace}");
			}
			return false;
		}	
	}
}