using Sdl.Community.HunspellDictionaryManager.ViewModel;

namespace Sdl.Community.HunspellDictionaryManager.Ui
{
	/// <summary>
	/// Interaction logic for MainViewControl.xaml
	/// </summary>
	public partial class MainViewControl
	{
		public MainViewControl()
		{
			InitializeComponent();
			DataContext = new MainViewControlViewModel();
		}		
	}
}