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

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BeGlobalWindowViewModel : BaseViewModel
	{
		private readonly LanguagePair[] _languagePairs;
		private readonly BeGlobalWindow _mainWindow;
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;
		private ICommand _okCommand;
		private bool _reSendChecked;
		private TranslationModel _selectedModel;
		private BeGlobalLoginOptions _selectedLoginOption;
		private string _loginMethod;
		private int _selectedIndex;

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options, LanguagePair[] languagePairs, TranslationProviderCredential credentials)
		{
			Options = options;
			_mainWindow = mainWindow;
			_languagePairs = languagePairs;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
			_loginMethod = Enums.LoginOptions.APICredentials.ToString();
			TranslationOptions = new ObservableCollection<TranslationModel>();
			SelectedIndex = (int)Enums.LoginOptions.APICredentials;

			var beGlobalTranslator = new BeGlobalV4Translator(Options);
			var accountId = beGlobalTranslator.GetUserInformation();

			var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
			if (Options != null)
			{
				ReSendChecked = options.ResendDrafts;
			}		
			SetLoginOptions();
			GetEngineModels(subscriptionInfo);
			SetEngineModel();
			SetAuthenticationOptions(credentials);
		}

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));
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

		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				_selectedIndex = value;				
				OnPropertyChanged(nameof(SelectedIndex));
			}
		}

		public ObservableCollection<BeGlobalLoginOptions> LoginOptions { get; set; }
	    public BeGlobalLoginOptions SelectedLoginOption
		{
			get => _selectedLoginOption;
			set
			{
				_selectedLoginOption = value;
				OnPropertyChanged(nameof(SelectedLoginOption));

				LoginMethod = _selectedLoginOption?.LoginOption;
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
			WindowCloser.SetDialogResult(_mainWindow, true);
			_mainWindow.Close();
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
			//set by default the APICredentials method in case it was not setup yet
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
				_mainWindow.ClientIdBox.Password = Options.ClientId;
				_mainWindow.ClientSecretBox.Password = Options.ClientSecret;
			}
			Options.AuthenticationMethod = SelectedLoginOption?.LoginOption;
		}

		private void SetAuthenticationOptions(TranslationProviderCredential credentials)
		{
			if (string.IsNullOrEmpty(Options.AuthenticationMethod)) return;
			if (Options.AuthenticationMethod.Equals(Enums.GetDisplayName(Enums.LoginOptions.APICredentials)))
			{
				if (!string.IsNullOrEmpty(Options.ClientId) && !string.IsNullOrEmpty(Options.ClientSecret))
				{
					_mainWindow.ClientIdBox.Password = Options.ClientId;
					_mainWindow.ClientSecretBox.Password = Options.ClientSecret;
				}
				else
				{
					var splitedCredentials = credentials?.Credential.Split('#');
					_mainWindow.ClientIdBox.Password = splitedCredentials.Length > 0 ? splitedCredentials[0] : string.Empty;
					_mainWindow.ClientSecretBox.Password = splitedCredentials.Length > 0 ? splitedCredentials[1] : string.Empty;
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
	}
}