using System.Windows.Controls;

namespace Sdl.Community.Reports.Viewer.View
{
	/// <summary>
	/// Interaction logic for ProjectFilesView.xaml
	/// </summary>
	public partial class BrowserView : UserControl
	{	
		//private readonly BrowserViewModel _viewModel;

		public BrowserView()
		{
			InitializeComponent();

			//_viewModel = viewModel;
			//Loaded += BrowserView_Loaded;
		}

		///// <summary>
		///// We need to wait for the view to be fully loaded before binding the view model
		///// Ensure Loaded is called only once; unsubscribe to the Loaded event
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//private void BrowserView_Loaded(object sender, RoutedEventArgs e)
		//{
		//	Loaded -= BrowserView_Loaded;
		//	DataContext = _viewModel;
		//}
	}
}
