using Sdl.Community.SdlFreshstart.Helpers;
using Sdl.Community.SdlFreshstart.Services;

namespace Sdl.Community.SdlFreshstart.ViewModel
{
	public class MainWindowViewModel
	{
		public MainWindowViewModel(MainWindow mainWindow)
		{
			var versionService = new StudioVersionService();
			var messageService = new MessageService();
			var registryHelper = new RegistryHelper();
			StudioViewModel = new StudioViewModel(mainWindow, versionService, messageService, registryHelper);
			MultiTermViewModel = new MultiTermViewModel(mainWindow, messageService, versionService, registryHelper);
			ReadMeViewModel = new ReadMeViewModel(versionService);
		}

		public MultiTermViewModel MultiTermViewModel { get; set; }
		public ReadMeViewModel ReadMeViewModel { get; set; }
		public StudioViewModel StudioViewModel { get; set; }
	}
}