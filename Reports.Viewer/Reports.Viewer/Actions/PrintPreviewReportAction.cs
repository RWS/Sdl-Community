namespace Sdl.Community.Reports.Viewer.Actions
{
	//[Action("ReportsViewer_PrintPreviewReport_Action",
	//	Name = "ReportsViewer_PrintPreviewReport_Name",
	//	Description = "ReportsViewer_PrintPreviewReport_Description",
	//	ContextByType = typeof(ReportsViewerController),
	//	Icon = "PrintPreview"
	//)]
	//[ActionLayout(typeof(ReportsViewerReportGroups), 4, DisplayType.Normal)]
	//public class PrintPreviewReportAction : BaseReportAction
	//{
	//	private ReportsViewerController _reportsViewerController;
	//	private bool _canEnable;
	//	private bool _isLoading;

	//	protected override void Execute()
	//	{
	//		_reportsViewerController.ShowPrintPreviewDialog();
	//	}

	//	public override void UpdateEnabled(bool loading)
	//	{
	//		_isLoading = loading;
	//		SetEnabled();
	//	}

	//	public override void Initialize()
	//	{
	//		_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();
	//		_reportsViewerController.ReportSelectionChanged += ReportsViewerController_ReportSelectionChanged;

	//		SetEnabled();
	//	}

	//	public void Run()
	//	{
	//		Execute();
	//	}

	//	private void ReportsViewerController_ReportSelectionChanged(object sender, CustomEventArgs.ReportSelectionChangedEventArgs e)
	//	{
	//		_canEnable = e.SelectedReports?.Count == 1;
	//		SetEnabled();
	//	}

	//	private void SetEnabled()
	//	{
	//		Enabled = false;
	//		//Enabled = !_isLoading && _canEnable;
	//	}
	//}
}
