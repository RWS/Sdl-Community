using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Trados.Transcreate.Model
{
	public class Controllers
	{
		public Controllers(ProjectsController projectsController, FilesController filesController, EditorController editorController, 
			TranscreateViewController transcreateViewController)
		{
			//ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			//FilesController = SdlTradosStudio.Application.GetController<FilesController>();
			//EditorController = SdlTradosStudio.Application.GetController<EditorController>();
			//TranscreateController = SdlTradosStudio.Application.GetController<TranscreateViewController>();

			ProjectsController = projectsController;
			FilesController = filesController;
			EditorController = editorController;
			TranscreateController = transcreateViewController;
		}
		
		public ProjectsController ProjectsController { get; }
		public FilesController FilesController { get; }
		public EditorController EditorController { get; }
		public TranscreateViewController TranscreateController { get; }
	}
}
