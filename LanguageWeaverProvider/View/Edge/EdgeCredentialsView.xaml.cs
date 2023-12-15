using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LanguageWeaverProvider.ViewModel.Edge;

namespace LanguageWeaverProvider.View.Edge
{
	/// <summary>
	/// Interaction logic for EdgeCredentialsView.xaml
	/// </summary>
	public partial class EdgeCredentialsView : UserControl
	{
		private EdgeCredentialsViewModel _edgeCredentialsViewModel;

		public EdgeCredentialsView()
		{
			InitializeComponent();
		}

		private void ViewLoaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is not EdgeCredentialsViewModel viewModel)
			{
				return;
			}

			_edgeCredentialsViewModel = viewModel;
			if (_edgeCredentialsViewModel.TranslationOptions is null)
			{
				return;
			}

			userPwBox.Password = _edgeCredentialsViewModel.Password;
			apiKeyBox.Password = _edgeCredentialsViewModel.ApiKey;
		}

		private void KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				_edgeCredentialsViewModel.SignInCommand.Execute(null);
			}
		}

		private void UserPassword_Changed(object sender, RoutedEventArgs e)
		{
			if (_edgeCredentialsViewModel is not null)
			{
				_edgeCredentialsViewModel.Password = (sender as PasswordBox).Password;
			}
		}

		private void ApiKey_Changed(object sender, RoutedEventArgs e)
		{
			if (_edgeCredentialsViewModel is not null)
			{
				_edgeCredentialsViewModel.ApiKey = (sender as PasswordBox).Password;
			}
		}
    }
}