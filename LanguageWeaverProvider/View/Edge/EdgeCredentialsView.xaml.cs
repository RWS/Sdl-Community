using System.Windows;
using System.Windows.Controls;
using LanguageWeaverProvider.ViewModel.Edge;

namespace LanguageWeaverProvider.View.Edge
{
	/// <summary>
	/// Interaction logic for EdgeCredentialsView.xaml
	/// </summary>
	public partial class EdgeCredentialsView : UserControl
	{
		public EdgeCredentialsView()
		{
			InitializeComponent();
		}

		private void UserPassword_Changed(object sender, RoutedEventArgs e)
		{
			if (DataContext is not null && DataContext is EdgeCredentialsViewModel viewModel)
			{
				viewModel.Password = (sender as PasswordBox).Password;
			}

		}

		private void ApiKey_Changed(object sender, RoutedEventArgs e)
		{
			if (DataContext is not null && DataContext is EdgeCredentialsViewModel viewModel)
			{
				viewModel.ApiKey = (sender as PasswordBox).Password;
			}
		}

		private void ViewLoaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is not EdgeCredentialsViewModel dataContext || dataContext.TranslationOptions is null)
			{
				return;
			}

			userPwBox.Password = dataContext.Password;
			apiKeyBox.Password = dataContext.ApiKey;
		}
	}
}