using System.Collections.Generic;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.Interfaces
{
	public interface ITranscreateController
	{
		List<ProjectFile> GetSelectedProjectFiles();

		List<IProject> GetSelectedProjects();

		List<IProject> GetProjects();

		void UpdateProjectData(TaskContext taskContext);

		void UpdateBackTranslationProjectData(string parentProjectId, TaskContext taskContext);
	}
}
