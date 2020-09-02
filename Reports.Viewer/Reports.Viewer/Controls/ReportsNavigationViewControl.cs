using System.Windows.Forms;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Community.Reports.Viewer.ViewModel;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.Reports.Viewer.Controls
{
	public partial class ReportsNavigationViewControl : UserControl, IUIControl
	{
		public ReportsNavigationViewControl()
		{
			InitializeComponent();
		}

		public void UpdateViewModel(ReportsNavigationView view)
		{

			DisposeViewModel();
			AssignView(view);
		}

		private void AssignView(ReportsNavigationView view)
		{
			elementHost1.Child = view;
		}

		private void DisposeViewModel()
		{
			if (elementHost1.Child != null && elementHost1.Child is ReportsNavigationView view)
			{
				if (view.DataContext is ReportsNavigationViewModel model)
				{
					model.Dispose();
				}
			}
		}
	}
}
