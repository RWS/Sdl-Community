using System.Windows.Forms;
using Sdl.Community.Transcreate.View;
using Sdl.Community.Transcreate.ViewModel;

namespace Sdl.Community.Transcreate.Controls
{
	public partial class ProjectPropertiesViewControl : UserControl
	{
		public ProjectPropertiesViewControl(ProjectPropertiesViewModel viewModel)
		{
			InitializeComponent();
			UpdateViewModel(viewModel);
		}

		internal void UpdateViewModel(ProjectPropertiesViewModel viewModel)
		{
			DisposeViewModel();
			AssignView(viewModel);
		}

		private void AssignView(ProjectPropertiesViewModel viewModel)
		{
			elementHost.Child = new ProjectPropertiesView(viewModel);
		}

		private void DisposeViewModel()
		{
			if (elementHost.Child != null && elementHost.Child is ProjectPropertiesView view)
			{
				if (view.DataContext is ProjectPropertiesViewModel model)
				{
					model.Dispose();
				}
			}
		}
	}
}
