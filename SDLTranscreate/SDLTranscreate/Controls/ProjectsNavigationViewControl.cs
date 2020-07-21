using System.Windows.Forms;
using Sdl.Community.Transcreate.View;
using Sdl.Community.Transcreate.ViewModel;

namespace Sdl.Community.Transcreate.Controls
{
	public partial class ProjectsNavigationViewControl : UserControl
	{		
		public ProjectsNavigationViewControl(ProjectsNavigationViewModel viewModel)
		{
			InitializeComponent();			
			UpdateViewModel(viewModel);
		}

		internal void UpdateViewModel(ProjectsNavigationViewModel viewModel)
		{
			DisposeViewModel();
			AssignView(viewModel);
		}

		private void AssignView(ProjectsNavigationViewModel viewModel)
		{
			elementHost1.Child = new ProjectsNavigationView(viewModel);
		}

		private void DisposeViewModel()
		{
			if (elementHost1.Child != null && elementHost1.Child is ProjectsNavigationView view)
			{
				if (view.DataContext is ProjectsNavigationViewModel model)
				{
					model.Dispose();
				}
			}
		}
	}
}
