using System.Windows.Forms;
using Sdl.Community.XLIFF.Manager.View;
using Sdl.Community.XLIFF.Manager.ViewModel;

namespace Sdl.Community.XLIFF.Manager.Controls
{
	public partial class ProjectsNavigationViewControl : UserControl
	{		
		public ProjectsNavigationViewControl(ProjectsNavigationViewModel viewModel)
		{
			InitializeComponent();

			DisposeViewModel();			
			BindViewModel(viewModel);
		}

		private void BindViewModel(ProjectsNavigationViewModel viewModel)
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
