using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.View
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
