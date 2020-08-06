using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.Actions
{
	[Action("ReportsViewer_Refresh_Action",
		Name = "ReportsViewer_Refresh_Name",
		Description = "ReportsViewer_Refresh_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Refresh"
	)]
	[ActionLayout(typeof(ReportsViewerViewGroups), 2, DisplayType.Large)]
	public class RefreshAction : AbstractViewControllerAction<ReportsViewerController>
	{	
		private ReportsViewerController _controller;

		protected override void Execute()
		{
			_controller.RefreshView();
		}

		public override void Initialize()
		{
			Enabled = false;

			_controller = SdlTradosStudio.Application.GetController<ReportsViewerController>();			
		}
	}
}
