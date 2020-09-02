using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.API.Example.Actions
{
	[Action("ReportsViewerAPIExample_Refresh_Action",
		Name = "ReportsViewerAPIExample_Refresh_Name",
		Description = "ReportsViewerAPIExample_Refresh_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Refresh"
	)]
	[ActionLayout(typeof(ReportsViewerAPIExampleActionsGroup), 2, DisplayType.Large)]
	public class RefreshAction : AbstractViewControllerAction<ReportsViewerController>
	{
		private ReportsViewerController _controller;		

		protected override void Execute()
		{
			_controller.RefreshView();
		}

		public override void Initialize()
		{
			_controller = SdlTradosStudio.Application.GetController<ReportsViewerController>();
			Enabled = true;
		}

	}
}
