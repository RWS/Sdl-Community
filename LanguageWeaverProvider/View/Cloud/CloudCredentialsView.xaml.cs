using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LanguageWeaverProvider.ViewModel.Cloud;

namespace LanguageWeaverProvider.View.Cloud
{
	/// <summary>
	/// Interaction logic for CloudMainView.xaml
	/// </summary>
	public partial class CloudCredentialsView : UserControl
	{
		private CloudCredentialsViewModel _cloudCredentialsViewModel;

		public CloudCredentialsView()
		{
			InitializeComponent();
		}

		private void ViewLoaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is not CloudCredentialsViewModel viewModel)
			{
				return;
			}

			_cloudCredentialsViewModel = viewModel;
			if (_cloudCredentialsViewModel.TranslationOptions is null)
			{
				return;
			}

			userPwBox.Password = _cloudCredentialsViewModel.UserPassword;
			clientSecretBox.Password = _cloudCredentialsViewModel.ClientSecret;
		}

		private new void KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				_cloudCredentialsViewModel.SignInCommand.Execute(null);
			}
		}

		private void UserPassword_Changed(object sender, RoutedEventArgs e)
		{
			if (_cloudCredentialsViewModel is not null && sender is PasswordBox passwordBox)
			{
				_cloudCredentialsViewModel.UserPassword = passwordBox.Password;
			}
        }

		private void ClientSecret_Changed(object sender, RoutedEventArgs e)
		{
			if (_cloudCredentialsViewModel is not null && sender is PasswordBox passwordBox)
			{
				_cloudCredentialsViewModel.ClientSecret = passwordBox.Password;
			}
		}
	}
}