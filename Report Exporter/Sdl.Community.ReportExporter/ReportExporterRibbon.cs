using System;
using System.Collections.Generic;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ReportExporter
{
	[RibbonGroup("Sdl.Community.ReportExporter", Name = "Report Exporter")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]

	public class ReportExporterRibbon : AbstractRibbonGroup
	{
	}

	[Action("Sdl.Community.ReportExporter", Name = "Report Exporter", Icon = "folder2_blue", Description = "Studio Report Exporter")]
	[ActionLayout(typeof(ReportExporterRibbon), 20, DisplayType.Large)]
	class ReportExporterViewPartAction : AbstractAction
	{

		protected override void Execute()
		{
			var exporter = new ReportExporterControl();
			exporter.ShowDialog();
		}
	}

	[Action("Sdl.Community.ReportExporter.Button",
		Name = "Report Exporter",
		Description = "Studio Report Exporter",
		Icon = "folder2_blue")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "", true)]
	public class ReportExporter : AbstractAction
	{
		protected override void Execute()
		{
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var selectedProjects = projectController.SelectedProjects;
			var foldersPth = new List<string>();
			foreach (var project in selectedProjects)
			{
				foldersPth.Add(project.FilePath);
			}
			var dialog = new ReportExporterControl(foldersPth);
			dialog.ShowDialog();
		}
	}
}