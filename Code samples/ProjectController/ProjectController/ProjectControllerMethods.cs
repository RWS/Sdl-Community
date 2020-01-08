using System.IO;
using System.Linq;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace ProjectController
{
	[ApplicationInitializer]
	public class ProjectControllerMethods : IApplicationInitializer
	{
		public void Execute()
		{
			//files controller
			var filesController = SdlTradosStudio.Application.GetController<FilesController>();
			var activeProjectFromFiles = filesController.CurrentProject;
		}
	}


	[RibbonGroup("Test", Name = "Test project icon")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class ProjectTemplateRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("TestProjIconAction", Icon = "", Name = "Test icon",
		Description = "")]
	[ActionLayout(typeof(ProjectTemplateRibbonGroup), 10, DisplayType.Large)]
	public class ProjectAction : AbstractAction
	{
		protected override void Execute()
		{
			//projects controler
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var activeProject = projectsController?.CurrentProject;

			var termbConfig = activeProject?.GetTermbaseConfiguration();
			var tembaseSettings = termbConfig?.Termbases.FirstOrDefault()?.SettingsXML;
			var tembase = termbConfig?.Termbases.FirstOrDefault();

			// Adapting translate task settings
			//var settings = activeProject?.GetSettings();

			//// Injecting custom type and icon for the project
			//var projectSettings = settings?.GetSettingsGroup<ProjectSettings>();
			//if (projectSettings != null)
			//{
			//	projectSettings.ProjectOrigin = "Example project type";
			//	projectSettings.ProjectIconPath =
			//		"icon path"; //@"C:\Repository\AppStoreApplications\LingotekTMS\LingotekTMS\Resources\icon.ico";
			//}

			//activeProject?.UpdateSettings(settings);
			//activeProject?.Save();
			//projectsController?.RefreshProjects();
		}
	}
}
