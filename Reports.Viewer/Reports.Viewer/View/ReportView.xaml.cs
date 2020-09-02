using System.Windows.Controls;

namespace Sdl.Community.Reports.Viewer.View
{
	/// <summary>
	/// Interaction logic for ProjectFilesView.xaml
	/// </summary>
	public partial class ReportView : UserControl
	{
		//private readonly ReportViewModel _viewModel;

		public ReportView()
		{
			InitializeComponent();

			//_viewModel = viewModel;
			//Loaded += ReportView_Loaded;
		}

		///// <summary>
		///// We need to wait for the view to be fully loaded before binding the view model
		///// Ensure Loaded is called only once; unsubscribe to the Loaded event
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//private void ReportView_Loaded(object sender, RoutedEventArgs e)
		//{
		//	Loaded -= ReportView_Loaded;
		//	DataContext = _viewModel;
		//}
	}
}
