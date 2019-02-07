using System.Collections.Generic;
using System.Linq;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	public static class ProjectFileManager
    {
        public static IEnumerable<ProjectFile> GetProjectFiles()
        {
            List<ProjectFile> files = new List<ProjectFile>();

            var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            var filesController = SdlTradosStudio.Application.GetController<FilesController>();

            if (projectsController != null)
            {
                foreach (var project in projectsController.SelectedProjects)
                {
                    files.AddRange(project.GetTargetLanguageFiles().Where(f => f.Role == FileRole.Translatable));
                }
            }

            if (files.Count == 0 && filesController != null)
            {
                files.AddRange(filesController.SelectedFiles.Where(f => f.Role == FileRole.Translatable));
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