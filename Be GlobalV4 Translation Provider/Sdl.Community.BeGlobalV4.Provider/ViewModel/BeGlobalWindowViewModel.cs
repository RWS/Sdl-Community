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
using static Sdl.Community.BeGlobalV4.Provider.Helpers.Enums;
using Application = System.Windows.Application;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BeGlobalWindowViewModel : BaseViewModel
	{
		private readonly LanguagePair[] _languagePairs;
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;
		private bool _reSendChecked;
		private TranslationModel _selectedModel;
		private BeGlobalLoginOptions _selectedLoginOption;
		private string _loginMethod;
		private int _selectedIndex;
		private MessageBoxService _messageBoxService;
		private TranslationProviderCredential _credentials;
		private readonly StudioCredentials _studioCredentials = new StudioCredentials();

		private ICommand _okCommand;
		private ICommand _loginCommand;

		public BeGlobalWindowViewModel(BeGlobalTranslationOptions options, LanguagePair[] languagePairs, TranslationProviderCredential credentials)
		{
			Options = options;
			BeGlobalWindow = new BeGlobalWindow();
			_languagePairs = languagePairs;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
			_loginMethod = Enums.LoginOptions.APICredentials.ToString();
			_credentials = credentials;
			TranslationOptions = new ObservableCollection<TranslationModel>();
			SelectedIndex = (int)Enums.LoginOptions.APICredentials;
			_messageBoxService = new MessageBoxService();
			if (Options != null)
			{
				ReSendChecked = options.ResendDrafts;
			}
			SetLoginOptions();
			if (credentials == null) return;
			var credential = _credentials.Credential.Replace("machinetranslationcloudprovider:///", string.Empty);
			if (credential.Contains("#"))
			{
				var beGlobalTranslator = new BeGlobalV4Translator(Options, _messageBoxService, _credentials);
				var accountId = beGlobalTranslator.GetUserInformation();
				var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
				GetEngineModels(subscriptionInfo);
				SetEngineModel();
				SetAuthenticationOptions();
			}
		}
		
		public BeGlobalTranslationOptions Options { get; set; }
		public BeGlobalWindow BeGlobalWindow { get; set; }
		public ObservableCollection<BeGlobalLoginOptions> LoginOptions { get; set; }

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

		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				_selectedIndex = value;				
				OnPropertyChanged(nameof(SelectedIndex));
			}
		}

	    public BeGlobalLoginOptions SelectedLoginOption
		{
			get => _selectedLoginOption;
			set
			{
				_selectedLoginOption = value;
				OnPropertyChanged(nameof(SelectedLoginOption));

				CheckLoginMethod();
				SetClientOptions();
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

		public ObservableCollection<TranslationModel> TranslationOptions { get; set; }

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));
		public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new RelayCommand(LoginAction));

		private void LoginAction(object parameter)
		{

		}

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
			WindowCloser.SetDialogResult(BeGlobalWindow, true);
			BeGlobalWindow.Close();
			if (Options?.Model == null)
			{
				Options.ClientId = BeGlobalWindow.ClientIdBox.Password;
				Options.ClientSecret = BeGlobalWindow.ClientSecretBox.Password;
				var beGlobalTranslator = new BeGlobalV4Translator(Options, _messageBoxService, _credentials);
				var accountId = beGlobalTranslator.GetUserInformation();
				var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
				GetEngineModels(subscriptionInfo);
				SetEngineModel();
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

		private void SetLoginOptions()
		{
			LoginOptions = new ObservableCollection<BeGlobalLoginOptions>();
			foreach (LoginOptions enumVal in Enum.GetValues(typeof(LoginOptions)))
			{
				var displayName = Enums.GetDisplayName(enumVal);
				displayName = !string.IsNullOrEmpty(displayName) ? displayName : enumVal.ToString();

				var beGlobalLoginOption = new BeGlobalLoginOptions { LoginOption = displayName };
				LoginOptions.Add(beGlobalLoginOption);
			}
			//set by default the APICredentials method in case it wasn't setup yet
			if (string.IsNullOrEmpty(Options.AuthenticationMethod)
				|| Options.AuthenticationMethod.Equals(Enums.GetDisplayName(Enums.LoginOptions.APICredentials)))
			{
				SelectedLoginOption = new BeGlobalLoginOptions { LoginOption = Enums.GetDisplayName(Enums.LoginOptions.APICredentials) };
			}
		}

		private void SetClientOptions()
		{
			if(!string.IsNullOrEmpty(Options.ClientId) && !string.IsNullOrEmpty(Options.ClientSecret))
			{
				BeGlobalWindow.ClientIdBox.Password = Options.ClientId;
				BeGlobalWindow.ClientSecretBox.Password = Options.ClientSecret;
			}
			Options.AuthenticationMethod = SelectedLoginOption?.LoginOption;
		}

		private void SetAuthenticationOptions()
		{
			if (string.IsNullOrEmpty(Options.AuthenticationMethod)) return;
			if (Options.AuthenticationMethod.Equals(Enums.GetDisplayName(Enums.LoginOptions.APICredentials)))
			{
				if (!string.IsNullOrEmpty(Options.ClientId) && !string.IsNullOrEmpty(Options.ClientSecret))
				{
					BeGlobalWindow.ClientIdBox.Password = Options.ClientId;
					BeGlobalWindow.ClientSecretBox.Password = Options.ClientSecret;
				}
				else
				{
					var splitedCredentials = _credentials?.Credential.Split('#');
					BeGlobalWindow.ClientIdBox.Password = splitedCredentials.Length > 0 ? splitedCredentials[0] : string.Empty;
					BeGlobalWindow.ClientSecretBox.Password = splitedCredentials.Length > 0 ? splitedCredentials[1] : string.Empty;
				}
			}
			else
			{
				SelectedIndex = (int)Enums.LoginOptions.StudioAuthentication;
				SelectedLoginOption = new BeGlobalLoginOptions { LoginOption = Enums.GetDisplayName(Enums.LoginOptions.StudioAuthentication) };
			}
		}

		private void SetOptions(TranslationModel translationModel)
		{
			Options.Model = translationModel.Model;
			Options.DisplayName = translationModel.DisplayName;
			Options.LanguagesSupported = translationModel.LanguagesSupported;
		}

		/// <summary>
		///  Set the LoginMethod based on user selection.
		///  If LoginMethod is Studio Authentication, check if user is logged-in in Studio
		/// </summary>
		private void CheckLoginMethod()
		{
			LoginMethod = _selectedLoginOption?.LoginOption;
			if (LoginMethod.Equals(Enums.GetDisplayName(Enums.LoginOptions.StudioAuthentication)))
			{
				AppItializer.EnsureInitializer();
				Application.Current?.Dispatcher?.Invoke(() =>
				{
					_studioCredentials.GetToken();
				});
			}
		}
	}
}