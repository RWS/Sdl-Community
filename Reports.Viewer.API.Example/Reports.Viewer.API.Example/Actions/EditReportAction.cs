using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.API.Example.Actions
{
	[Action("ReportsViewerAPIExample_EditReport_Action",
		Name = "ReportsViewerAPIExample_EditReport_Name",
		Description = "ReportsViewerAPIExample_EditReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Edit"
	)]
	[ActionLayout(typeof(ReportsViewerAPIExampleActionsGroup), 8, DisplayType.Large)]
	public class EditReportAction : AbstractViewControllerAction<ReportsViewerController>
	{
		private ReportsViewerController _reportsViewerController;

		protected override void Execute()
		{
			var reports = _reportsViewerController.GetSelectedReports();
			if (reports == null || reports.Count == 0)
			{
				return;
			}

			foreach (var report in reports)
			{
				report.Name += " [updated]";
				report.Description += $" [updated] {DateTime.Now}";				
				_reportsViewerController.UpdateReport(report);
			}			
		}

		public void Run()
		{
			Execute();
		}

		public override void Initialize()
		{
			Enabled = true;
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();
		}
	}
}
