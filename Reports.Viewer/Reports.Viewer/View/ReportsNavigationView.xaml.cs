using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Reports.Viewer.Plus.ViewModel;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Reports.Viewer.Plus.View
{
	/// <summary>
	/// Interaction logic for ProjectsNavigationView.xaml
	/// </summary>
	public partial class ReportsNavigationView : UserControl, IUIControl
	{
		public ReportsNavigationView()
		{
			InitializeComponent();
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
			var viewModel = DataContext as ReportsNavigationViewModel;
			if (viewModel == null)
			{
				return;
			}

			if (sender is TreeView treeView && viewModel.ReportGroups.Count > 0)
			{
				var selectedItem = treeView.SelectedItem;
				if (e.Key == Key.Up)
				{
					if (selectedItem == viewModel.ReportGroups[0])
					{
						e.Handled = true;
					}
				}
				else if (e.Key == Key.Down)
				{
					var reprotGroup = viewModel.ReportGroups[viewModel.ReportGroups.Count - 1];
					var reportItem = reprotGroup?.GroupItems?[reprotGroup.GroupItems.Count - 1];
					var report = reportItem?.Reports?[reportItem.Reports.Count - 1];

					if (selectedItem == report)
					{
						e.Handled = true;
					}
				}
			}
		}

		public void Dispose()
		{
		}
	}
}
