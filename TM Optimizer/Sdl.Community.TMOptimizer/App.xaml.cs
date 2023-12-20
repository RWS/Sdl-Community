using System.Windows;

namespace Sdl.Community.TMOptimizer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // First assign the MainWindow to the main form
            var viewModel = new TMCleanerViewModel();
            var window = new MainWindow();
            window.DataContext = viewModel;
            Application.Current.MainWindow = window; // main window
            window.Show();
        }
    }
}