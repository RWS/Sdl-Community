using System.Windows;
using System.Windows.Controls;
using Sdl.Community.XLIFF.Manager.ViewModel;

namespace Sdl.Community.XLIFF.Manager.View
{
	/// <summary>
	/// Interaction logic for ProjectFilesView.xaml
	/// </summary>
	public partial class ProjectFilesView : UserControl
	{
		private readonly ProjectFilesViewModel _viewModel;

		public ProjectFilesView(ProjectFilesViewModel viewModel)
		{
			InitializeComponent();

			_viewModel = viewModel;
			Loaded += ProjectFilesView_Loaded;
		}

		/// <summary>
		/// We need to wait for the view to be fully loaded before binding the view model
		/// Ensure Loaded is called only once; unsubscribe to the Loaded event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProjectFilesView_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= ProjectFilesView_Loaded;
			DataContext = _viewModel;
		}
	}
}
