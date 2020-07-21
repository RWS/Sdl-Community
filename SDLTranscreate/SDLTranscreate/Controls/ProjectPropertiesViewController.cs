using System.Windows.Forms;
using Sdl.Community.Transcreate.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Transcreate.Controls
{
	[ViewPart(
		Id = "TranscreateManager_ProjectProperties_ViewPart",
		Name = "TranscreateManager_ProjectProperties_Name",
		Description = "TranscreateManager_ProjectProperties_Description"
	)]
	[ViewPartLayout(Dock = DockType.Bottom, Pinned = false, Height = 200, ZIndex = 0, LocationByType = typeof(TranscreateViewController))]
	public class ProjectPropertiesViewController : AbstractViewPartController
	{		
		private ProjectPropertiesViewControl _control;

		private ProjectPropertiesViewModel _viewModel;

		protected override void Initialize()
		{			
			_control = new ProjectPropertiesViewControl(ViewModel);
		}

		protected override Control GetContentControl()
		{
			return _control;
		}

		public Control Control => _control;

		public ProjectPropertiesViewModel ViewModel
		{
			get => _viewModel;
			set
			{
				_viewModel = value;
				_control.UpdateViewModel(_viewModel);
			}
		}
	}
}
