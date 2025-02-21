using System.Windows.Forms;
using Sdl.Community.XLIFF.Manager.View;
using Sdl.Community.XLIFF.Manager.ViewModel;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.XLIFF.Manager.Controls
{
	public partial class ProjectFilesViewControl : UserControl, IUIControl
	{		
		public ProjectFilesViewControl(ProjectFilesViewModel viewModel)
		{
			InitializeComponent();			
			UpdateViewModel(viewModel);
		}

		internal void UpdateViewModel(ProjectFilesViewModel viewModel)
		{
			DisposeViewModel();
			AssignView(viewModel);
		}

		private void AssignView(ProjectFilesViewModel viewModel)
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
