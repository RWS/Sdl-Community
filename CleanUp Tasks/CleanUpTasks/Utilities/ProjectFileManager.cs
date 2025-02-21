using System.Collections.Generic;
using System.Linq;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace SDLCommunityCleanUpTasks.Utilities
{
	public static class ProjectFileManager
	{
		public static IEnumerable<ProjectFile> GetProjectFiles()
		{
			var files = new List<ProjectFile>();
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

			if (projectsController != null)
			{
				var currentProject = projectsController.SelectedProjects != null && projectsController.SelectedProjects.Count() > 0
						? projectsController.SelectedProjects.ToList()[0]
						: projectsController.CurrentProject;

				var targetFiles = currentProject.GetTargetLanguageFiles().Where(f => f.Role == FileRole.Translatable);
				if (targetFiles.Count() > 0)
				{
					files.AddRange(targetFiles);
				}
			}
			return files;
		}

		public static string GetProjectFolder()
		{
			var folder = string.Empty;
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var filesController = SdlTradosStudio.Application.GetController<FilesController>();

			if (projectsController != null)
			{
				if (projectsController.SelectedProjects.Count() == 1)
				{
					var project = projectsController.SelectedProjects.FirstOrDefault();
					folder = project.FilePath;
				}
			}

			if (string.IsNullOrEmpty(folder) && filesController != null)
			{
				folder = filesController.CurrentProject.FilePath;
			}

			return folder;
		}

		public static string GetTargetLanguageFolder()
		{
			var folder = string.Empty;
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var filesController = SdlTradosStudio.Application.GetController<FilesController>();

			if (projectsController != null)
			{
				if (projectsController.SelectedProjects.Count() == 1)
				{
					var project = projectsController.SelectedProjects.FirstOrDefault();

					if (project != null)
					{
						var firstFile = project.GetTargetLanguageFiles().FirstOrDefault();
						if (firstFile != null)
						{
							folder = firstFile.Folder;
						}
					}
				}
			}

			if (string.IsNullOrEmpty(folder) && filesController != null)
			{
				var firstFile = filesController.SelectedFiles.FirstOrDefault();

				if (firstFile != null)
				{
					folder = firstFile.Folder;
				}
			}

			return folder;
		}
	}
}