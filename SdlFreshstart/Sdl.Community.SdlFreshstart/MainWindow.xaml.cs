using NLog;
using Sdl.Community.SdlFreshstart.Helpers;
using Sdl.Community.SdlFreshstart.ViewModel;

namespace Sdl.Community.SdlFreshstart
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow 
	{
		private readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public MainWindow()
		{
			InitializeComponent();
			Log.Setup();
			Logger.Info("Log initialized");
			var viewModel = new MainWindowViewModel(this);
			DataContext = viewModel;
		}
	}
}
