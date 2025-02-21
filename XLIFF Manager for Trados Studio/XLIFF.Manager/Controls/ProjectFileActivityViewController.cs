using System.Windows.Forms;
using Sdl.Community.XLIFF.Manager.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.XLIFF.Manager.Controls
{
	[ViewPart(
		Id = "XLIFFManager_ProjectFileActivity_ViewPart",
		Name = "XLIFFManager_ProjectFileActivity_Name",
		Description = "XLIFFManager_ProjectFileActivity_Description"
	)]
	[ViewPartLayout(Dock = DockType.Bottom, Pinned = false, Height = 200, ZIndex = 0, LocationByType = typeof(XLIFFManagerViewController))]
	public class ProjectFileActivityViewController : AbstractViewPartController
	{		
		private ProjectFileActivityViewControl _control;

		private ProjectFileActivityViewModel _viewModel;

		protected override void Initialize()
		{			
			_control = new ProjectFileActivityViewControl(ViewModel);
		}

		protected override IUIControl GetContentControl()
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
