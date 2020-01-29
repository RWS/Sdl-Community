using System.Collections.Generic;
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
		private readonly LanguagePair[] _languagePairs;
		private readonly string _serverAddress = "https://translate-api.sdlbeglobal.com";
		private Constants _constants = new Constants();

		private ICommand _passwordChangedCommand;
		private ICommand _navigateCommand;

		public LoginViewModel(
			BeGlobalTranslationOptions options,
			LanguagePair[] languagePairs,
			LanguageMappingsViewModel languageMappingsViewModel,
			BeGlobalWindowViewModel beGlobalWindowViewModel)
		{
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
			SelectedOption = options.AuthenticationMethod.Equals("ClientLogin") ? AuthenticationOptions[0] : AuthenticationOptions[1];
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
			Utils.LogServerIPAddresses();

			var beGlobalTranslator = new BeGlobalV4Translator(_serverAddress, Options);
			var accountId = Options.AuthenticationMethod.Equals("ClientLogin") ? beGlobalTranslator.GetClientInformation() : beGlobalTranslator.GetUserInformation();
			var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
			Options.SubscriptionInfo = subscriptionInfo;
			
			var isEngineSet = GetEngineModels(subscriptionInfo?.LanguagePairs);
			if (!isEngineSet)
			{
				BeGlobalWindowViewModel.Message = _constants.CredentialsAndInternetValidation;
				return false;
			}
			return true;
		}
			
		private bool GetEngineModels(List<BeGlobalLanguagePair> beGlobalLanguagePairs)
		{			
			if (beGlobalLanguagePairs != null)
			{
				foreach (var languageMapping in LanguageMappingsViewModel?.LanguageMappings)
				{
					//get beGlobalLanguagePairs for the specific source language MTSourceCodes
					var sourcePairs = beGlobalLanguagePairs.Where(b => languageMapping.MTCodesSource.Any(l => b.SourceLanguageId.Equals(l)));

					//get beGlobalLanguagePairs for the specific target MTTargetCodes and exiting sourcePairs
					var serviceLanguagePairs = sourcePairs.Where(s => languageMapping.MTCodesTarget.Any(l => s.TargetLanguageId.Equals(l)));
					var splittedLangPair = Utils.SplitLanguagePair(languageMapping.ProjectLanguagePair);
					var sourceCultureName = _languagePairs?.FirstOrDefault(n => n.SourceCulture.DisplayName.Equals(splittedLangPair[0]))?.SourceCulture.Name;
					var targetCultureName = _languagePairs?.FirstOrDefault(n => n.TargetCulture.DisplayName.Equals(splittedLangPair[1]))?.TargetCulture.Name;

					if(string.IsNullOrEmpty(sourceCultureName) || string.IsNullOrEmpty(targetCultureName))
					{
						return false;
					}
					foreach (var serviceLanguagePair in serviceLanguagePairs)
					{
						var existingTranslationModel = languageMapping.Engines.FirstOrDefault(e => e.Model.Equals(serviceLanguagePair.Model) 
																			&& e.DisplayName.Contains(serviceLanguagePair.TargetLanguageId));
						TranslationModel newTranslationModel = null;
						if (existingTranslationModel == null)
						{
							newTranslationModel = new TranslationModel
							{
								Model = serviceLanguagePair.Model,
								DisplayName = $"{serviceLanguagePair.SourceLanguageId}-{serviceLanguagePair.TargetLanguageId} {serviceLanguagePair.DisplayName}"
							};
							(existingTranslationModel ?? newTranslationModel).LanguagesSupported.Add(sourceCultureName, targetCultureName);

							if (!languageMapping.Engines.Any(e=>e.DisplayName.Equals(newTranslationModel.DisplayName)))
							{
								languageMapping.Engines.Add(newTranslationModel);
								languageMapping.SelectedModelOption = languageMapping?.SelectedModelOption != null ? languageMapping?.SelectedModelOption : languageMapping?.Engines?[0];
							}
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
							Options.AuthenticationMethod = "ClientLogin";
							break;
						case "ClientSecretBox":
							Options.ClientSecret = passwordBox.Password;
							Options.AuthenticationMethod = "ClientLogin";
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
					Options.ClientId = textBox.Text;
					Options.AuthenticationMethod = "UserLogin";
				}
			}
		}

		private void Navigate(object obj)
		{
			Process.Start("https://translate.sdlbeglobal.com/");
		}
	}
}