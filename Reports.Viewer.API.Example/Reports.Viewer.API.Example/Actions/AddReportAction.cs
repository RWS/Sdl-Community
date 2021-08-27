using System;
using System.IO;
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
			var report1 = new Report
			{			
				Path = $"{Directory.GetCurrentDirectory()}\\Samples\\WordCount.xml",				
				TemplateId = "Sdl.ProjectApi.AutomaticTasks.WordCount",
				Name = $"Demo Report WordCount {DateTime.Now}",
				Language = "de-de"	
			};

			var report2 = new Report
			{			
				Path = $"{Directory.GetCurrentDirectory()}\\Samples\\BasicReport.xml",
				TemplateId = "Sdl.ProjectApi.AutomaticTasks.WordCount",
				Name = $"Demo Report Custom {DateTime.Now}",
				Language = "de-de",
				IsCustomReport = true
			};

			_reportsViewerController.AddReport(report2);	

		}

		public override void Initialize()
		{
			Enabled = true;
			_reportsViewerController = SdlTradosStudio.Application.GetController<ReportsViewerController>();
		}		
	}
}
