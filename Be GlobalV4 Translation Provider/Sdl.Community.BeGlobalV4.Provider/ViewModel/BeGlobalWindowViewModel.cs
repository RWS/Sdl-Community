using System.ComponentModel;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Community.BeGlobalV4.Provider.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.ViewModel
{
	public class BeGlobalWindowViewModel : BaseViewModel
	{
		public BeGlobalTranslationOptions Options { get; set; }
		public LoginViewModel LoginViewModel { get; set; }
		public SettingsViewModel SettingsViewModel { get; set; }

		//public BeGlobalWindowViewModel(BeGlobalTranslationOptions options, TranslationProviderCredential credentialStore)
		//{
		//	Options = options;
		//}

		public BeGlobalWindowViewModel(BeGlobalWindow mainWindow, BeGlobalTranslationOptions options, TranslationProviderCredential credentialStore)
		{
			LoginViewModel = new LoginViewModel(mainWindow);
			SettingsViewModel = new SettingsViewModel(mainWindow);
			Options = options;
		}
	}
}
