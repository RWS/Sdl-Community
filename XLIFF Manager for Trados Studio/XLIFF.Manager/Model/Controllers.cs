using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class Controllers
	{
		public Controllers()
		{			
			ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			FilesController = SdlTradosStudio.Application.GetController<FilesController>();
			EditorController = SdlTradosStudio.Application.GetController<EditorController>();
			XliffManagerController = SdlTradosStudio.Application.GetController<XLIFFManagerViewController>();
		}

		public ProjectsController ProjectsController { get; }
		public FilesController FilesController { get; }
		public EditorController EditorController { get; }
		public XLIFFManagerViewController XliffManagerController { get; }
	}
}
