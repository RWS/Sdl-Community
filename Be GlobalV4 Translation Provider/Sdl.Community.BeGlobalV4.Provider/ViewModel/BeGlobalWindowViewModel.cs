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
		public LanguageMappingsViewModel LanguageMappingsViewModel { get; set; }
		private ICommand _okCommand;
		private int _selectedTabIndex;
		private readonly BeGlobalWindow _mainWindow;
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;
		private readonly LanguagePair[] _languagePairs;
		public static readonly Log Log = Log.Instance;

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options,
			TranslationProviderCredential credentialStore, LanguagePair[] languagePairs)
		{
			LoginViewModel = new LoginViewModel(options);
			LanguageMappingsViewModel = new LanguageMappingsViewModel(options);
			Options = options;
			_mainWindow = mainWindow;
			_languagePairs = languagePairs;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();

			if (credentialStore == null) return;
			if (options.UseClientAuthentication)
			{
				_mainWindow.LoginTab.ClientIdBox.Password = options.ClientId;
				_mainWindow.LoginTab.ClientSecretBox.Password = options.ClientSecret;
			}
			else
			{
				LoginViewModel.Email = options.ClientId;
				_mainWindow.LoginTab.PasswordBox.Password = options.ClientSecret;
			}
		}

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));

		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set
			{
				_selectedTabIndex = value;
				IsWindowValid();
				OnPropertyChanged();
			}
		}

		private void GetEngineModels(List<BeGlobalLanguagePair> beGlobalLanguagePairs)
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
						if (LanguageMappingsViewModel?.TranslationOptions != null)
						{
							var engineExists = LanguageMappingsViewModel.TranslationOptions.Any(e => e.Model.Equals(serviceLanguagePair.Model));
							if (!engineExists)
							{
								LanguageMappingsViewModel.TranslationOptions.Add(new TranslationModel
								{
									Model = serviceLanguagePair.Model,
									DisplayName = serviceLanguagePair.DisplayName
								});
							}
						}
					}
				}
			}
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

		public void SetEngineModel()
		{
			var beGlobalTranslator = new BeGlobalV4Translator("https://translate-api.sdlbeglobal.com", Options);
			var accountId = Options.UseClientAuthentication ? beGlobalTranslator.GetClientInformation() : beGlobalTranslator.GetUserInformation();
			var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
			Options.SubscriptionInfo = subscriptionInfo;

			GetEngineModels(subscriptionInfo.LanguagePairs);
			if (Options?.Model == null)
			{
				if (LanguageMappingsViewModel.TranslationOptions?.Count > 0)
				{
					LanguageMappingsViewModel.SelectedModelOption = LanguageMappingsViewModel.TranslationOptions?[0];
					if (Options != null)
					{
						Options.Model = LanguageMappingsViewModel.TranslationOptions?[0].Model;
					}
				}
			}
			else
			{
				var mtModel = LanguageMappingsViewModel.TranslationOptions.FirstOrDefault(m => m.Model.Equals(Options.Model));
				if (mtModel != null)
				{
					var selectedModelIndex = LanguageMappingsViewModel.TranslationOptions.IndexOf(mtModel);
					LanguageMappingsViewModel.SelectedModelOption = LanguageMappingsViewModel.TranslationOptions[selectedModelIndex];
				}
			}
		}

		private bool IsWindowValid()
		{
			var loginTab = _mainWindow?.LoginTab;
			Options.ResendDrafts = LanguageMappingsViewModel.ReSendChecked;
			Options.Model = LanguageMappingsViewModel.SelectedModelOption?.Model;
			try
			{
				if (LoginViewModel.SelectedOption.Type.Equals(Constants.User))
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
							SetEngineModel();
						}
						return true;
					}
				}
				else
				{
					var clientId = loginTab?.ClientIdBox.Password;
					var clientSecret = loginTab?.ClientSecretBox.Password;
					if (!string.IsNullOrEmpty(clientId?.TrimEnd().TrimStart()) && !string.IsNullOrEmpty(clientSecret.TrimEnd().TrimStart()))
					{
						Options.ClientId = clientId;
						Options.ClientSecret = clientSecret;
						Options.UseClientAuthentication = true;
						if (Options.Model == null)
						{
							SetEngineModel();
						}
						LoginViewModel.Message = string.Empty;
						return true;
					}
				}
				if (loginTab != null)
				{
					LoginViewModel.Message = Constants.CredentialsValidation;
				}
			}
			catch (Exception e)
			{
				if (loginTab != null)
				{
					LoginViewModel.Message = e.Message.Contains(Constants.TokenFailed) ? Constants.CredentialsNotValid : e.Message;
				}
				Log.Logger.Error($"{Constants.IsWindowValid} {e.Message}\n {e.StackTrace}");
			}
			return false;
		}
	}
}