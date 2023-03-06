using System.Collections.Generic;
using System.Linq;
using Sdl.Community.MTEdge.Provider.Interface;
using Sdl.Community.MTEdge.Provider.Model;

namespace Sdl.Community.MTEdge.Provider.ViewModel
{
	public class CredentialsViewModel : BaseModel, ICredentialsViewModel
	{
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
		private bool _persistsCredentials;
		private bool _useRwsCredentials;

		public CredentialsViewModel(ITranslationOptions options)
		{
			ViewModel = this;
			_translationOptions = options;
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
				if (_requiresSecureProtocol== value) return;
				_requiresSecureProtocol = value;
				OnPropertyChanged(nameof(RequiresSecureProtocol));
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

		public bool UseRwsCredentials
		{
			get => _useRwsCredentials;
			set
			{
				if (_useRwsCredentials == value) return;
				_useRwsCredentials = value;
				OnPropertyChanged(nameof(UseRwsCredentials));
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
				OnPropertyChanged(nameof(SelectedAuthenticationMethod));
				UseRwsCredentials = SelectedAuthenticationMethod.Contains("RWS");
			}
		}

		private void InitializeView()
		{
			AuthenticationMethods = new() { "RWS Credentials", "API Key" };
			SelectedAuthenticationMethod = _autheticationMethods.First();
			UseRwsCredentials = SelectedAuthenticationMethod.Contains("RWS");
		}
	}
}