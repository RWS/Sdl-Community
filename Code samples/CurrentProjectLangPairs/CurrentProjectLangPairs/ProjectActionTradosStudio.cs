using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace CurrentProjectLangPairs
{
	[Action("Test action")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class ProjectActionTradosStudio : AbstractAction
	{
		protected override void Execute()
		{
			//project controller
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var currentProject = projectController?.CurrentProject;

			var langPairs = currentProject?.GetProjectInfo()?.TargetLanguages;
		}
	}
}
