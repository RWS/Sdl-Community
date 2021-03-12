using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Trados.Transcreate.View;
using Trados.Transcreate.ViewModel;

namespace Trados.Transcreate.Controls
{
	[ViewPart(
		Id = "TranscreateManager_ProjectFileActivity_ViewPart",
		Name = "TranscreateManager_ProjectFileActivity_Name",
		Description = "TranscreateManager_ProjectFileActivity_Description"
	)]
	[ViewPartLayout(Dock = DockType.Bottom, Pinned = false, Height = 200, ZIndex = 1, LocationByType = typeof(TranscreateViewController))]
	public class ProjectFileActivityViewController : AbstractViewPartController
	{		
		private ProjectFileActivityView _control;

		private ProjectFileActivityViewModel _viewModel;

		protected override void Initialize()
		{			
			_control = new ProjectFileActivityView();
		}

		protected override IUIControl GetContentControl()
		{
			return _control;
		}

		public IUIControl Control => _control;

		public ProjectFileActivityViewModel ViewModel
		{
			get => _viewModel;
			set
			{
				_viewModel = value;
				_control.DataContext = _viewModel;
			}
		}
	}
}
