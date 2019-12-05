using System;
using System.Collections.Generic;
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
		public BeGlobalTranslationOptions Options { get; set; }
		public LoginViewModel LoginViewModel { get; set; }
		public SettingsViewModel SettingsViewModel { get; set; }
		private ICommand _okCommand;
		private int _selectedIndex;
		private readonly BeGlobalWindow _mainWindow;
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;
		private readonly LanguagePair[] _languagePairs;
		public static readonly Log Log = Log.Instance;

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options,
			TranslationProviderCredential credentialStore, LanguagePair[] languagePairs)
		{
			LoginViewModel = new LoginViewModel(options);
			SettingsViewModel = new SettingsViewModel(options);
			Options = options;
			_mainWindow = mainWindow;
			_languagePairs = languagePairs;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();

			if (credentialStore == null) return;
			if (options.UseClientAuthentication)
			{
				_mainWindow.LoginTab.ClientKeyBox.Password = options.ClientId;
				_mainWindow.LoginTab.ClientSecretBox.Password = options.ClientSecret;
			}
			else
			{
				LoginViewModel.Email = options.ClientId;
				_mainWindow.LoginTab.PasswordBox.Password = options.ClientSecret;
			}
		}

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));

		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				_selectedIndex = value;

				if (value.Equals(1) && IsWindowValid())
				{
					SettingsViewModel.MessageVisibility = "Collapsed";
					SetEngineModel();
				}
				OnPropertyChanged();
			}
		}

		private bool GetEngineModels(List<BeGlobalLanguagePair> beGlobalLanguagePairs)
		{
			if (beGlobalLanguagePairs != null)
			{
				var sourceLanguage = _normalizeSourceTextHelper.GetCorrespondingLangCode(_languagePairs?[0].SourceCulture);
				var pairsWithSameSource = beGlobalLanguagePairs.Where(l => l.SourceLanguageId.Equals(sourceLanguage)).ToList();
				if (_languagePairs?.Count() > 0)
				{
					foreach (var languagePair in _languagePairs)
					{
						var targetLanguage =
							_normalizeSourceTextHelper.GetCorrespondingLangCode(languagePair.TargetCulture);

						var serviceLanguagePairs = pairsWithSameSource.Where(t => t.TargetLanguageId.Equals(targetLanguage)).ToList();

						foreach (var serviceLanguagePair in serviceLanguagePairs)
						{
							if (SettingsViewModel?.TranslationOptions != null)
							{
								var engineExists = SettingsViewModel.TranslationOptions.Any(e => e.Model.Equals(serviceLanguagePair.Model));
								if (!engineExists)
								{
									SettingsViewModel.TranslationOptions.Add(new TranslationModel
									{
										Model = serviceLanguagePair.Model,
										DisplayName = serviceLanguagePair.DisplayName
									});
								}
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		private void Ok(object parameter)
		{
			var loginTab = parameter as Login;
			if (loginTab != null)
			{
				var isValid = IsWindowValid();
				if (isValid)
				{
					WindowCloser.SetDialogResult(_mainWindow, true);
					_mainWindow.Close();
				}
			}
		}

		public bool SetEngineModel()
		{
			var beGlobalTranslator = new BeGlobalV4Translator("https://translate-api.sdlbeglobal.com", Options.ClientId, Options.ClientSecret, Options.Model, Options.UseClientAuthentication);
			var accountId = Options.UseClientAuthentication ? beGlobalTranslator.GetClientInformation() : beGlobalTranslator.GetUserInformation();
			var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
			Options.SubscriptionInfo = subscriptionInfo;

			var areEngiesRetrieved = GetEngineModels(subscriptionInfo?.LanguagePairs);
			if (Options?.Model == null)
			{
				if (SettingsViewModel?.TranslationOptions?.Count > 0)
				{
					SettingsViewModel.SelectedModelOption = SettingsViewModel?.TranslationOptions?[0];
					if (Options != null)
					{
						Options.Model = SettingsViewModel?.TranslationOptions?[0].Model;
					}
				}
			}
			else
			{
				var mtModel = SettingsViewModel?.TranslationOptions?.FirstOrDefault(m => m.Model.Equals(Options.Model));
				if (mtModel != null)
				{
					var selectedModelIndex = SettingsViewModel.TranslationOptions.IndexOf(mtModel);
					SettingsViewModel.SelectedModelOption = SettingsViewModel.TranslationOptions[selectedModelIndex];
				}
			}
			return areEngiesRetrieved;
		}

		private bool IsWindowValid()
		{
			var loginTab = _mainWindow?.LoginTab;
			Options.ResendDrafts = SettingsViewModel.ReSendChecked;
			Options.Model = SettingsViewModel.SelectedModelOption?.Model;
			try
			{
				if (LoginViewModel.SelectedOption.Type.Equals("User"))
				{
					var password = loginTab?.PasswordBox.Password;
					if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(LoginViewModel.Email))
					{
						Options.ClientId = LoginViewModel.Email.TrimEnd().TrimStart();
						Options.ClientSecret = password.TrimEnd().TrimStart();
						Options.UseClientAuthentication = false;
						LoginViewModel.Message = string.Empty;
						if (Options.Model == null)
						{
							return ValidateEnginesSetup();
						}
						return true;
					}
				}
				else
				{
					var clientId = loginTab?.ClientKeyBox.Password;
					var clientSecret = loginTab?.ClientSecretBox.Password;
					if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
					{
						Options.ClientId = clientId.TrimEnd().TrimStart();
						Options.ClientSecret = clientSecret.TrimEnd().TrimStart();
						Options.UseClientAuthentication = true;
						LoginViewModel.Message = string.Empty;
						if (Options.Model == null)
						{
							return ValidateEnginesSetup();
						}
						return true;
					}
				}
				if (loginTab != null)
				{
					LoginViewModel.Message = "Please fill the credentials fields!";
				}
			}
			catch (Exception e)
			{
				SettingsViewModel.MessageVisibility = "Visible";
				if (loginTab != null)
				{
					LoginViewModel.Message = (e.Message.Contains("Acquiring token failed") || e.Message.Contains("Value cannot be null."))
						? "Please verify your credentials!"
						: e.Message;
				}
				Log.Logger.Error($"Is window valid method: {e.Message}\n {e.StackTrace}");
			}
			return false;
		}

		private bool ValidateEnginesSetup()
		{
			var isEngineSet = SetEngineModel();
			if (!isEngineSet)
			{
				LoginViewModel.Message = "The MTCloud host was unable to be reached and setups cannot be saved. Please verify your credentials and internet connection. Please ensure you are able to connect to the server from this computer.";
				return false;
			}
			return true;
		}
	}
}