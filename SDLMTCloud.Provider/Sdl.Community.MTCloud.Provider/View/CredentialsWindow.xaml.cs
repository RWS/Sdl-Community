using System.Windows;
using System.Windows.Controls;
using Sdl.Community.MTCloud.Provider.UiHelpers.Watermark;
using Sdl.Community.MTCloud.Provider.ViewModel;

namespace Sdl.Community.MTCloud.Provider.View
{
	/// <summary>
	/// Interaction logic for Credentials.xaml
	/// </summary>
	public partial class CredentialsWindow : Window
	{
		public CredentialsWindow()
		{
			InitializeComponent();
			WatermarkHandler.Handle(UserNameBox);
			WatermarkHandler.Handle(UserPasswordBox);
		}

		private void CenterWindowOnScreen(double width, double height)
		{
			var screenWidth = SystemParameters.PrimaryScreenWidth;
			var screenHeight = SystemParameters.PrimaryScreenHeight;
			Left = (screenWidth / 2) - width / 2;
			Top = (screenHeight / 2) - 0.5 * height;
		}

		private void UserPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (DataContext != null)
			{ ((CredentialsViewModel)DataContext).UserPassword = ((PasswordBox)sender).Password; }
		}

		private void ClientIdBox_PasswordChanged(object sender, RoutedEventArgs e) 
		{
			if (DataContext != null)
			{ ((CredentialsViewModel)DataContext).ClientId = ((PasswordBox)sender).Password; }
		}

		private void ClientSecretBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (DataContext != null)
			{ ((CredentialsViewModel)DataContext).ClientSecret = ((PasswordBox)sender).Password; }
		}

		private void AuthControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (((CredentialsWindow)sender).IsVisible)
				CenterWindowOnScreen(e.NewSize.Width, e.NewSize.Height);
		}
	}
}
