using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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


		private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			var treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
			if (treeViewItem == null)
			{
				return;
			}

			treeViewItem.IsSelected = true;
			e.Handled = true;
		}

		private static TreeViewItem VisualUpwardSearch(DependencyObject source)
		{
			while (source != null && !(source is TreeViewItem))
			{
				source = VisualTreeHelper.GetParent(source);
			}

			return source as TreeViewItem;
		}

		private void TreeView_OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (sender is TreeView treeView && _viewModel.ReportGroups.Count > 0)
			{
				var selectedItem = treeView.SelectedItem;
				if (e.Key == Key.Up)
				{
					if (selectedItem == _viewModel.ReportGroups[0])
					{
						e.Handled = true;
					}
				}
				else if (e.Key == Key.Down)
				{
					var reprotGroup = _viewModel.ReportGroups[_viewModel.ReportGroups.Count - 1];
					var reportItem = reprotGroup?.GroupItems?[reprotGroup.GroupItems.Count - 1];
					var report = reportItem?.Reports?[reportItem.Reports.Count - 1];

					if (selectedItem == report)
					{
						e.Handled = true;
					}
				}
			}
		}
	}
}
