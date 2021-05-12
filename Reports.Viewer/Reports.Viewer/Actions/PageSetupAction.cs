namespace Sdl.Community.Reports.Viewer.Actions
{
	//[Action("ReportsViewer_PageSetup_Action",
	//	Name = "ReportsViewer_PageSetup_Name",
	//	Description = "ReportsViewer_PageSetup_Description",
	//	ContextByType = typeof(ReportsViewerController),
	//	Icon = "PageSetup"
	//)]
	//[ActionLayout(typeof(ReportsViewerReportGroups), 3, DisplayType.Normal)]
	//public class PageSetupAction : BaseReportAction
	//{
	//	private ReportsViewerController _reportsViewerController;
	//	private bool _canEnable;
	//	private bool _isLoading;

	//	protected override void Execute()
	//	{
	//		_reportsViewerController.ShowPageSetupDialog();
	//	}

	//	public override void UpdateEnabled(bool loading)
	//	{
	//		_isLoading = loading;
	//		SetEnabled();
	//	}

	//	public void Run()
	//	{
	//		Execute();
	//	}
	//	public override void Initialize()
	//	{
	//		_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();
	//		_reportsViewerController.ReportSelectionChanged += ReportsViewerController_ReportSelectionChanged;

	//		SetEnabled();
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
