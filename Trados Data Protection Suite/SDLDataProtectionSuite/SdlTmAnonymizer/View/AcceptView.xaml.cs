using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for AcceptWindow.xaml
	/// </summary>
	public partial class AcceptView 
	{
		public AcceptView(SettingsService settingsService)
		{
			InitializeComponent();

			DataContext = new AcceptViewModel(settingsService);
		}
	}
}
