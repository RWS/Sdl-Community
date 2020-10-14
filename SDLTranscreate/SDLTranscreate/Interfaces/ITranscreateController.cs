using System.Collections.Generic;
using Sdl.Community.Transcreate.Model;

namespace Sdl.Community.Transcreate.Interfaces
{
	public interface ITranscreateController
	{
		List<ProjectFile> GetSelectedProjectFiles();

		List<Project> GetSelectedProjects();

		List<Project> GetProjects();

		void UpdateProjectData(TaskContext taskContext, bool isBatchTask = false);	
	}
}
