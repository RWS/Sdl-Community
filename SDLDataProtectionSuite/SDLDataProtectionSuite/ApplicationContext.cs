using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlDataProtectionSuite
{
	public static class ApplicationContext
	{
		private static SettingsService _settingsService;

		public static SettingsService SettingsService => _settingsService ??= new SettingsService(new PathInfo());

		public static void ShowSettingsWindow()
		{
			var settingsWindow = new SettingsView();
			var settingsViewModel = new SettingsViewModel(settingsWindow, SettingsService);
			settingsWindow.DataContext = settingsViewModel;

			settingsWindow.ShowDialog();
		}
	}
}