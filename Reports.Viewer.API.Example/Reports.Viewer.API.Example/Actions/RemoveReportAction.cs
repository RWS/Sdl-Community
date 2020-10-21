using System;
using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Reports.Viewer.API.Example.Actions
{
	[Action("ReportsViewerAPIExample_RemoveReport_Action",
		Name = "ReportsViewerAPIExample_RemoveReport_Name",
		Description = "ReportsViewerAPIExample_RemoveReport_Description",
		ContextByType = typeof(ReportsViewerController),
		Icon = "Delete"
	)]
	[ActionLayout(typeof(ReportsViewerAPIExampleActionsGroup), 7, DisplayType.Large)]
	public class RemoveReportAction : AbstractViewControllerAction<ReportsViewerController>
	{		
		private ReportsViewerController _reportsViewerController;

		protected override void Execute()
		{
			var reports = _reportsViewerController.GetSelectedReports();
			if (reports?.Count == 0)
			{
				return;
			}

			var message1 = "This action will remove the selected reports from the project";
			var message2 = "Do you want to proceed?";

			var response = MessageBox.Show(message1 + Environment.NewLine + Environment.NewLine + message2,
				PluginResources.Plugin_Name, MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (response == MessageBoxResult.No)
			{
				return;
			}

			_reportsViewerController.RemoveReports(reports);
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
