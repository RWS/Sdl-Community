using Sdl.Community.SdlFreshstart.Services;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
	public class MainWindowViewModel
	{
		public MainWindowViewModel(MainWindow mainWindow)
		{
			var versionService = new VersionService();
			StudioViewModel = new StudioViewModel(mainWindow, versionService, new MessageService());
			MultiTermViewModel = new MultiTermViewModel(mainWindow);
			ReadMeViewModel = new ReadMeViewModel(versionService);
		}

		public MultiTermViewModel MultiTermViewModel { get; set; }
		public ReadMeViewModel ReadMeViewModel { get; set; }
		public StudioViewModel StudioViewModel { get; set; }
	}
}