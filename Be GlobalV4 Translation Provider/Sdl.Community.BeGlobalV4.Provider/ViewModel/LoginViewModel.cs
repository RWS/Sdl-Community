using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Model;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class LoginViewModel : BaseViewModel
	{
		private Authentication _selectedOption;
		private string _email;
		private string _loginMethod;
		private readonly NormalizeSourceTextHelper _normalizeSourceTextHelper;
		private readonly LanguagePair[] _languagePairs;
		private Constants _constants = new Constants();

		private ICommand _passwordChangedCommand;
		private ICommand _navigateCommand;

		public LoginViewModel(
			BeGlobalTranslationOptions options,
			LanguagePair[] languagePairs,
			LanguageMappingsViewModel languageMappingsViewModel,
			BeGlobalWindowViewModel beGlobalWindowViewModel)
		{
			_normalizeSourceTextHelper = new NormalizeSourceTextHelper();
			_languagePairs = languagePairs;
			LanguageMappingsViewModel = languageMappingsViewModel;
			BeGlobalWindowViewModel = beGlobalWindowViewModel;
			Options = options;

			AuthenticationOptions = new List<Authentication>
			{
				new Authentication
				{
					DisplayName = _constants.ClientAuthentication,
					Type = _constants.Client
				},
				new Authentication
				{   DisplayName = _constants.UserAuthentication,
					Type = _constants.User
				}
			};
			SelectedOption = options.UseClientAuthentication ? AuthenticationOptions[0] : AuthenticationOptions[1];
			LoginMethod = SelectedOption.Type;
		}

		public BeGlobalWindowViewModel BeGlobalWindowViewModel { get; set; }
		public BeGlobalTranslationOptions Options { get; set; }
		public LanguageMappingsViewModel LanguageMappingsViewModel { get; set; }

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

		public List<Authentication> AuthenticationOptions { get; set; }

		public Authentication SelectedOption
		{
			get => _selectedOption;
			set
			{
				_selectedOption = value;
				if (_selectedOption != null)
				{
					LoginMethod = _selectedOption.Type.Equals(_constants.User) ? _constants.User : _constants.Client; 					
				}
				OnPropertyChanged();
			}
		}
		
		public string Email
		{
			get => _email;
			set
			{
				_email = value;
				OnPropertyChanged();
			}
		}
			
		public ICommand NavigateCommand => _navigateCommand ?? (_navigateCommand = new RelayCommand(Navigate));
		public ICommand PasswordChangedCommand => _passwordChangedCommand ?? (_passwordChangedCommand = new RelayCommand(ChangePasswordAction));

		public bool ValidateEnginesSetup()
		{
			var isEngineSet = SetEngineModel();
			if (!isEngineSet)
			{
				BeGlobalWindowViewModel.Message = _constants.CredentialsAndInternetValidation;
				return false;
			}
			return true;
		}

		private bool SetEngineModel()
		{
			var beGlobalTranslator = new BeGlobalV4Translator("https://translate-api.sdlbeglobal.com", Options);
			var accountId = Options.UseClientAuthentication ? beGlobalTranslator.GetClientInformation() : beGlobalTranslator.GetUserInformation();
			var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
			Options.SubscriptionInfo = subscriptionInfo;

			var areEngiesRetrieved = GetEngineModels(subscriptionInfo?.LanguagePairs);
			if (string.IsNullOrEmpty(Options?.Model))
			{
				if (LanguageMappingsViewModel?.TranslationOptions?.Count > 0)
				{
					LanguageMappingsViewModel.SelectedModelOption = LanguageMappingsViewModel?.TranslationOptions?[0];
					if (string.IsNullOrEmpty(Options?.Model))
					{
						SetOptions(LanguageMappingsViewModel?.TranslationOptions[0]);
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
			return areEngiesRetrieved;
		}

		private bool GetEngineModels(List<BeGlobalLanguagePair> beGlobalLanguagePairs)
		{
			if (beGlobalLanguagePairs != null)
			{
				foreach (var languageMapping in LanguageMappingsViewModel?.LanguageMappings)
				{
					//get beGlobalLanguagePairs for the specific source language MTSourceCodes
					var sourcePairs = beGlobalLanguagePairs.Where(b => languageMapping.MTCodeSource.Any(l => b.SourceLanguageId.Equals(l)));

					//get beGlobalLanguagePairs for the specific target MTTargetCodes and exiting sourcePairs
					var serviceLanguagePairs = sourcePairs.Where(s => languageMapping.MTCodeTarget.Any(l => s.TargetLanguageId.Equals(l)));
					var splittedLangPair = Utils.SplitLanguagePair(languageMapping.ProjectLanguagePair);
					var sourceCultureName = _languagePairs?.FirstOrDefault(n => n.SourceCulture.EnglishName.Equals(splittedLangPair[0]))?.SourceCulture.Name;
					var targetCultureName = _languagePairs?.FirstOrDefault(n => n.TargetCulture.EnglishName.Equals(splittedLangPair[1]))?.TargetCulture.Name;

					foreach (var serviceLanguagePair in serviceLanguagePairs)
					{
						var existingTranslationModel = LanguageMappingsViewModel.TranslationOptions.FirstOrDefault(e => e.Model.Equals(serviceLanguagePair.Model));
						TranslationModel newTranslationModel = null;
						if (existingTranslationModel == null)
						{
							newTranslationModel = new TranslationModel
							{
								Model = serviceLanguagePair.Model,
								DisplayName = $"{serviceLanguagePair.SourceLanguageId}-{serviceLanguagePair.TargetLanguageId} {serviceLanguagePair.DisplayName}"
							};
							languageMapping.Engines.Add(newTranslationModel);
							languageMapping.SelectedModelOption = languageMapping?.Engines?[0];
							(existingTranslationModel ?? newTranslationModel).LanguagesSupported.Add(sourceCultureName, targetCultureName);
						}
					}
				}
				return true;
			}
			return false;
		}

		private void ChangePasswordAction(object parameter)
		{
			if (parameter.GetType().Name.Equals(_constants.PasswordBox))
			{
				var passwordBox = (PasswordBox)parameter;
				if (passwordBox.Password.Length > 0)
				{
					switch(passwordBox.Name)
					{
						case "ClientIdBox":
							Options.ClientId = passwordBox.Password;
							break;
						case "ClientSecretBox":
							Options.ClientSecret = passwordBox.Password;
							break;
						case "UserPasswordBox":
							Options.ClientSecret = passwordBox.Password;
							break;
					}
					BeGlobalWindowViewModel.Message = string.Empty;
				}
			}
			else
			{
				var textBox = (TextBox)parameter;
				if (textBox.Text.Length > 0)
				{
					BeGlobalWindowViewModel.Message = string.Empty;
					Options.ClientSecret = textBox.Text;
				}
			}
		}

		private void Navigate(object obj)
		{
			Process.Start("https://translate.sdlbeglobal.com/");
		}

		private void SetOptions(TranslationModel translationModel)
		{
			Options.Model = translationModel.Model;
			Options.DisplayName = translationModel.DisplayName;
			Options.LanguagesSupported = translationModel.LanguagesSupported;
		}
	}
}