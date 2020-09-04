using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Transcreate.Model
{
	public class Controllers
	{
		public Controllers()
		{			
			ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			FilesController = SdlTradosStudio.Application.GetController<FilesController>();
			EditorController = SdlTradosStudio.Application.GetController<EditorController>();
			TranscreateController = SdlTradosStudio.Application.GetController<TranscreateViewController>();
		}

		public ProjectsController ProjectsController { get; }
		public FilesController FilesController { get; }
		public EditorController EditorController { get; }
		public TranscreateViewController TranscreateController { get; }
	}
}
