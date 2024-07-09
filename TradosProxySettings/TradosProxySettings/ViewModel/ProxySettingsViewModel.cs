using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TradosProxySettings.Commands;
using TradosProxySettings.Model;

namespace TradosProxySettings.ViewModel
{
    public class ProxySettingsViewModel : INotifyPropertyChanged
    {
        private string _address;
        private int _port;
        private string _username;
        private string _password;
        private string _domain;
        private bool _useDefaultCredentials;
        private bool _bypassProxyOnLocal;
        private bool _isEnabled;

        private ICommand _saveCommand;

        private readonly Window _window;
        public ProxySettings ProxySettings { get; }


        public ProxySettingsViewModel(Window window, ProxySettings proxySettings)
        {
            _window = window;
            if (proxySettings == null)
            {
                ProxySettings = new ProxySettings();
            }
            else
            {
                ProxySettings = proxySettings.Clone() as ProxySettings;
            }

            Address = ProxySettings.Address;
            Port = ProxySettings.Port;
            Username = ProxySettings.Username;
            Password = ProxySettings.Password;
            Domain = ProxySettings.Domain;
            UseDefaultCredentials = ProxySettings.UseDefaultCredentials;
            BypassProxyOnLocal = ProxySettings.BypassProxyOnLocal;
            IsEnabled = ProxySettings.IsEnabled;
        }

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new CommandHandler(param => SaveSettings(), param => CanSaveSettings());
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
                OnPropertyChanged(nameof(IsEnabledAndDontUseDefaultCredentials));
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
                OnPropertyChanged(nameof(IsValid));
            }
        }
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public bool BypassProxyOnLocal
        {
            get => _bypassProxyOnLocal;
            set
            {
                _bypassProxyOnLocal = value;
                OnPropertyChanged(nameof(BypassProxyOnLocal));
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public bool UseDefaultCredentials
        {
            get => _useDefaultCredentials;
            set
            {
                _useDefaultCredentials = value;
                OnPropertyChanged(nameof(UseDefaultCredentials));
                OnPropertyChanged(nameof(IsEnabledAndDontUseDefaultCredentials));
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(IsValid));
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                OnPropertyChanged(nameof(IsValid));
            }
        }
        public string Domain
        {
            get => _domain;
            set
            {
                _domain = value;
                OnPropertyChanged(nameof(Domain));
                OnPropertyChanged(nameof(IsValid));
            }
        }




        public bool IsValid => CanSaveSettings();


        public bool IsEnabledAndDontUseDefaultCredentials => IsEnabled && !UseDefaultCredentials;


        private void SaveSettings()
        {
            ProxySettings.Address = Address;
            ProxySettings.Port = Port;
            ProxySettings.Username = Username;
            ProxySettings.Password = Password;
            ProxySettings.Domain = Domain;
            ProxySettings.UseDefaultCredentials = UseDefaultCredentials;
            ProxySettings.BypassProxyOnLocal = BypassProxyOnLocal;
            ProxySettings.IsEnabled = IsEnabled;

            _window.Close();

        }

        private bool CanSaveSettings()
        {
            // Validation logic here
            if (IsEnabled)
            {
                var success = !(string.IsNullOrEmpty(Address) || Port <= 0);
                return success;
            }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
