using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace ProjectController
{
	[ApplicationInitializer]
	public  class ProjectControllerMethods : IApplicationInitializer
	{
		public void Execute()
		{ 
			//files controller
			var filesController = SdlTradosStudio.Application.GetController<FilesController>();
			var activeProjectFromFiles = filesController.CurrentProject;

			//editor controler
			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var activeProjectFromEditor = editorController.ActiveDocument.Project;
		}
	}
}
