using System.Windows;
using System.Windows.Controls;
using Sdl.Community.Reports.Viewer.ViewModel;

namespace Sdl.Community.Reports.Viewer.View
{
	/// <summary>
	/// Interaction logic for ProjectsNavigationView.xaml
	/// </summary>
	public partial class ReportsNavigationView : UserControl
	{
		private readonly ReportsNavigationViewModel _viewModel;

		public ReportsNavigationView(ReportsNavigationViewModel viewModel)
		{
			InitializeComponent();

			_viewModel = viewModel;
			Loaded += ReportsNavigationView_Loaded;
		}

		/// <summary>
		/// We need to wait for the view to be fully loaded before binding the view model
		/// Ensure Loaded is called only once; unsubscribe to the Loaded event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ReportsNavigationView_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= ReportsNavigationView_Loaded;
			DataContext = _viewModel;
		}
	}
}
