namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class LoginViewModel: ProjectWizardViewModelBase
	{
		private bool _isValid;

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
	}
}
