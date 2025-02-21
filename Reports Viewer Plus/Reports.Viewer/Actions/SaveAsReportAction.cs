using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Reports.Viewer.Plus.Actions
{
	[Action("ReportsViewer_SaveAsReport_Action",
		Name = "ReportsViewer_SaveAsReport_Name",
		Description = "ReportsViewer_SaveAsReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Save"
	)]
	[ActionLayout(typeof(ReportsViewerReportGroups), 6, DisplayType.Normal)]
	public class SaveAsReportAction : BaseReportAction
	{
		private ReportsViewerController _reportsViewerController;
		private bool _canEnable;
		private bool _isLoading;

		protected override void Execute()
		{
			_reportsViewerController.SaveReport();
		}

		public override void UpdateEnabled(bool loading)
		{
			_isLoading = loading;
			SetEnabled();
		}

		public override void Initialize()
		{
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();
			_reportsViewerController.ReportSelectionChanged += ReportsViewerController_ReportSelectionChanged;

			SetEnabled();
		}

		public void Run()
		{
			Execute();
		}

		private void ReportsViewerController_ReportSelectionChanged(object sender, CustomEventArgs.ReportSelectionChangedEventArgs e)
		{
			_canEnable = e.SelectedReports?.Count > 0;
			SetEnabled();
		}

		private void SetEnabled()
		{
			Enabled = !_isLoading && _canEnable;
		}
	}
}
