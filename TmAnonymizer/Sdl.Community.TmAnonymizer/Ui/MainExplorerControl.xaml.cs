using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.Ui
{
	/// <summary>
	/// Interaction logic for MainExplorerControl.xaml
	/// </summary>
	public partial class MainExplorerControl
	{
		public MainExplorerControl(MainViewModel model)
		{
			InitializeComponent();
			DataContext = model;
		}

		//private void ParentGrid_OnPreviewKeyUp(object sender, KeyEventArgs e)
		//{
		//	if (e.Key == Key.Down)
		//	{
		//		ScrollViewer.LineDown();
		//	}
		//	if (e.Key == Key.Up)
		//	{
		//		ScrollViewer.LineUp();
		//	}
		//}
	}
}
