using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Services
{
	public class ActiveViewService
	{
		private static ProjectsController _projectsController;
		private static FilesController _filesController;
		private static EditorController _editorController;

		private static ActiveViewService _instance;

		private ActiveViewService() { }

		public static ActiveViewService Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ActiveViewService();

					_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
					_projectsController.ActivationChanged += ProjectsController_ActivationChanged;

					_filesController = SdlTradosStudio.Application.GetController<FilesController>();
					_filesController.ActivationChanged += FilesController_ActivationChanged;

					_editorController = SdlTradosStudio.Application.GetController<EditorController>();
					_editorController.ActivationChanged += EditorController_ActivationChanged;					
				}

				return _instance;
			}
		}

		public void Initialize()
		{
			_projectsController.Activate();
		}

		public bool ProjectsViewIsActive { get; private set; }

		public bool FilesViewIsActive { get; private set; }

		public bool EditorViewIsActive { get; private set; }

		private static void ProjectsController_ActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			Instance.ProjectsViewIsActive = e.Active;
		}

		private static void FilesController_ActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			Instance.FilesViewIsActive = e.Active;
		}

		private static void EditorController_ActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			Instance.EditorViewIsActive = e.Active;
		}
	}
}
