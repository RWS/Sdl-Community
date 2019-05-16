using System.Windows.Input;
using Sdl.Community.GSVersionFetch.Commands;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class LoginViewModel: ProjectWizardViewModelBase
	{
		private bool _isValid;
		private string _url;
		private string _userName;
		private string _password;

		public LoginViewModel(object view): base(view)
		{
			_isValid = true;
		}

		public override string DisplayName => "Login";
		public override bool IsValid
		{
			get => _isValid;
			set
			{
				if (_isValid == value)
					return;

				_isValid = value;
				OnPropertyChanged(nameof(IsValid));
			}
		}
		public string Url
		{
			get => _url;
			set
			{
				_url = value;
				OnPropertyChanged(nameof(Url));
			}
		}
		public string UserName
		{
			get => _userName;
			set
			{
				_userName = value;
				OnPropertyChanged(nameof(UserName));
			}
		}
	}
}
