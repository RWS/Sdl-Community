using System.Windows.Forms;
using Sdl.Community.Transcreate.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Transcreate.Controls
{
	[ViewPart(
		Id = "TranscreateManager_ProjectFileActivity_ViewPart",
		Name = "TranscreateManager_ProjectFileActivity_Name",
		Description = "TranscreateManager_ProjectFileActivity_Description"
	)]
	[ViewPartLayout(Dock = DockType.Bottom, Pinned = false, Height = 200, ZIndex = 1, LocationByType = typeof(TranscreateViewController))]
	public class ProjectFileActivityViewController : AbstractViewPartController
	{		
		private ProjectFileActivityViewControl _control;

		private ProjectFileActivityViewModel _viewModel;

		protected override void Initialize()
		{			
			_control = new ProjectFileActivityViewControl(ViewModel);
		}

		protected override Control GetContentControl()
		{
			return _control;
		}

		public Control Control => _control;

		public ProjectFileActivityViewModel ViewModel
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
