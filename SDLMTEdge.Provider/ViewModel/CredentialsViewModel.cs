using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using Sdl.Community.MTEdge.Provider.Command;
using Sdl.Community.MTEdge.Provider.Helpers;
using Sdl.Community.MTEdge.Provider.Interface;
using Sdl.Community.MTEdge.Provider.Model;

namespace Sdl.Community.MTEdge.Provider.ViewModel
{
	public class CredentialsViewModel : BaseModel, ICredentialsViewModel
	{
		private const string BasicCredentialsMethod = "Basic credentials";
		private const string ApiKeyMethod = "API Key";
		private const string Auth0SSOMethod = "SSO";

		private readonly ITranslationOptions _translationOptions;

		private string _port;
		private string _host;
		private string _apiKey;
		private string _username;
		private string _password;
		private List<string> _autheticationMethods;
		private string _selectedAuthenticationMethod;

		private bool _persistsHost;
		private bool _requiresSecureProtocol;
		private bool _persistsApiKey;
		private bool _persistsCredentials;
		private bool _useBasicCredentials;
		private bool _useApiKey;
		private bool _useAuth0SSO;

		private ICommand _clearCommand;

		public CredentialsViewModel(ITranslationOptions options, bool showSettingsView = false)
		{
			ViewModel = this;
			_translationOptions = options;
			RequiresSecureProtocol = true;
			InitializeView();
		}

		public BaseModel ViewModel { get; set; }

		public string Port
		{
			get => _port ??= _translationOptions.Port.ToString();
			set
			{
				if (_port == value) return;
				if (value.Any(x => !char.IsDigit(x)))
				{
					return;
				}

				_port = value;
				OnPropertyChanged(nameof(Port));
				_translationOptions.Port = Convert.ToInt32(value);
			}
		}

		public string Host
		{
			get => _host;
			set
			{
				if (_host == value) return;
				_host = value;
				OnPropertyChanged(nameof(Host));
			}
		}

		public string ApiKey
		{
			get => _apiKey;
			set
			{
				if (value == _apiKey) return;
				_apiKey = value;
				OnPropertyChanged(nameof(ApiKey));
			}
		}

		public string UserName
		{
			get => _username;
			set
			{
				if (_username == value) return;
				_username = value;
				OnPropertyChanged(nameof(UserName));
			}
		}

		public string Password
		{
			get => _password;
			set
			{
				if (_password == value) return;
				_password = value;
				OnPropertyChanged(nameof(Password));
			}
		}

		public bool PersistsHost
		{
			get => _persistsHost;
			set
			{
				if (_persistsHost == value) return;
				_persistsHost = value;
				OnPropertyChanged(nameof(PersistsHost));
			}
		}

		public bool RequiresSecureProtocol
		{
			get => _requiresSecureProtocol;
			set
			{
				if (_requiresSecureProtocol == value) return;
				_requiresSecureProtocol = value;
				OnPropertyChanged(nameof(RequiresSecureProtocol));
				_translationOptions.RequiresSecureProtocol = value;
			}
		}

		public bool PersistsCredentials
		{
			get => _persistsCredentials;
			set
			{
				if (_persistsCredentials == value) return;
				_persistsCredentials = value;
				OnPropertyChanged(nameof(PersistsCredentials));
				_translationOptions.PersistCredentials = value;
			}
		}

		public bool PersistsApiKey
		{
			get => _persistsApiKey;
			set
			{
				if (_persistsApiKey == value) return;
				_persistsApiKey = value;
				OnPropertyChanged(nameof(PersistsApiKey));
			}
		}

		public bool UseBasicCredentials
		{
			get => _useBasicCredentials;
			set
			{
				if (_useBasicCredentials == value) return;
				_useBasicCredentials = value;
				OnPropertyChanged(nameof(UseBasicCredentials));
				_translationOptions.UseBasicAuthentication = UseBasicCredentials;
			}
		}

		public bool UseApiKey
		{
			get => _useApiKey;
			set
			{
				if (_useApiKey == value) return;
				_useApiKey = value;
				OnPropertyChanged(nameof(UseApiKey));
				_translationOptions.UseApiKey = value;
			}
		}

		public bool UseAuth0SSO
		{
			get => _useAuth0SSO;
			set
			{
				if (_useAuth0SSO == value) return;
				_useAuth0SSO = value;
				OnPropertyChanged(nameof(UseAuth0SSO));
				_translationOptions.UseAuth0SSO = value;
			}
		}

		public List<string> AuthenticationMethods
		{
			get => _autheticationMethods;
			set
			{
				if (_autheticationMethods == value) return;
				_autheticationMethods = value;
				OnPropertyChanged(nameof(AuthenticationMethods));
			}
		}

