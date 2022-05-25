using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Reports.Viewer.Plus.Actions
{
	[Action("ReportsViewer_Refresh_Action",
		Name = "ReportsViewer_Refresh_Name",
		Description = "ReportsViewer_Refresh_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Refresh"
	)]
	[ActionLayout(typeof(ReportsViewerViewGroups), 2, DisplayType.Large)]
	public class RefreshAction : BaseReportAction
	{	
		private ReportsViewerController _controller;
		private bool _canEnable;
		private bool _isLoading;

		protected override void Execute()
		{
			_controller.RefreshView(true);
		}

		public override void UpdateEnabled(bool loading)
		{
			_isLoading = loading;
			SetEnabled();
		}

		public override void Initialize()
		{
			_canEnable = true;
			_controller = SdlTradosStudio.Application.GetController<ReportsViewerController>();

			SetEnabled();
		}

		private void SetEnabled()
		{
			Enabled = !_isLoading && _canEnable;
		}
	}
}
