using System.Windows.Controls;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for MainViewControl.xaml
	/// </summary>
	public partial class MainViewControl: UserControl
	{
		public MainViewControl(MainViewModel model)
		{			
			InitializeComponent();
			DataContext = model;
		}	
	}
}
