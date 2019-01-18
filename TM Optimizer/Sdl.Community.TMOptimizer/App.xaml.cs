using System.Windows;
using Studio.AssemblyResolver;

namespace Sdl.Community.TMOptimizer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AssemblyResolver.Resolve();
            // First assign the MainWindow to the main form
            TMCleanerViewModel viewModel = new TMCleanerViewModel();
            MainWindow window = new MainWindow();
            window.DataContext = viewModel;
            Application.Current.MainWindow = window; // main win
            window.Show();
        }
    }
}