using System.Windows.Forms;
using Sdl.Community.Transcreate.View;
using Sdl.Community.Transcreate.ViewModel;

namespace Sdl.Community.Transcreate.Controls
{
	public partial class ProjectFileActivityViewControl : UserControl
	{
		public ProjectFileActivityViewControl(ProjectFileActivityViewModel viewModel)
		{
			InitializeComponent();
			UpdateViewModel(viewModel);
		}

		internal void UpdateViewModel(ProjectFileActivityViewModel viewModel)
		{
			DisposeViewModel();
			AssignView(viewModel);
		}

		private void AssignView(ProjectFileActivityViewModel viewModel)
		{
			elementHost.Child = new ProjectFileActivityView(viewModel);
		}

		private void DisposeViewModel()
		{
			if (elementHost.Child != null && elementHost.Child is ProjectFileActivityView view)
			{
				if (view.DataContext is ProjectFileActivityViewModel model)
				{
					model.Dispose();
				}
			}
		}
	}
}
