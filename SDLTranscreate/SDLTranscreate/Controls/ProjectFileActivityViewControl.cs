using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Trados.Transcreate.View;
using Trados.Transcreate.ViewModel;

namespace Trados.Transcreate.Controls
{
	public partial class ProjectFileActivityViewControl : UserControl, IUIControl
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
