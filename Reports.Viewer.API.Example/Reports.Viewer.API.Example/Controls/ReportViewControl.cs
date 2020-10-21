using System.Windows.Forms;
using Sdl.Community.Reports.Viewer.API.Example.View;
using Sdl.Community.Reports.Viewer.API.Example.ViewModel;

namespace Sdl.Community.Reports.Viewer.API.Example.Controls
{
	public partial class ReportViewControl : UserControl
	{
		public ReportViewControl()
		{
			InitializeComponent();
		}

		public void UpdateViewModel(DataView view)
		{
			DisposeViewModel();
			AssignView(view);
		}

		private void AssignView(DataView view)
		{						
			elementHost1.Child = view;
		}

		private void DisposeViewModel()
		{
			if (elementHost1.Child != null && elementHost1.Child is DataView view)
			{
				if (view.DataContext is DataViewModel model)
				{
					model.Dispose();
				}
			}
		}
	}
}
