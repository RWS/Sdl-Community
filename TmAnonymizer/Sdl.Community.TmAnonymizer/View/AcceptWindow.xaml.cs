using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for AcceptWindow.xaml
	/// </summary>
	public partial class AcceptWindow 
	{
		public AcceptWindow(SettingsService settingsService)
		{
			InitializeComponent();

			DataContext = new AcceptWindowViewModel(settingsService);
		}
	}
}
