using System.Collections.Generic;
using System.Linq;
using Sdl.Community.ExportAnalysisReports.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ExportAnalysisReports
{
	public class AbstractExportReportAction : AbstractAction
	{
		public void ExecuteAction()
		{
			Execute();
		}

		public override void Initialize()
		{
			base.Initialize();
			Text = PluginResources.Plugin_Name;
		}

		protected override void Execute()
		{
			var pathInfo = new PathInfo();
			var settingsService = new SettingsService(pathInfo);
			var projectService = new ProjectService();
			var messageBoxService = new MessageBoxService();
			var reportService = new ReportService(messageBoxService, projectService, settingsService);

			var studioProjectsPaths = new List<string>();
			var selectedProjects = projectService.GetSelectedStudioProjects();
			if (selectedProjects != null)
			{
				studioProjectsPaths.AddRange(selectedProjects.Select(project => project.FilePath));
			}

			var dialog = new ReportExporterControl(studioProjectsPaths, settingsService, projectService, messageBoxService, reportService);
			dialog.ShowDialog();
		}
	}

	[Action("ExportAnalysisReports.Button", Name = "Trados Export Analysis Reports (selected projects)", Description = "Open Trados Export Analysis Reports based on analysed projects selection", Icon = "folder2_blue")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "", true)]
	public class ReportExporter : AbstractExportReportAction
	{
	}

	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	[RibbonGroup("ExportAnalysisReports", Name = "", Description = "Trados Export Analysis Reports")]
	public class ReportExporterRibbon : AbstractRibbonGroup
	{
	}

	[Action("ExportAnalysisReports", Name = "Trados Export Analysis Reports (all projects)", Icon = "folder2_blue", Description = "Open Trados Export Analysis Reports for all analysed projects")]
	[ActionLayout(typeof(ReportExporterRibbon), 20, DisplayType.Large)]
	internal class ReportExporterViewPartAction : AbstractExportReportAction
	{
	}
}