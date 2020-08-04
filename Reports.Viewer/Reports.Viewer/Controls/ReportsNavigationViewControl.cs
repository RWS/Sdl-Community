using System.Windows.Forms;
using System.Windows.Threading;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Community.Reports.Viewer.ViewModel;

namespace Sdl.Community.Reports.Viewer.Controls
{
	public partial class ReportsNavigationViewControl : UserControl
	{
		public ReportsNavigationViewControl()
		{
			InitializeComponent();
		}

		public void UpdateViewModel(ReportsNavigationView view)
		{

			//DisposeViewModel();
			AssignView(view);
		}

		private void AssignView(ReportsNavigationView view)
		{
			elementHost1.Child = view;
		}

		//private void DisposeViewModel()
		//{
		//	if (elementHost1.Child != null && elementHost1.Child is ReportsNavigationView view)
		//	{
		//		if (view.DataContext is ReportsNavigationViewModel model)
		//		{
		//			model.Dispose();
		//		}
		//	}
		//}
	}
}
