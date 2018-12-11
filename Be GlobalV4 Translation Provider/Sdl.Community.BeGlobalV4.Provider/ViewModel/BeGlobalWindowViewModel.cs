using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BeGlobalWindowViewModel : BaseViewModel
	{
		public BeGlobalTranslationOptions Options { get; set; }
		public LoginViewModel LoginViewModel { get; set; }
		public SettingsViewModel SettingsViewModel { get; set; }	
		private ICommand _okCommand;
		private int _selectedIndex;
		private readonly bool _tellMeAction;   
		private readonly BeGlobalWindow _mainWindow;
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options,
			TranslationProviderCredential credentialStore)
		{
			LoginViewModel = new LoginViewModel(options);
			SettingsViewModel = new SettingsViewModel(options);
			Options = options;
			_mainWindow = mainWindow;
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

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options, bool tellMeAction)
		{
			LoginViewModel = new LoginViewModel(options);
			SettingsViewModel = new SettingsViewModel(options);
			Options = options;
			if (options != null)
			{
				var mtModel = SettingsViewModel.TranslationOptions.FirstOrDefault(m => m.Model.Equals(options.Model));
				if (mtModel != null)
				{
					var selectedModelIndex = SettingsViewModel.TranslationOptions.IndexOf(mtModel);
					SettingsViewModel.SelectedModelOption = SettingsViewModel.TranslationOptions[selectedModelIndex];
				}
				else if(SettingsViewModel.TranslationOptions.Count.Equals(0))
				{
					var translationModel = new TranslationModel
					{
						Model = options.Model,
						DisplayName = options.Model
					};
					SettingsViewModel.TranslationOptions.Add(translationModel);
					SettingsViewModel.SelectedModelOption = translationModel;
				}
			}
			_mainWindow = mainWindow;
			_tellMeAction = tellMeAction;
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
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

		private void GetEngineModels(List<BeGlobalLanguagePair>languagePairs)
		{
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetProjectInfo();
			var sourceLanguage = _normalizeSourceTextHelper.GetCorespondingLangCode(currentProject.SourceLanguage.CultureInfo.ThreeLetterISOLanguageName);
			var pairsWithSameSource = languagePairs.Where(l => l.SourceLanguageId.Equals(sourceLanguage)).ToList();
			foreach (var projectTargetLanguage in currentProject.TargetLanguages)
			{
				var targetLanguage =
					_normalizeSourceTextHelper.GetCorespondingLangCode(projectTargetLanguage.CultureInfo.ThreeLetterISOLanguageName);

				var serviceLanguagePairs = pairsWithSameSource.Where(t => t.TargetLanguageId.Equals(targetLanguage)).ToList();

				foreach (var languagePair in serviceLanguagePairs)
				{ 
					if (SettingsViewModel?.TranslationOptions != null)
					{
						var engineExists = SettingsViewModel.TranslationOptions.Any(e => e.Model.Equals(languagePair.Model));
						if (!engineExists)
						{
							SettingsViewModel.TranslationOptions.Add(new TranslationModel
							{
								Model = languagePair.Model,
								DisplayName = languagePair.DisplayName
							});
						}
					}
				}
			} 
		}

		private void Ok(object parameter)
		{
			var loginTab = parameter as Login;

			if (_tellMeAction)
			{
				WindowCloser.SetDialogResult(_mainWindow, true);
				_mainWindow.Close();
			}
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

		private void SetEngineModel()
		{
			var beGlobalTranslator = new BeGlobalV4Translator("https://translate-api.sdlbeglobal.com", Options.ClientId, Options.ClientSecret, string.Empty, string.Empty, Options.Model, Options.UseClientAuthentication);
			var accountId = beGlobalTranslator.GetUserInformation();
			var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());

			GetEngineModels(subscriptionInfo.LanguagePairs);
			if (Options?.Model == null)
			{
				if (SettingsViewModel.TranslationOptions?.Count > 0)
				{
					SettingsViewModel.SelectedModelOption = SettingsViewModel.TranslationOptions?[0];
					if (Options != null)
					{
						Options.Model = SettingsViewModel.TranslationOptions?[0].Model;
					}
				}	
			}
			else
			{
				var mtModel = SettingsViewModel.TranslationOptions.FirstOrDefault(m => m.Model.Equals(Options.Model));
				if (mtModel != null)
				{
					var selectedModelIndex = SettingsViewModel.TranslationOptions.IndexOf(mtModel);
					SettingsViewModel.SelectedModelOption = SettingsViewModel.TranslationOptions[selectedModelIndex];
				}
			}	   
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
						Options.ClientId = LoginViewModel.Email;
						Options.ClientSecret = password;
						loginTab.ValidationBlock.Visibility = Visibility.Collapsed;
						if (Options.Model == null)
						{
							SetEngineModel();
						}
						Options.UseClientAuthentication = false;
						return true;
					}
				}
				else
				{
					var clientId = loginTab?.ClientKeyBox.Password;
					var clientSecret = loginTab?.ClientSecretBox.Password;
					if (!string.IsNullOrEmpty(clientId?.TrimEnd()) && !string.IsNullOrEmpty(clientSecret.TrimEnd()))
					{
						Options.ClientId = clientId;
						Options.ClientSecret = clientSecret;
						Options.UseClientAuthentication = true;
						if (Options.Model == null)
						{
							SetEngineModel();
						}
						loginTab.ValidationBlock.Visibility = Visibility.Collapsed;
						return true;
					}
				}
				if (loginTab != null)
				{
					loginTab.ValidationBlock.Visibility = Visibility.Visible;
				}
			}
			catch (Exception e)
			{
				SettingsViewModel.MessageVisibility = "Visible";
				if (loginTab != null)
				{
					loginTab.ValidationBlock.Visibility = Visibility.Visible;
					loginTab.ValidationBlock.Text = e.Message.Contains("Acquiring token failed") ? "Please verify your credentials." : e.Message;
				}
			} 
			return false;
		}
	}
}
