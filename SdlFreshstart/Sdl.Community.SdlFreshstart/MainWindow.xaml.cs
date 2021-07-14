using Sdl.Community.SdlFreshstart.Helpers;
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
			Log.Setup();
			var viewModel = new MainWindowViewModel(this);
			DataContext = viewModel;
		}
	}
}
