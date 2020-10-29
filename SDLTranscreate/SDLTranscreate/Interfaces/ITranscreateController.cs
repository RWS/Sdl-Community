using System.Collections.Generic;
using Sdl.Community.Transcreate.Model;

namespace Sdl.Community.Transcreate.Interfaces
{
	public interface ITranscreateController
	{
		List<ProjectFile> GetSelectedProjectFiles();

		List<IProject> GetSelectedProjects();

		List<IProject> GetProjects();

		void UpdateProjectData(TaskContext taskContext);

		void UpdateBackTranslationProjectData(IProject parentProject, TaskContext taskContext);
	}
}
