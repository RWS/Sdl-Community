using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace ProjectCredentialsTest
{
	//[Action("ProjectCredentialsTest")]
	//[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	//public class MyCustomTradosStudio : AbstractAction
	//{
	//	protected override void Execute()
	//	{
	//		var termbaseUri = new Uri("");


	//		var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
	//		projectController.CurrentProject.Credentials.AddCredential(termbaseUri, true, "", "");
	//	}
	//}
	[Action("Merge Project files")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MyCustomTradosStudio : AbstractAction
	{
		protected override void Execute()
		{
			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var selectedProject = projectController?.CurrentProject;
			if (selectedProject != null)
			{
				var sourceFiles = selectedProject.GetSourceLanguageFiles();
				selectedProject.SetFileRole(sourceFiles.GetIds(), FileRole.Translatable);
				selectedProject.CreateMergedProjectFile("MergedFile.sdlxliff", string.Empty, sourceFiles.GetIds());
			}
		}
	}
}