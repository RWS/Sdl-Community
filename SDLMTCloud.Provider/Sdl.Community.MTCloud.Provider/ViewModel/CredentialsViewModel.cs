using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Auth0Service.ViewModel;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Window = System.Windows.Window;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class CredentialsViewModel : BaseViewModel
	{
		private readonly IConnectionService _connectionService;
		private readonly Window _owner;

		private bool _isInProgress;
		private bool _isSignedIn;
		private bool _studioIsSignedIn;

		private string _studioSignedInAs;
		private string _userName;
		private string _userPassword;
		private string _clientId;
		private string _clientSecret;
		private string _clickingHere;
		private string _exceptionMessage;

		private string _currentWeaverWorkingPlatformsUriLogin; 
		private string _currentWeaverClientWorkingPlatformsUri;

		private List<Authentication> _authenticationOptions;
		private Authentication _selectedAuthentication;
		private WorkingPortal _selectedWorkingPortal;

		private Auth0ControlViewModel _auth0ViewModel;

		private ICommand _clearCommand;
		private ICommand _signInCommand;
		private ICommand _signOutCommand;
		private ICommand _navigateToCommand;

		public CredentialsViewModel(Window owner, IConnectionService connectionService)
		{
			_owner = owner;
			_connectionService = connectionService;
			FillWeaverWorkingPlatformsUrlLogin();
			FillAuthenticationOptions();
			FillCredentials();
		}

		public bool IsInProgress
		{
			get => _isInProgress;
			set
			{
				if (_owner is not null)
				{
					Mouse.OverrideCursor = value ? Cursors.Wait : Cursors.Arrow;
				}

				_isInProgress = value;
				OnPropertyChanged();
			}
		}

		public bool IsSignedIn
		{
			get => _isSignedIn;
			set
			{
				if (_isSignedIn == value) return;
				_isSignedIn = value;
				OnPropertyChanged(nameof(IsSignedIn));
			}
		}

		public bool StudioIsSignedIn
		{
			get => _studioIsSignedIn;
			set
			{
				if (_studioIsSignedIn == value) return;
				_studioIsSignedIn = value;
				OnPropertyChanged(nameof(StudioIsSignedIn));
			}
		}

		public string StudioSignedInAs
		{
			get => _studioSignedInAs;
			set
			{
				if (_studioSignedInAs == value) return;
				_studioSignedInAs = value;
				OnPropertyChanged(nameof(StudioSignedInAs));
			}
		}

		public string UserName
		{
			get => _userName;
			set
			{
				if (_userName == value) return;
				_userName = value;
				OnPropertyChanged(nameof(UserName));
			}
		}

		public string UserPassword
		{
			get => _userPassword;
			set
			{
				if (_userPassword == value) return;
				_userPassword = value;
				OnPropertyChanged(nameof(UserPassword));
			}
		}

		public string ClientId
		{
			get => _clientId;
			set
			{
				if (_clientId == value) return;
				_clientId = value;
				OnPropertyChanged(nameof(ClientId));
			}
		}

		public string ClientSecret
		{
			get => _clientSecret;
			set
			{
				if (_clientSecret == value) return;
				_clientSecret = value;
				OnPropertyChanged(nameof(ClientSecret));
			}
		}

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
				if (_clickingHere == value) return;
				_clickingHere = value;
				OnPropertyChanged(nameof(ClickingHere));
			}
		}

		public string ExceptionMessage
		{
			get => _exceptionMessage;
			set
			{
				if (_exceptionMessage == value) return;
				_exceptionMessage = value;
				ShowLoginUrls();
				OnPropertyChanged(nameof(ExceptionMessage));
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
				if (_currentWeaverWorkingPlatformsUriLogin == value) return;
				_currentWeaverWorkingPlatformsUriLogin = value;
				OnPropertyChanged(nameof(CurrentWeaverWorkingPlatformsUriLogin));
			}
		}

		public string CurrentWeaverClientWorkingPlatformsUri
		{
			get => _currentWeaverClientWorkingPlatformsUri;
			set
			{
				if (_currentWeaverClientWorkingPlatformsUri == value) return;
				_currentWeaverClientWorkingPlatformsUri = value;
				OnPropertyChanged(nameof(CurrentWeaverClientWorkingPlatformsUri));
			}
		}

		public WorkingPortal SelectedWorkingPortal
		{
			get => _selectedWorkingPortal;
			set
			{
				_selectedWorkingPortal = value;
				_connectionService.Credential.AccountRegion = _selectedWorkingPortal;
		
				WeaverWorkingPlatformsUriLogin.TryGetValue(_selectedWorkingPortal,
					out var currentWeaverWorkingPlatform);
				CurrentWeaverClientWorkingPlatformsUri = currentWeaverWorkingPlatform;

				ClearErrorMessageArea();
				OnPropertyChanged(nameof(SelectedWorkingPortal));
				
			}
		}

		public List<Authentication> AuthenticationOptions
		{
			get => _authenticationOptions;
			set
			{
				if (_authenticationOptions == value) return;
				_authenticationOptions = value;
				OnPropertyChanged(nameof(AuthenticationOptions));
			}
		}

		public Authentication SelectedAuthentication
		{
			get => _selectedAuthentication;
			set
			{
				if (_selectedAuthentication == value) return;
				IsInProgress = false;
				_selectedAuthentication = value;
				OnPropertyChanged(nameof(SelectedAuthentication));

				ClearErrorMessageArea();

				if (_selectedAuthentication.Type != Authentication.AuthenticationType.Studio)
				{
					return;
				}

				(StudioIsSignedIn, var message) = _connectionService.Connect(new Credential { Type = Authentication.AuthenticationType.Studio });
				StudioSignedInAs = StudioIsSignedIn ? _connectionService.Credential?.Name : string.Empty;
				ExceptionMessage = message != "OK" ? message : "";
			}
		}

		public Dictionary<WorkingPortal, string> WeaverWorkingPlatformsUriLogin { get; set; }

		public Auth0ControlViewModel Auth0ViewModel

		{
			get => _auth0ViewModel;
			set
			{
				_auth0ViewModel = value; 
				OnPropertyChanged();
			}
		}

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		public ICommand LoginCommand => _signInCommand ??= new RelayCommand(Signin);

		public ICommand LogoutCommand => _signOutCommand ??= new RelayCommand(Signout);

		public ICommand NavigateToCommand => _navigateToCommand ??= new RelayCommand(NavigateTo);

		private void ShowLoginUrls()
		{
			if (string.IsNullOrEmpty(_exceptionMessage))
			{
				return;
			}

			WeaverWorkingPlatformsUriLogin.TryGetValue(SelectedWorkingPortal, out var currentWeaverWorkingPlatformsLogin);
			CurrentWeaverWorkingPlatformsUriLogin = currentWeaverWorkingPlatformsLogin;
			ClickingHere = PluginResources.ClickingHere;
		}

		private void FillWeaverWorkingPlatformsUrlLogin()
		{
			WeaverWorkingPlatformsUriLogin = new Dictionary<WorkingPortal, string>
			{
				{WorkingPortal.UEPortal, Constants.MTCloudTranslateAPIUrlEULogin},
				{WorkingPortal.USPortal, Constants.MTCloudTranslateAPIUrlUSLogin}
			};
		}

		private void FillAuthenticationOptions()
		{
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

			var authentication = AuthenticationOptions.First(a => a.Type == _connectionService.Credential.Type);
			SelectedAuthentication = authentication;
			SelectedWorkingPortal = _connectionService.Credential.AccountRegion;
		}

		private void FillCredentials()
		{
			if (_connectionService.Credential.Type == Authentication.AuthenticationType.User)
			{
				UserName = _connectionService.Credential.Name;
				UserPassword = _connectionService.Credential.Password;
			}
			else if (_connectionService.Credential.Type == Authentication.AuthenticationType.Client)
			{
				ClientId = _connectionService.Credential.Name;
				ClientSecret = _connectionService.Credential.Password;
			}
		}

		private void Clear(object o)
		{
			if (o is not string objectName)
			{
				return;
			}

			switch (objectName)
			{
				case "UserName":
					UserName = string.Empty;
					break;
			}
		}

		private void Signin(object o)
		{
			if (!CanAttemptSignIn())
			{
				return;
			}

			try
			{
				IsInProgress = true;
				IsSignedIn = false;
				TrySignin();
			}
			catch (Exception e)
			{
				ExceptionMessage = e.Message;
			}
			finally
			{
				IsInProgress = false;
				if (_owner is not null && IsSignedIn)
				{
					_owner.DialogResult = true;
					_owner.Close();
				}
			}
		}

		private bool CanAttemptSignIn(bool showMessage = true)
		{
			ClearErrorMessageArea();
			var authenticationhType = SelectedAuthentication.Type;
			var isUser = authenticationhType == Authentication.AuthenticationType.User;
			var isUserValid = !(string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserPassword));
			var isClient = authenticationhType == Authentication.AuthenticationType.Client;
			var isClientValid = !(string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(ClientSecret));

			if ((isUser && !isUserValid) || (isClient && !isClientValid))
			{
				if (!showMessage)
				{
					return false;
				}

				ExceptionMessage = PluginResources.Message_Please_verify_your_credentials;
			}

			return authenticationhType == Authentication.AuthenticationType.Studio
				|| string.IsNullOrEmpty(ExceptionMessage);
		}

		private void TrySignin()
		{
			var credentials = GetCredentials();

			var useSingleSignOn = SelectedAuthentication.Type == Authentication.AuthenticationType.Studio;
			var showDialog = useSingleSignOn && !StudioIsSignedIn;

			var (isSuccesful, responseMessage) = _connectionService.Connect(credentials, showDialog);
			IsSignedIn = isSuccesful;
			ExceptionMessage = IsSignedIn ? string.Empty : GetLoginFailMessagePlatformRelated(responseMessage);

			if (credentials.Type == Authentication.AuthenticationType.Studio)
			{
				StudioIsSignedIn = IsSignedIn;
				StudioSignedInAs = _connectionService.Credential.Name;
			}
		}

		private Credential GetCredentials()
		{
			return SelectedAuthentication.Type switch
			{
				Authentication.AuthenticationType.Studio => new Credential
				{
					Type = Authentication.AuthenticationType.Studio,
					AccountRegion = SelectedWorkingPortal
				},
				Authentication.AuthenticationType.User => new Credential()
				{
					Type = Authentication.AuthenticationType.User,
					Name = UserName,
					Password = UserPassword,
					AccountRegion = SelectedWorkingPortal
				},
				Authentication.AuthenticationType.Client => new Credential
				{
					Type = Authentication.AuthenticationType.Client,
					Name = ClientId,
					Password = ClientSecret,
					AccountRegion = SelectedWorkingPortal
				},
				_ => null
			};
		}

		private void Signout(object o)
		{
			IsInProgress = true;
			_connectionService.SignOut();
			StudioSignedInAs = null;
			StudioIsSignedIn = false;
			IsInProgress = false;
		}

		private string GetLoginFailMessagePlatformRelated(string message)
		{
			if (!message.Contains(PluginResources.Message_Please_verify_your_credentials))
			{
				return message;
			}

			return SelectedWorkingPortal switch
			{
				WorkingPortal.UEPortal => PluginResources.UnableToConnectToWorkingEUPortal,
				WorkingPortal.USPortal => PluginResources.UnableToConnectToWorkingUSPortal,
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		private void ClearErrorMessageArea()
		{
			ExceptionMessage = string.Empty;
			ClickingHere = string.Empty;
			CurrentWeaverWorkingPlatformsUriLogin = string.Empty;
		}

		private void NavigateTo(object obj)
		{
			if (obj is Uri uri)
			{
				Process.Start(uri.AbsoluteUri);
				return;
			}

			if (obj is not string value)
			{
				return;
			}

			value = value.Trim();
			if (!string.IsNullOrEmpty(value))
			{
				Process.Start(value);
			}
		}
	}
}