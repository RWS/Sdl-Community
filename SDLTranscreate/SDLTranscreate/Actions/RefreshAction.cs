using System;
using Sdl.Community.Transcreate.Common;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Transcreate.Actions
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
			_controller.RefreshProjects();
		}

		public override void Initialize()
		{
			Enabled = true;

			_controller = SdlTradosStudio.Application.GetController<TranscreateViewController>();
			_pathInfo = new PathInfo();
		}
	}
}
