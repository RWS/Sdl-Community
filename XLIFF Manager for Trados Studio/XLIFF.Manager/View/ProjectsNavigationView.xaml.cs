using System.Windows;
using System.Windows.Controls;
using Sdl.Community.XLIFF.Manager.ViewModel;

namespace Sdl.Community.XLIFF.Manager.View
{
	/// <summary>
	/// Interaction logic for ProjectsNavigationView.xaml
	/// </summary>
	public partial class ProjectsNavigationView : UserControl
	{
		private readonly ProjectsNavigationViewModel _viewModel;

		public ProjectsNavigationView(ProjectsNavigationViewModel viewModel)
		{
			InitializeComponent();

			_viewModel = viewModel;
			Loaded += ProjectsNavigationView_Loaded;
		}

		/// <summary>
		/// We need to wait for the view to be fully loaded before binding the view model
		/// Ensure Loaded is called only once; unsubscribe to the Loaded event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProjectsNavigationView_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= ProjectsNavigationView_Loaded;
			DataContext = _viewModel;
		}

	
	}
}
