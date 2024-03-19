using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MicrosoftTranslatorProvider.ViewModel;

namespace MicrosoftTranslatorProvider.View
{
	/// <summary>
	/// Interaction logic for MicrosoftView.xaml
	/// </summary>
	public partial class MicrosoftAuthenticationView : UserControl
    {
		MicrosoftAuthenticationViewModel _microsoftAuthenticationViewModel;

		public MicrosoftAuthenticationView()
        {
            InitializeComponent();
        }

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is not MicrosoftAuthenticationViewModel viewModel)
			{
				return;
			}

			_microsoftAuthenticationViewModel = viewModel;
		}

		private void UserPassword_Changed(object sender, RoutedEventArgs e)
		{
			if (_microsoftAuthenticationViewModel is not null && sender is PasswordBox passwordBox)
			{
				_microsoftAuthenticationViewModel.ApiKey = passwordBox.Password;
			}
		}

		private new void KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				_microsoftAuthenticationViewModel.SignInCommand.Execute(null);
			}
		}
	}
}