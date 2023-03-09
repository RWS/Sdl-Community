using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.MTEdge.Provider.Command;
using Sdl.Community.MTEdge.Provider.Helpers;
using Sdl.Community.MTEdge.Provider.Interface;
using Sdl.Community.MTEdge.Provider.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTEdge.Provider.ViewModel
{
	public class CredentialsViewModel : BaseModel, ICredentialsViewModel
	{
		private const string BasicCredentialsMethod = "Basic credentials";
		private const string ApiKeyMethod = "API Key";
		private const string Auth0SSOMethod = "SSO";

		private readonly ITranslationOptions _translationOptions;
		private LanguagePair[] _languagePairs;

		private string _port;
		private string _host;
		private string _apiKey;
		private string _username;
		private string _password;
		private List<string> _autheticationMethods;
		private string _selectedAuthenticationMethod;

		private bool _persistsHost;
		private bool _requiresSecureProtocol;
		private bool _persistsCredentials;
		private bool _useBasicCredentials;
		private bool _useApiKey;
		private bool _useAuth0SSO;

		public CredentialsViewModel(ITranslationOptions options, LanguagePair[] languagePairs)
		{
			ViewModel = this;
			_translationOptions = options;
			_languagePairs = languagePairs;
			RequiresSecureProtocol = true;
			InitializeView();
		}

		public BaseModel ViewModel { get; set; }

		public string Port
		{
			get => _port;
			set
			{
				if (_port == value) return;
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
				_translationOptions.Host = value.Replace("https://", string.Empty).Replace("http://", string.Empty);
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
			}
		}

		private void InitializeView()
		{
			AuthenticationMethods = new() { ApiKeyMethod, BasicCredentialsMethod, Auth0SSOMethod };
			SelectedAuthenticationMethod = _autheticationMethods.First(x => x.Equals(BasicCredentialsMethod));
		}
	}
}