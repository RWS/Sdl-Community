using System.Collections.Generic;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.mtOrigin.Interface
{
	public interface IProcessFileService
	{
		void RemoveTranslationOrigin(List<FileBasedProject> selectedProjects, string newOrigin);
	}
}
