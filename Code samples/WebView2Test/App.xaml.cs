using System.Configuration;
using System.Windows;
using WebView2Test.Helpers;
using WebView2Test.Model;
using WebView2Test.Service;
using WebView2Test.ViewModel;

namespace WebView2Test
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var authorizationSettings = new AuthorizationSettings
			(
				ConfigurationManager.AppSettings["ClientId"],
				ConfigurationManager.AppSettings["Audience"],
				ConfigurationManager.AppSettings["Auth0Url"]
			);

			var authService = new AuthorizationService(new LoginGeneratorsHelper(), new CredentialRepository(), authorizationSettings);

			var mainWindow = new MainWindow {DataContext = new MainWindowViewModel(authService)};
			mainWindow.Show();
		}
	}
}