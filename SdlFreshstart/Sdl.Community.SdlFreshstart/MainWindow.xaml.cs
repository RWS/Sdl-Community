using MahApps.Metro.Controls.Dialogs;
using Sdl.Community.SdlFreshstart.ViewModel;

namespace Sdl.Community.SdlFreshstart
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow 
	{
		public MainWindow()
		{
			InitializeComponent();
			
			var viewModel = new MainWindowViewModel(this, DialogCoordinator.Instance);
			DataContext = viewModel;
		}
	}
}
