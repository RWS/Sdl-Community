using System.Windows;
using System.Windows.Controls;
using Sdl.Community.XLIFF.Manager.ViewModel;

namespace Sdl.Community.XLIFF.Manager.View
{
	/// <summary>
	/// Interaction logic for ProjectFileActivitiesView.xaml
	/// </summary>
	public partial class ProjectFileActivityView : UserControl
	{	
		private readonly ProjectFileActivityViewModel _viewModel;

		public ProjectFileActivityView(ProjectFileActivityViewModel viewModel)
		{
			InitializeComponent();

			_viewModel = viewModel;
			Loaded += ProjectFileActivityView_Loaded;
		}

		/// <summary>
		/// We need to wait for the view to be fully loaded before binding the view model
		/// Ensure Loaded is called only once; unsubscribe to the Loaded event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProjectFileActivityView_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= ProjectFileActivityView_Loaded;
			DataContext = _viewModel;
		}
	}
}
