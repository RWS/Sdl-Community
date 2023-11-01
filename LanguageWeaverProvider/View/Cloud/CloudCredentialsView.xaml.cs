using System.Windows;
using System.Windows.Controls;
using LanguageWeaverProvider.ViewModel.Cloud;

namespace LanguageWeaverProvider.View.Cloud
{
	/// <summary>
	/// Interaction logic for CloudMainView.xaml
	/// </summary>
	public partial class CloudCredentialsView : UserControl
	{
		public CloudCredentialsView()
		{
			InitializeComponent();
		}

		private void UserPassword_Changed(object sender, RoutedEventArgs e)
		{
			(DataContext as CloudCredentialsViewModel).UserPassword = (sender as PasswordBox).Password;
        }

		private void ClientSecret_Changed(object sender, RoutedEventArgs e)
		{
			(DataContext as CloudCredentialsViewModel).ClientSecret = (sender as PasswordBox).Password;
		}

		private void ViewLoaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is not CloudCredentialsViewModel dataContext || dataContext.TranslationOptions is null)
			{
				return;
			}

			userPwBox.Password = dataContext.UserPassword;
			clientSecretBox.Password = dataContext.ClientSecret;
		}
    }
}