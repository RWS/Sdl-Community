using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Trados.Transcreate.Common;

namespace Trados.Transcreate.Actions
{
	[Action("TranscreateManager_Refresh_Action",
		Name = "TranscreateManager_Refresh_Name",
		Description = "TranscreateManager_Refresh_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "Refresh"
	)]
	[ActionLayout(typeof(TranscreateManagerViewGroup), 2, DisplayType.Large)]
	public class RefreshAction : AbstractViewControllerAction<TranscreateViewController>
	{
		private PathInfo _pathInfo;
		private TranscreateViewController _controller;

		protected override void Execute()
		{
			_controller.RefreshProjects(true);
		}

		public override void Initialize()
		{
			Enabled = true;

			_controller = SdlTradosStudio.Application.GetController<TranscreateViewController>();
			_pathInfo = new PathInfo();
		}
	}
}
