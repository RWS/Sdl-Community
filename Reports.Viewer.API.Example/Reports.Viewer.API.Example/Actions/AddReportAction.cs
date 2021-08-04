using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.FileBased.Reports.Models;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.API.Example.Actions
{
	[Action("ReportsViewerAPIExample_AddReport_Action",
		Name = "ReportsViewerAPIExample_AddReport_Name",
		Description = "ReportsViewerAPIExample_AddReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Add"
	)]
	[ActionLayout(typeof(ReportsViewerAPIExampleActionsGroup), 9, DisplayType.Large)]
	public class AddReportAction : AbstractViewControllerAction<ReportsViewerController>
	{
		private ReportsViewerController _reportsViewerController;

		protected override void Execute()
		{
			var testData = new TestData.TestDataUtil();

			var report = new Report
			{
				Path = testData.CreateReport(),
				TemplateId= "Sdl.ProjectApi.AutomaticTasks.Translate",
				Name = $"Demo Report{DateTime.Now}",
				Language = "de-de"
			};

			_reportsViewerController.AddReport(report);	

		}

		public override void Initialize()
		{
			Enabled = true;
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();
		}
	}
}
