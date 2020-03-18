using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.Studio;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class LoginViewModel : BaseViewModel
	{
		private readonly LanguagePair[] _languagePairs;	
		private Authentication _selectedOption;
		private string _email;
		private string _loginMethod;
		
		private ICommand _passwordChangedCommand;
		private ICommand _navigateCommand;

		public LoginViewModel(
			SdlMTCloudTranslationOptions options,
			LanguagePair[] languagePairs,
			LanguageMappingsViewModel languageMappingsViewModel,
			OptionsWindowModel beGlobalWindowViewModel)
		{
			_languagePairs = languagePairs;

			LanguageMappingsViewModel = languageMappingsViewModel;
			BeGlobalWindowViewModel = beGlobalWindowViewModel;
			Options = options;

			AuthenticationOptions = new List<Authentication>
			{
				new Authentication
				{
					DisplayName = Constants.ClientAuthentication,
					Type = Constants.Client
				},
				new Authentication
				{   DisplayName = Constants.UserAuthentication,
					Type = Constants.User
				}
			};

			if (!string.IsNullOrEmpty(options.AuthenticationMethod))
			{
				SelectedOption = options.AuthenticationMethod.Equals("ClientLogin") 
					? AuthenticationOptions[0] 
					: AuthenticationOptions[1];
			}
			else
			{
				SelectedOption = AuthenticationOptions[1];
			}

			LoginMethod = SelectedOption.Type;
		}

		public OptionsWindowModel BeGlobalWindowViewModel { get; set; }

		public SdlMTCloudTranslationOptions Options { get; set; }

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
					LoginMethod = _selectedOption.Type.Equals(Constants.User) ? Constants.User : Constants.Client;
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

			var beGlobalTranslator = new SdlMTCloudTranslator(Constants.MTCloudTranslateAPIUri, Options);
			var accountId = Options.AuthenticationMethod.Equals("ClientLogin")
				? beGlobalTranslator.GetClientInformation()
				: beGlobalTranslator.GetUserInformation();

			var subscriptionInfo = beGlobalTranslator.GetLanguagePairs(accountId.ToString());
			Options.SubscriptionInfo = subscriptionInfo;

			var isEngineSet = GetEngineModels(subscriptionInfo?.LanguagePairs);
			if (!isEngineSet)
			{
				BeGlobalWindowViewModel.Message = Constants.CredentialsAndInternetValidation;
				return false;
			}

			return true;
		}

		private bool GetEngineModels(IReadOnlyCollection<BeGlobalLanguagePair> beGlobalLanguagePairs)
		{
			if (beGlobalLanguagePairs != null && LanguageMappingsViewModel?.LanguageMappings != null)
			{
				foreach (var languageMapping in LanguageMappingsViewModel?.LanguageMappings)
				{
					//get beGlobalLanguagePairs for the specific source language MTSourceCodes
					var sourcePairs = beGlobalLanguagePairs.Where(b =>
						languageMapping.MTCodesSource.Any(l => b.SourceLanguageId.Equals(l.CodeName)));

					//get beGlobalLanguagePairs for the specific target MTTargetCodes and exiting sourcePairs
					var serviceLanguagePairs =
						sourcePairs.Where(s => languageMapping.MTCodesTarget.Any(l => s.TargetLanguageId.Equals(l.CodeName)));

					var splittedLangPair = Utils.SplitLanguagePair(languageMapping.ProjectLanguagePair);


					var sourceCultureName = _languagePairs
						?.FirstOrDefault(n => n.SourceCulture.DisplayName.Equals(splittedLangPair[0]))?.SourceCulture.Name;
					var targetCultureName = _languagePairs
						?.FirstOrDefault(n => n.TargetCulture.DisplayName.Equals(splittedLangPair[1]))?.TargetCulture.Name;

					if (string.IsNullOrEmpty(sourceCultureName) || string.IsNullOrEmpty(targetCultureName))
					{
						return false;
					}

					foreach (var serviceLanguagePair in serviceLanguagePairs)
					{
						var existingTranslationModel = languageMapping.Engines.FirstOrDefault(e =>
							e.Model.Equals(serviceLanguagePair.Model)
							&& e.DisplayName.Contains(serviceLanguagePair.TargetLanguageId));

						TranslationModel newTranslationModel;
						if (existingTranslationModel == null)
						{
							newTranslationModel = new TranslationModel
							{
								Model = serviceLanguagePair.Model,
								DisplayName =
									$"{serviceLanguagePair.SourceLanguageId}-{serviceLanguagePair.TargetLanguageId} {serviceLanguagePair.DisplayName}"
							};

							newTranslationModel.LanguagesSupported.Add(sourceCultureName, targetCultureName);

							if (!languageMapping.Engines.Any(e => e.DisplayName.Equals(newTranslationModel.DisplayName)))
							{
								// the initialization is needed to display the progress ring while loading the engines
								// after user presses on the "Reset Langauge Mappings to default" button
								var currentEnginesCollection = languageMapping.Engines;
								languageMapping.Engines = new ObservableCollection<TranslationModel>(currentEnginesCollection)
								{
									newTranslationModel

								};

								languageMapping.SelectedModelOption = languageMapping.SelectedModelOption ?? languageMapping.Engines?[0];
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
			if (parameter.GetType().Name.Equals(Constants.PasswordBox))
			{
				var passwordBox = (PasswordBox)parameter;
				if (passwordBox.Password.Length > 0)
				{
					switch (passwordBox.Name)
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
			Process.Start(Constants.MTCloudTranslateUri);
		}
	}
}