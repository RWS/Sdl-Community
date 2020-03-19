using System;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Studio;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class OptionsWindowModel : BaseViewModel, IWindowContext
	{
		public static readonly Log Log = Log.Instance;

		private readonly OptionsWindow _window;		
		private readonly Languages.Provider.Languages _languages;

		private ICommand _okCommand;
		private int _selectedTabIndex;
		private string _message;
		
		public OptionsWindowModel(OptionsWindow window, SdlMTCloudTranslationOptions options,
			TranslationProviderCredential credentialStore, LanguagePair[] languagePairs, Languages.Provider.Languages languages)
		{
			_languages = languages;
						

			Options = options;
			LanguageMappingsViewModel = new LanguageMappingsViewModel(options, this, languagePairs, _languages);

			LoginViewModel = new LoginViewModel(options, languagePairs, LanguageMappingsViewModel, this);
			_window = window;

			if (credentialStore == null)
			{
				return;
			}

			if (options.AuthenticationMethod.Equals("ClientLogin"))
			{
				_window.LoginTab.ClientIdBox.Password = options.ClientId;
				_window.LoginTab.ClientSecretBox.Password = options.ClientSecret;
				LoginViewModel.SelectedOption = LoginViewModel.AuthenticationOptions[0];
			}
			else
			{
				LoginViewModel.Email = options.ClientId;
				_window.LoginTab.UserPasswordBox.Password = options.ClientSecret;
				LoginViewModel.SelectedOption = LoginViewModel.AuthenticationOptions[1];
			}

			var projectController = AppInitializer.GetProjectController();
			projectController.ProjectsChanged += ProjectController_ProjectsChanged;
		}

		public SdlMTCloudTranslationOptions Options { get; set; }

		public LoginViewModel LoginViewModel { get; set; }

		public LanguageMappingsViewModel LanguageMappingsViewModel { get; set; }

		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set
			{
				try
				{
					Mouse.OverrideCursor = Cursors.Wait;

					_selectedTabIndex = value;
					if (_selectedTabIndex == 1)
					{
						LanguageMappingsViewModel.LoadLanguageMappings();
					}

					ValidateWindow(true);
					OnPropertyChanged();
				}
				finally
				{
					Mouse.OverrideCursor = Cursors.Arrow;
				}
			}
		}
		
		public string Message
		{
			get => _message;
			set
			{
				if (_message == value)
				{
					return;
				}
				_message = value;
				OnPropertyChanged(nameof(Message));
			}
		}

		public bool IsProviderWindow => true;

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));

		private void Ok(object parameter)
		{
			try
			{
				Mouse.OverrideCursor = Cursors.Wait;

				if (parameter is LoginView loginTab)
				{
					var validateWindow = ValidateWindow(true);
					if (string.IsNullOrEmpty(validateWindow))
					{
						// Remove and add the new settings back to SettingsGroup of .sdlproj when user presses on Ok				
						LanguageMappingsViewModel.SaveLanguageMappingSettings();

						WindowCloser.SetDialogResult(_window, true);
						_window.Close();
					}
				}
			}
			finally
			{
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		/// <summary>
		/// Validate window
		/// </summary>
		/// <param name="isOkPressed">isOkPressed</param>
		/// <returns>message in case validation is not fine, otherwise, returns string.Empty</returns>
		public string ValidateWindow(bool isOkPressed)
		{
			var loginTab = _window?.LoginTab;
			Options.ResendDrafts = LanguageMappingsViewModel.ReSendChecked;
			try
			{
				if(!LanguageMappingsViewModel.LanguageMappings.Any())
				{
					Message = Constants.EnginesSelectionMessage;
					return Message;
				}

				if (LoginViewModel.SelectedOption.Type.Equals(Constants.User))
				{
					var password = loginTab?.UserPasswordBox.Password;
					if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(LoginViewModel.Email))
					{
						return GetEngineValidation(LoginViewModel?.Email, password, "UserLogin", isOkPressed);
					}
				}
				else
				{
					var clientId = loginTab?.ClientIdBox.Password;
					var clientSecret = loginTab?.ClientSecretBox.Password;
					if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
					{
						return GetEngineValidation(clientId, clientSecret, "ClientLogin", isOkPressed);
					}
				}

				if (loginTab != null)
				{
					Message = Constants.CredentialsValidation;
				}				
			}
			catch (Exception e)
			{
				if (loginTab != null)
				{
					Message = (e.Message.Contains(Constants.TokenFailed) || e.Message.Contains(Constants.NullValue)) ? Constants.CredentialsNotValid : e.Message;
				}

				Log.Logger.Error($"{Constants.IsWindowValid} {e.Message}\n {e.StackTrace}");
			}

			return Message;
		}

		/// <summary>
		/// Validate the engines setup
		/// </summary>
		/// <param name="clientId">clientId</param>
		/// <param name="clientSecret">clientSecret</param>
		/// <param name="authenticationMethod">authenticationMethod</param>
		/// <param name="isOkPressed">isOkPressed</param>
		/// <returns>Return message if the engines is not validated correctly, otherwise, returns string.Empty</returns>
		private string GetEngineValidation(string clientId, string clientSecret, string authenticationMethod, bool isOkPressed)
		{
			Options.ClientId = clientId.TrimEnd().TrimStart();
			Options.ClientSecret = clientSecret.TrimEnd().TrimStart();
			Options.AuthenticationMethod = authenticationMethod;
			Message = string.Empty;

			if (isOkPressed || LanguageMappingsViewModel.LanguageMappings.Any())
			{
				var isEngineSetup = LoginViewModel.ValidateEnginesSetup();
				if (!isEngineSetup)
				{
					Message = Constants.CredentialsAndInternetValidation;
				}
				else if (!LanguageMappingsViewModel.LanguageMappings.Any(l => l.Engines.Any()))
				{
					Message = Constants.NoEnginesLoaded;
					return Message;
				}

				return Message;
			}

			return Message;
		}

		private void ProjectController_ProjectsChanged(object sender, EventArgs e)
		{
			var projectsController = (ProjectsController)sender;
			var lastCreatedProj = projectsController?.GetAllProjects()?.LastOrDefault();
			var currentProj = projectsController?.CurrentProject;

			// Save language mappings configuration for the last created project
			// (used when user adds the provider through Project creation wizard)
			if (currentProj != null && currentProj.FilePath.Equals(lastCreatedProj?.FilePath))
			{
				LanguageMappingsViewModel.SaveLanguageMappingSettings();
			}
		}
	}
}