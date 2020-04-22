using System.Windows.Forms;
using Sdl.Community.XLIFF.Manager.View;
using Sdl.Community.XLIFF.Manager.ViewModel;

namespace Sdl.Community.XLIFF.Manager.Controls
{
	public partial class ProjectFilesViewControl : UserControl
	{		
		public ProjectFilesViewControl(ProjectFilesViewModel viewModel)
		{
			InitializeComponent();

			DisposeViewModel();
			BindViewModel(viewModel);
		}

		private void BindViewModel(ProjectFilesViewModel viewModel)
		{
			elementHost1.Child = new ProjectFilesView(viewModel);
		}

		private void DisposeViewModel()
		{
			if (elementHost1.Child != null && elementHost1.Child is ProjectFilesView view)
			{
				if (view.DataContext is ProjectFilesViewModel model)
				{
					model.Dispose();
				}
			}
		}
	}
}
