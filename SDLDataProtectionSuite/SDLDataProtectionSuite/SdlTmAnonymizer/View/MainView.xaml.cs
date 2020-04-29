using System.Windows.Controls;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for MainViewControl.xaml
	/// </summary>
	public partial class MainView: UserControl
	{
		public MainView(MainViewModel model)
		{			
			InitializeComponent();
			DataContext = model;
		}	
	}
}
