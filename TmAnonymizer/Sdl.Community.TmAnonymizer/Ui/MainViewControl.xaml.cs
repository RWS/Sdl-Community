using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.Ui
{
	/// <summary>
	/// Interaction logic for MainViewControl.xaml
	/// </summary>
	public partial class MainViewControl 
	{
		public MainViewControl()
		{
			InitializeComponent();
			DataContext = new MainViewModel();
		}
	}
}
