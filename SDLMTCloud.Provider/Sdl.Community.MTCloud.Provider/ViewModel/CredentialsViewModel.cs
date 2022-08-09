using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Auth0Service;
using Auth0Service.ViewModel;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class CredentialsViewModel : BaseViewModel
	{
		private readonly Window _owner;
		private readonly IConnectionService _connectionService;

		private bool _isInProgress;
		private string _clientId;
		private string _clientSecret;
		private string _userName;
		private string _userPassword;
		private bool _isSignedIn;
		private bool _studioIsSignedIn;
		private string _studioSignedInAs;
		private string _signInLabel;
		private string _exceptionMessage;
		private List<Authentication> _authenticationOptions;
		private Authentication _selectedAuthentication;
		private ICommand _clearCommand;
		private WorkingPortal _selectedWorkingPortal;
		private string _currentWeaverWorkingPlatformsUriLogin; 
		private string _currentWeaverClientWorkingPlatformsUri;

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);



		private void Clear(object obj)
		{
			if (!(obj is string objectName)) return;

			switch (objectName)
			{
				case "UserName":
					UserName = string.Empty;
					break;
			}
		}

		public CredentialsViewModel(Window owner, IConnectionService connectionService)
		{
			_owner = owner;
			_connectionService = connectionService;
			FillWeaverWorkingPlatformsUrlLogin();
			SignInCommand = new CommandHandler(Signin);
			NavigateToCommand = new CommandHandler(NavigateTo);

			AuthenticationOptions = new List<Authentication>
			{
				new Authentication
				{
					Index = 0,
					DisplayName = Constants.StudioAuthentication,
					Type = Authentication.AuthenticationType.Studio
				},
				new Authentication
				{
					Index = 1,
					DisplayName = Constants.UserAuthentication,
					Type = Authentication.AuthenticationType.User
				},
				new Authentication
				{
					Index = 2,
					DisplayName = Constants.ClientAuthentication,
					Type = Authentication.AuthenticationType.Client
				}
			};

			var authentication = AuthenticationOptions.First(a => a.Type == connectionService.Credential.Type);

			if (connectionService.Credential.Type == Authentication.AuthenticationType.User)
			{
				UserName = connectionService.Credential.Name;
				UserPassword = connectionService.Credential.Password;
			}
			else if (connectionService.Credential.Type == Authentication.AuthenticationType.Client)
			{
				ClientId = connectionService.Credential.Name;
				ClientSecret = connectionService.Credential.Password;
			}

			SelectedAuthentication = authentication;
			SelectedWorkingPortal = connectionService.Credential.AccountRegion;
		}

		public ICommand SignInCommand { get; }

		public ICommand NavigateToCommand { get; }

		public bool IsInProgress
		{
			get => _isInProgress;
			set
			{
				_isInProgress = value;
				OnPropertyChanged(nameof(IsInProgress));
			}
		}

		public string UserName
		{
			get => _userName;
			set
			{
				if (_userName == value)
				{
					return;
				}

				_userName = value;
				OnPropertyChanged(nameof(UserName));
			}
		}

		public string UserPassword
		{
			get => _userPassword;
			set
			{
				if (_userPassword == value)
				{
					return;
				}

				_userPassword = value;
				OnPropertyChanged(nameof(UserPassword));
			}
		}

		public string ClientId
		{
			get => _clientId;
			set
			{
				if (_clientId == value)
				{
					return;
				}

				_clientId = value;
				OnPropertyChanged(nameof(ClientId));
			}
		}

		public string ClientSecret
		{
			get => _clientSecret;
			set
			{
				if (_clientSecret == value)
				{
					return;
				}

				_clientSecret = value;
				OnPropertyChanged(nameof(ClientSecret));
			}
		}

		public bool IsSignedIn
		{
			get => _isSignedIn;
			set
			{
				if (_isSignedIn == value)
				{
					return;
				}

				_isSignedIn = value;
				OnPropertyChanged(nameof(IsSignedIn));
			}
		}

		public bool StudioIsSignedIn
		{
			get => _studioIsSignedIn;
			set
			{
				if (_studioIsSignedIn == value)
				{
					return;
				}

				_studioIsSignedIn = value;
				OnPropertyChanged(nameof(StudioIsSignedIn));
			}
		}

		public string StudioSignedInAs
		{
			get => _studioSignedInAs;
			set
			{
				if (_studioSignedInAs == value)
				{
					return;
				}

				_studioSignedInAs = value;
				OnPropertyChanged(nameof(StudioSignedInAs));
			}
		}

		public string SignInLabel
		{
			get => _signInLabel;
			set
			{
				if (_signInLabel == value)
				{
					return;
				}

				_signInLabel = value;
				OnPropertyChanged(nameof(SignInLabel));
			}
		}

		public string ExceptionMessage
		{
			get => _exceptionMessage;
			set
			{
				if (_exceptionMessage == value)
				{
					return;
				}

				_exceptionMessage = value;
				ShowLoginUrls(_exceptionMessage);
				OnPropertyChanged(nameof(ExceptionMessage));
			}
		}

		private void ShowLoginUrls(string exceptionMessage)
		{
			if (!string.IsNullOrEmpty(_exceptionMessage))
			{
				WeaverWorkingPlatformsUriLogin.TryGetValue(SelectedWorkingPortal,
					out var currentWeaverWorkingPlatformsLogin);
				CurrentWeaverWorkingPlatformsUriLogin = currentWeaverWorkingPlatformsLogin;
				ClickingHere = PluginResources.ClickingHere;

			}
		}

		public WorkingPortal SelectedWorkingPortal
		{
			get => _selectedWorkingPortal;
			set
			{
				if (_selectedWorkingPortal == value)
				{
					return;
				}

				_selectedWorkingPortal = value;
				_connectionService.Credential.AccountRegion = _selectedWorkingPortal;
		
				WeaverWorkingPlatformsUriLogin.TryGetValue(_selectedWorkingPortal,
					out var currentWeaverWorkingPlatform);
				CurrentWeaverClientWorkingPlatformsUri = currentWeaverWorkingPlatform;

				ClearErrorMessageArea();
				OnPropertyChanged(nameof(SelectedWorkingPortal));
				
			}
		}

		private void ClearErrorMessageArea()
		{
			ExceptionMessage = string.Empty;
			ClickingHere = string.Empty;
			CurrentWeaverWorkingPlatformsUriLogin = string.Empty;
		}

		public Dictionary<WorkingPortal, string> WeaverWorkingPlatformsUriLogin;
		private string _clickingHere;
		private Auth0ControlViewModel _auth0ViewModel;

		public string ClickingHere
		{
			get
			{
				if (!string.IsNullOrEmpty(ExceptionMessage) &&
				    (ExceptionMessage.Equals(PluginResources.UnableToConnectToWorkingEUPortal) ||
				     ExceptionMessage.Equals(PluginResources.UnableToConnectToWorkingUSPortal)))

					return _clickingHere;
				return string.Empty;
			}
			set
			{
				if (_clickingHere == value)
				{
					return;
				}
				_clickingHere = value;
				OnPropertyChanged(nameof(ClickingHere));
			}
		}

		public string CurrentWeaverWorkingPlatformsUriLogin
		{
			get
			{
				if (!string.IsNullOrEmpty(ExceptionMessage) &&
					(ExceptionMessage.Equals(PluginResources.UnableToConnectToWorkingEUPortal) || ExceptionMessage.Equals(PluginResources.UnableToConnectToWorkingUSPortal)))
					return _currentWeaverWorkingPlatformsUriLogin;
				return string.Empty;
			}
			set
			{
				if (_currentWeaverWorkingPlatformsUriLogin == value)
				{
					return;
				}

				_currentWeaverWorkingPlatformsUriLogin = value;

				OnPropertyChanged(nameof(CurrentWeaverWorkingPlatformsUriLogin));
			}
		}


		public string CurrentWeaverClientWorkingPlatformsUri
		{
			get
			{
				return _currentWeaverClientWorkingPlatformsUri;
			
			}
			set
			{
				if (_currentWeaverClientWorkingPlatformsUri == value)
				{
					return;
				}

				_currentWeaverClientWorkingPlatformsUri = value;

				OnPropertyChanged(nameof(CurrentWeaverClientWorkingPlatformsUri));
			}
		}


		public List<Authentication> AuthenticationOptions
		{
			get => _authenticationOptions;
			set
			{
				_authenticationOptions = value;
				OnPropertyChanged(nameof(AuthenticationOptions));
			}
		}

		public Authentication SelectedAuthentication
		{
			get => _selectedAuthentication;
			set
			{
				if (_selectedAuthentication == value)
				{
					return;
				}

				_selectedAuthentication = value;
				OnPropertyChanged(nameof(SelectedAuthentication));

				ClearErrorMessageArea();

				if (_selectedAuthentication.Type == Authentication.AuthenticationType.Studio)
				{
					(StudioIsSignedIn, var message) = _connectionService.Connect(new Credential{Type = Authentication.AuthenticationType.Studio});
					StudioSignedInAs = StudioIsSignedIn ? _connectionService.Credential?.Name : string.Empty;
					SignInLabel = StudioIsSignedIn ? PluginResources.Label_SignOut : PluginResources.Label_Sign_In;
					ExceptionMessage = message != "OK" ? message : "";
				}
				else
				{
					SignInLabel = PluginResources.Label_Sign_In;
				}
			}
		}

		public Auth0ControlViewModel Auth0ViewModel
		{
			get => _auth0ViewModel;
			set
			{
				_auth0ViewModel = value; 
				OnPropertyChanged();
			}
		}

		private bool CanAttemptSignIn(bool showMessage = true)
		{
			ClearErrorMessageArea();

			switch (SelectedAuthentication.Type)
			{
				case Authentication.AuthenticationType.Studio:
					if (!StudioIsSignedIn)
					{
						if (showMessage)
						{
							ExceptionMessage = PluginResources.Message_User_is_signed_out;
						}

						return true;
					}
					else
					{
						return true;
					}
				case Authentication.AuthenticationType.User:
					if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword))
					{
						if (showMessage)
						{
							ExceptionMessage = PluginResources.Message_Please_verify_your_credentials;
						}

						return false;
					}
					else
					{
						ClearErrorMessageArea();
						return true;
					}
				case Authentication.AuthenticationType.Client:
					if (string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(ClientSecret))
					{
						if (showMessage)
						{
							ExceptionMessage = PluginResources.Message_Please_verify_your_credentials;
						}

						return false;
					}
					else
					{
						ClearErrorMessageArea();
						return true;
					}
				default:
					ClearErrorMessageArea();
					return true;
			}
		}

		private void Signin(object parameter)
		{
			if (parameter.ToString() == "Sign out")
			{
				IsInProgress = true;
				StudioSignOut();
				IsInProgress = false;

				StudioSignedInAs = null;
				StudioIsSignedIn = false;
				return;
			}

			if (!CanAttemptSignIn())
			{
				return;
			}

			IsInProgress = true;
			if (_owner != null)
			{
				Mouse.OverrideCursor = Cursors.Wait;
			}

			try
			{
				var message = string.Empty;
				IsSignedIn = false;

				if (SelectedAuthentication.Type == Authentication.AuthenticationType.Studio)
				{
					// Studio SSO will use the studio credentials											
					var result = _connectionService.Connect(
						new Credential
						{
							Type = Authentication.AuthenticationType.Studio,
							AccountRegion = SelectedWorkingPortal
						}, parameter.ToString() != "Use");

					IsSignedIn = result.Item1;
					message = result.Item2;
					StudioSignedInAs = _connectionService.Credential.Name;
					SignInLabel = IsSignedIn ? PluginResources.Label_SignOut : PluginResources.Label_Sign_In;
				}
				else if (SelectedAuthentication.Type == Authentication.AuthenticationType.User)
				{
					//var serviceCredential = _connectionService.Credential ?? new Credential();
					var serviceCredential = new Credential()
					{
						Type = Authentication.AuthenticationType.User,
						Name = UserName,
						Password = UserPassword,
						AccountRegion = SelectedWorkingPortal
					};
					var result = _connectionService.Connect(serviceCredential);

					IsSignedIn = result.Item1;
					message = result.Item2;
				}
				else if (SelectedAuthentication.Type == Authentication.AuthenticationType.Client)
				{
					//var serviceCredential = _connectionService.Credential ?? new Credential();
					var serviceCredential = new Credential
					{
						Type = Authentication.AuthenticationType.Client,
						Name = ClientId,
						Password = ClientSecret,
						AccountRegion = SelectedWorkingPortal
					};
					var result = _connectionService.Connect(serviceCredential);

					IsSignedIn = result.Item1;
					message = result.Item2;
				}

				ExceptionMessage = IsSignedIn ? string.Empty : GetLoginFailMessagePlatformRelated(message);
			}
			catch (Exception ex)
			{
				ExceptionMessage = ex.Message;
			}
			finally
			{
				IsInProgress = false;
				if (_owner != null)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
				}

				if (IsSignedIn && _owner != null)
				{
					_owner.DialogResult = true;
					_owner.Close();
				}
			}
		}

		private void StudioSignOut()
		{
			_connectionService.SignOut();
			SignInLabel = PluginResources.Label_Sign_In;
		}

		private string GetLoginFailMessagePlatformRelated(string message)
		{
			if (message.Contains(PluginResources.Message_Please_verify_your_credentials))
				switch (SelectedWorkingPortal)
				{
					case WorkingPortal.UEPortal: return PluginResources.UnableToConnectToWorkingEUPortal;

					case WorkingPortal.USPortal:
						return PluginResources.UnableToConnectToWorkingUSPortal;

					default:
						throw new ArgumentOutOfRangeException();
				}

			return message;
		}

		private void NavigateTo(object obj)
		{
			var value = obj.ToString().Trim();
			if (!string.IsNullOrEmpty(value))
			{
				Process.Start(value);
			}
		}

		private void FillWeaverWorkingPlatformsUrlLogin()
		{
			WeaverWorkingPlatformsUriLogin = new Dictionary<WorkingPortal, string>
			{
					{WorkingPortal.UEPortal, Constants.MTCloudTranslateAPIUrlEULogin},
					{WorkingPortal.USPortal, Constants.MTCloudTranslateAPIUrlUSLogin}
				};
		}

	}
}
