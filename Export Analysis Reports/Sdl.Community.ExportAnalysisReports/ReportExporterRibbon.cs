using System.Collections.Generic;
using NLog;
using Sdl.Community.ExportAnalysisReports.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ExportAnalysisReports
{
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	[RibbonGroup("ExportAnalysisReports", Name = "", Description = "Trados Export Analysis Reports")]
	public class ReportExporterRibbon : AbstractRibbonGroup
	{
	}

	[Action("ExportAnalysisReports", Name = "Trados Export Analysis Reports (all projects)", Icon = "folder2_blue", Description = "Open Trados Export Analysis Reports for all analysed projects")]
	[ActionLayout(typeof(ReportExporterRibbon), 20, DisplayType.Large)]
	class ReportExporterViewPartAction : AbstractAction
	{
		public override void Initialize()
		{
			base.Initialize();
			Helpers.Log.Setup();
			Text = "Trados Export Analysis Reports";
		}
		protected override void Execute()
		{
			var pathInfo = new PathInfo();
			var settingsService = new SettingsService(pathInfo);
			var exporter = new ReportExporterControl(settingsService);
			exporter.ShowDialog();
		}
	}

	[Action("ExportAnalysisReports.Button", Name = "Trados Export Analysis Reports (selected projects)", Description = "Open Trados Export Analysis Reports based on analysed projects selection", Icon = "folder2_blue")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "", true)]
	public class ReportExporter : AbstractAction
	{
		public override void Initialize()
		{
			base.Initialize();
			Text = "Trados Export Analysis Reports";
		}
		protected override void Execute()
		{
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var selectedProjects = projectController?.SelectedProjects;
			var pathInfo = new PathInfo();
			var settingsService = new SettingsService(pathInfo);
			var foldersPth = new List<string>();
			foreach (var project in selectedProjects)
			{
				foldersPth.Add(project.FilePath);
			}
			var dialog = new ReportExporterControl(foldersPth, settingsService);
			dialog.ShowDialog();
		}
	}
}