		public string SelectedAuthenticationMethod
		{
			get => _selectedAuthenticationMethod;
			set
			{
				if (_selectedAuthenticationMethod == value) return;
				_selectedAuthenticationMethod = value;
				UseBasicCredentials = _selectedAuthenticationMethod.Equals(BasicCredentialsMethod);
				UseApiKey = _selectedAuthenticationMethod.Equals(ApiKeyMethod);
				UseAuth0SSO = _selectedAuthenticationMethod.Equals(Auth0SSOMethod);
				_translationOptions.UseBasicAuthentication = UseBasicCredentials;
				_translationOptions.UseApiKey = UseApiKey;
				_translationOptions.UseAuth0SSO = UseAuth0SSO;
			}
		}

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		public bool UriIsValid()
		{
			Host ??= string.Empty;
			Port ??= string.Empty;
			Host = Host.Trim();
			Port = Port.Trim();
			if (string.IsNullOrEmpty(Host))
			{
				ErrorHandler.HandleError("The Host field can not be empty.", "Host");
				return false;
			}

			if (string.IsNullOrEmpty(Port))
			{
				ErrorHandler.HandleError("The Port field can not be empty.", "Port");
				return false;
			}

			if (!int.TryParse(Port, out var portValue))
			{
				ErrorHandler.HandleError("The Port field is not valid", "Port");
				return false;
			}

			Host = Host.Replace("https://", string.Empty).Replace("http://", string.Empty);
			while (Host.EndsWith("/"))
			{
				Host = Host.Substring(0, Host.Length - 1);
			}

			var protocol = RequiresSecureProtocol ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
			var baseUrl = $"{protocol}://{Host}:{Port}";
			var targetUri = new Uri(baseUrl);
			if (targetUri is null)
			{
				ErrorHandler.HandleError("The URI couldn't be set", "URI");
				return false;
			}

			try
			{
				using var httpClient = new HttpClient();
				var httpResponse = httpClient.PostAsync(targetUri, null).Result;
			}
			catch
			{
				ErrorHandler.HandleError("Couldn't connect with the provided host, port and credentials", "Connection failed");
				return false;
			}

			_translationOptions.Host = Host;
			_translationOptions.Port = portValue;
			return true;
		}

		public bool CredentialsAreValid(bool isSettingsView = false)
		{
			try
			{
				if (UseBasicCredentials)
				{
					return BasicCredentialsAreSet(isSettingsView);
				}
				else if (UseApiKey)
				{
					return ApiKeyIsValid(isSettingsView);
				}
				else if (UseAuth0SSO)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				ErrorHandler.HandleError(ex);
			}

			return false;
		}

		private bool BasicCredentialsAreSet(bool isSettingsView)
		{
			UserName ??= string.Empty;
			UserName = UserName.Trim();
			if (string.IsNullOrEmpty(UserName))
			{
				if (isSettingsView)
				{
					ErrorHandler.HandleError("The connection has been lost, please reconnect", "Connection Lost");
					return false;
				}
				
				ErrorHandler.HandleError("The UserName field can not be empty", "UserName");
				return false;
			}

			Password ??= string.Empty;
			Password = Password.Trim();
			if (string.IsNullOrEmpty(Password))
			{
				if (isSettingsView)
				{
					ErrorHandler.HandleError("The connection has been lost, please reconnect", "Connection Lost");
					return false;
				}

				ErrorHandler.HandleError("The Password field can not be empty", "Password");
				return false;
			}

			return true;
		}

		private bool ApiKeyIsValid(bool isSettingsView)
		{
			ApiKey ??= string.Empty;
			ApiKey = ApiKey.Trim();
			if (string.IsNullOrEmpty(ApiKey))
			{
				if (isSettingsView)
                {
                    ErrorHandler.HandleError("The connection has been lost, please reconnect", "Connection Lost");
                    return false;
                }
          
				ErrorHandler.HandleError("The ApiKey field can not be empty", "ApiKey");
				return false;
			}

			return true;
		}

		private void InitializeView()
		{
			AuthenticationMethods = new() { ApiKeyMethod, BasicCredentialsMethod, Auth0SSOMethod };
			if (_translationOptions.UseApiKey)
			{
				SelectedAuthenticationMethod = ApiKeyMethod;
			}
			else if (_translationOptions.UseAuth0SSO)
			{
				SelectedAuthenticationMethod = Auth0SSOMethod;
			}
			else
			{
				SelectedAuthenticationMethod = BasicCredentialsMethod;
			}
		}

		private void Clear(object parameter)
		{
			if (parameter is not string parameterString)
			{
				return;
			}

			switch (parameterString)
			{
				case "UserName":
					UserName = string.Empty;
					break;

				case "Password":
					Password = string.Empty;
					break;
			}
		}
	}
}