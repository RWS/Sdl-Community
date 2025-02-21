using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for MainExplorerControl.xaml
	/// </summary>
	public partial class MainExplorerView
	{
		public MainExplorerView(MainViewModel model)
		{
			InitializeComponent();
			DataContext = model;
		}
	}
}
