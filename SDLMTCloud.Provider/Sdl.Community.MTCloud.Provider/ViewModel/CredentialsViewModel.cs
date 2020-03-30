using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class CredentialsViewModel : BaseViewModel
	{
		private readonly Window _parentWindow;
		private readonly ConnectionService _connectionService;

		private bool _isInProgress;
		private string _clientId;
		private string _clientSecret;
		private string _userName;
		private string _userPassword;
		private bool _studioSignedIn;
		private string _studioSignedInAs;
		private string _signInLabel;
		private string _exceptionMessage;
		private List<Authentication> _authenticationOptions;
		private Authentication _selectedAuthentication;

		public CredentialsViewModel(Window parentWindow, ConnectionService connectionService)
		{
			_parentWindow = parentWindow;
			_connectionService = connectionService;

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

		public bool StudioSignedIn
		{
			get => _studioSignedIn;
			set
			{
				if (_studioSignedIn == value)
				{
					return;
				}

				_studioSignedIn = value;
				OnPropertyChanged(nameof(StudioSignedIn));
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
				OnPropertyChanged(nameof(ExceptionMessage));
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

				ExceptionMessage = string.Empty;

				if (_selectedAuthentication.Type == Authentication.AuthenticationType.Studio)
				{
					StudioSignedIn = _connectionService.IsSignedInStudioAuthentication(out var user);
					StudioSignedInAs = user;
					SignInLabel = StudioSignedIn ? PluginResources.Label_OK : PluginResources.Label_Sign_In;
				}
				else
				{
					SignInLabel = PluginResources.Label_Sign_In;
				}
			}
		}	

		private bool CanAttemptSignIn(bool showMessage = true)
		{
			ExceptionMessage = string.Empty;

			switch (SelectedAuthentication.Type)
			{
				case Authentication.AuthenticationType.Studio:
					if (!StudioSignedIn)
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
						ExceptionMessage = string.Empty;
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
						ExceptionMessage = string.Empty;
						return true;
					}
				default:
					ExceptionMessage = string.Empty;
					return true;
			}
		}

		private void Signin(object obj)
		{
			if (!CanAttemptSignIn())
			{
				return;
			}

			IsInProgress = true;
			Mouse.OverrideCursor = Cursors.Wait;

			try
			{
				var message = string.Empty;

				if (SelectedAuthentication.Type == Authentication.AuthenticationType.Studio)
				{
					// Studio SSO will use the studio credentials											
					var result = _connectionService.Connect(
						new Credential
						{
							Type = Authentication.AuthenticationType.Studio
						});

					StudioSignedIn = result.Item1;
					message = result.Item2;
					StudioSignedInAs = _connectionService.Credential.Name;
					SignInLabel = StudioSignedIn ? PluginResources.Label_OK : PluginResources.Label_Sign_In;
				}
				else if (SelectedAuthentication.Type == Authentication.AuthenticationType.User)
				{				
					var result = _connectionService.Connect(
						new Credential
						{
							Type = Authentication.AuthenticationType.User,
							Name = UserName,
							Password = UserPassword
						});

					StudioSignedIn = result.Item1;
					message = result.Item2;
				}
				else if (SelectedAuthentication.Type == Authentication.AuthenticationType.Client)
				{											
					var result = _connectionService.Connect(
						new Credential
						{
							Type = Authentication.AuthenticationType.Client,
							Name = ClientId,
							Password = ClientSecret
						});

					StudioSignedIn = result.Item1;
					message = result.Item2;
				}

				ExceptionMessage = StudioSignedIn ? string.Empty : message;
			}
			catch (Exception ex)
			{
				ExceptionMessage = ex.Message;
			}
			finally
			{
				IsInProgress = false;
				Mouse.OverrideCursor = Cursors.Arrow;

				if (StudioSignedIn)
				{
					_parentWindow.DialogResult = true;
					_parentWindow.Close();
				}
			}
		}		

		private void NavigateTo(object obj)
		{
			var value = obj.ToString().Trim();
			if (!string.IsNullOrEmpty(value))
			{
				Process.Start(value);
			}
		}
	}
}
