using System.Windows.Input;
using Sdl.Community.GSVersionFetch.Commands;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class LoginViewModel: ProjectWizardViewModelBase
	{
		private bool _isValid;
		private string _url;
		private ICommand _clearCommand;

		public LoginViewModel(object view): base(view)
		{
			_isValid = true;
			//_url = "test";
			ClearCommand = new DelegateCommand(ExecuteClearCommand, CanExecuteClearCommand);

		}

		public DelegateCommand ClearCommand { get; }

		private bool CanExecuteClearCommand()
		{
			return true;
		}
		//public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new CommandHandler(ClearTextBox,true));

		private void ClearTextBox()
		{
			
		}


		private void ExecuteClearCommand()
		{
			Url = string.Empty;
			OnPropertyChanged(nameof(Url));
		}

		public override string DisplayName => "Login";
		public bool ClearButtonVisible => _url.Length > 0;
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
				OnPropertyChanged("ClearButtonVisible");
			}
		}
	}
}
