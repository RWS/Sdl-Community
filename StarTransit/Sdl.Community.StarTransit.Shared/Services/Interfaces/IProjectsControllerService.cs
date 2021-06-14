using System.Collections.Generic;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IProjectsControllerService
	{
		void OpenProjectInFilesView(IProject studioProject);
		IEnumerable<FileBasedProject> GetSelectedProjects();
	}
}
