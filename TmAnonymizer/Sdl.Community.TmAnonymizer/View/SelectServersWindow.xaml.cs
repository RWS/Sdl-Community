using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for AcceptWindow.xaml
	/// </summary>
	public partial class SelectServersWindow
	{
		public SelectServersWindow(SettingsService settingsService, Login login)
		{
			InitializeComponent();

			DataContext = new SelectServersWindowViewModel(settingsService, login);
		}
	}
}
