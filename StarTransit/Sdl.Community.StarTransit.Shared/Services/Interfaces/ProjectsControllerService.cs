using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public class ProjectsControllerService : IProjectsControllerService
	{
		private readonly ProjectsController _projectsController;

		public ProjectsControllerService(ProjectsController projectsController)
		{
			_projectsController = projectsController;
		}

		public void OpenProjectInFilesView(IProject studioProject)
		{
			_projectsController?.Open((FileBasedProject)studioProject);
		}
	}
}
