using System.Windows.Forms;
using Sdl.Community.Reports.Viewer.View;
using Sdl.Community.Reports.Viewer.ViewModel;

namespace Sdl.Community.Reports.Viewer.Controls
{
	public partial class ReportViewControl : UserControl
	{
		public ReportViewControl()
		{
			InitializeComponent();
		}

		public void UpdateViewModel(ReportView view)
		{
			DisposeViewModel();
			AssignView(view);
		}

		private void AssignView(ReportView view)
		{						
			elementHost1.Child = view;
		}

		private void DisposeViewModel()
		{
			if (elementHost1.Child != null && elementHost1.Child is ReportView view)
			{
				if (view.DataContext is ReportViewModel model)
				{
					model.Dispose();
				}
			}
		}
	}
}
