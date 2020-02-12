using System;
using System.Linq;
using System.Windows.Input;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BeGlobalWindowViewModel : BaseViewModel
	{
		public BeGlobalTranslationOptions Options { get; set; }
		public LoginViewModel LoginViewModel { get; set; }
		public LanguageMappingsViewModel LanguageMappingsViewModel { get; set; }
		private ICommand _okCommand;
		private int _selectedTabIndex;
		private string _message;
		private bool _isMTCodeIconVisible;
		private bool _isProviderIconVisible;
		private Constants _constants = new Constants();
		private readonly BeGlobalWindow _mainWindow;
		public static readonly Log Log = Log.Instance;

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options,
			TranslationProviderCredential credentialStore, LanguagePair[] languagePairs)
		{
			IsMTCodeIconVisible = false;
			IsProviderIconVisible = true;

			Options = options;
			LanguageMappingsViewModel = new LanguageMappingsViewModel(options, this, languagePairs);

			LoginViewModel = new LoginViewModel(options, languagePairs, LanguageMappingsViewModel, this);
			_mainWindow = mainWindow;

			if (credentialStore == null) return;
			if (options.AuthenticationMethod.Equals("ClientLogin"))
			{
				_mainWindow.LoginTab.ClientIdBox.Password = options.ClientId;
				_mainWindow.LoginTab.ClientSecretBox.Password = options.ClientSecret;
				LoginViewModel.SelectedOption = LoginViewModel.AuthenticationOptions[0];
			}
			else
			{
				LoginViewModel.Email = options.ClientId;
				_mainWindow.LoginTab.UserPasswordBox.Password = options.ClientSecret;
				LoginViewModel.SelectedOption = LoginViewModel.AuthenticationOptions[1];
			}

			var projectController = AppInitializer.GetProjectController();
			projectController.ProjectsChanged += ProjectController_ProjectsChanged;
		}
		
		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set
			{
				Mouse.OverrideCursor = Cursors.Wait;
				_selectedTabIndex = value;
				if (_selectedTabIndex == 1)
				{
					LanguageMappingsViewModel.LoadLanguageMappings();
				}			
				ValidateWindow(true);				
				
				OnPropertyChanged();
				Mouse.OverrideCursor = Cursors.Arrow;
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

		public bool IsMTCodeIconVisible
		{
			get => _isMTCodeIconVisible;
			set
			{
				_isMTCodeIconVisible = value;
				OnPropertyChanged(nameof(IsMTCodeIconVisible));
			}
		}

		public bool IsProviderIconVisible
		{
			get => _isProviderIconVisible;
			set
			{
				_isProviderIconVisible = value;
				OnPropertyChanged(nameof(IsProviderIconVisible));
			}
		}

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));

		private void Ok(object parameter)
		{
			Mouse.OverrideCursor = Cursors.Wait;
			var loginTab = parameter as Login;
			if (loginTab != null)
			{
				var validateWindow = ValidateWindow(true);
				if (string.IsNullOrEmpty(validateWindow))
				{
					// Remove and add the new settings back to SettingsGroup of .sdlproj when user presses on Ok				
					LanguageMappingsViewModel.SaveLanguageMappingSettings();

					WindowCloser.SetDialogResult(_mainWindow, true);
					_mainWindow.Close();
				}
			}
			Mouse.OverrideCursor = Cursors.Arrow;
		}

		/// <summary>
		/// Validate window
		/// </summary>
		/// <param name="isOkPressed">isOkPressed</param>
		/// <returns>message in case validation is not fine, otherwise, returns string.Empty</returns>
		public string ValidateWindow(bool isOkPressed)
		{
			var loginTab = _mainWindow?.LoginTab;
			Options.ResendDrafts = LanguageMappingsViewModel.ReSendChecked;
			try
			{
				if(!LanguageMappingsViewModel.LanguageMappings.Any())
				{
					Message = _constants.EnginesSelectionMessage;
					return Message;
				}

				if (LoginViewModel.SelectedOption.Type.Equals(_constants.User))
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
					Message = _constants.CredentialsValidation;
				}				
			}
			catch (Exception e)
			{
				if (loginTab != null)
				{
					Message = (e.Message.Contains(_constants.TokenFailed) || e.Message.Contains(_constants.NullValue)) ? _constants.CredentialsNotValid : e.Message;
				}
				Log.Logger.Error($"{_constants.IsWindowValid} {e.Message}\n {e.StackTrace}");
			}
			return Message;
		}
		
		/// <summary>
		/// Validate the engines setup
		/// </summary>
		/// <param name="clientId">clientId</param>
		/// <param name="clientSecret">clientSecret</param>
		/// <param name="useClientAuthentication">useClientAuthentication</param>
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
					Message = _constants.CredentialsAndInternetValidation;
				}
				else if (!LanguageMappingsViewModel.LanguageMappings.Any(l => l.Engines.Any()))
				{
					Message = _constants.NoEnginesLoaded;
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