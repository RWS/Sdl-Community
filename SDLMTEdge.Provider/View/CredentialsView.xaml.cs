using System.Windows;
using System.Windows.Controls;
using Sdl.Community.MTEdge.Provider.ViewModel;

namespace Sdl.Community.MTEdge.Provider.View
{
	/// <summary>
	/// Interaction logic for CredentialsView.xaml
	/// </summary>
	public partial class CredentialsView : UserControl
	{
		public CredentialsView()
		{
			InitializeComponent();
		}

		private void UserPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (DataContext is null
			 || DataContext is not CredentialsViewModel viewModel)
			{
				return;
			}

			viewModel.Password = (sender as PasswordBox).Password;
		}

		private void ApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (DataContext is null
			 || DataContext is not CredentialsViewModel viewModel)
			{
				return;
			}

			viewModel.ApiKey = (sender as PasswordBox).Password;
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (DataContext is null
			 || DataContext is not CredentialsViewModel viewModel)
			{
				return;
			}

			ApiBox.Password = viewModel.ApiKey;
			PasswordBox.Password = viewModel.Password;
		}
	}
}