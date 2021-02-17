using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Trados.Transcreate.View;
using Trados.Transcreate.ViewModel;

namespace Trados.Transcreate.Controls
{
	public partial class ProjectsNavigationViewControl : UserControl, IUIControl
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
