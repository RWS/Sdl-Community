using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExportAnalysisReports.Model;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace ExportAnalysisReports.Helpers
{
	public static class ProjectInformation
	{
		public static ProjectsController ProjectController => SdlTradosStudio.Application.GetController<ProjectsController>();
		private static string GetInternalProjectStatus(FileBasedProject studioProject)
		{
			var internalProjPropertyInfo =
				studioProject.GetType().GetProperty("InternalProject", BindingFlags.NonPublic | BindingFlags.Instance);

			if (internalProjPropertyInfo != null)
			{
				var internalProjMethodInfo = internalProjPropertyInfo.GetGetMethod(true);

				dynamic internalProject = internalProjMethodInfo.Invoke(studioProject, null);
				var statusProperty = internalProject.GetType().GetProperty("Status");
				var projectStatus = statusProperty.GetValue(internalProject, null).ToString();
				return projectStatus;
			}
			return string.Empty;
		}

		public static List<FileBasedProject> GetStudioProjects()
		{
			return ProjectController.GetAllProjects().ToList();
		}

		public static List<FileBasedProject> GetSelectedProjects()
		{
			return ProjectController.SelectedProjects.ToList();
		}

		public static string GetProjectStatus(string projectPath)
		{
			var studioProject = GetStudioProjects().FirstOrDefault(p => p.FilePath.Equals(projectPath));

			if (studioProject != null)
			{
				var projectStatus = GetInternalProjectStatus(studioProject);
				return projectStatus;
			}
			return string.Empty;
		}

		public static ProjectDetails GetExternalProjectDetails(string path)
		{
			var fileBasedProject = new FileBasedProject(path);
			var projectInfo = fileBasedProject.GetProjectInfo();

			var projectDetails = new ProjectDetails
			{
				ProjectName = projectInfo.Name,
				ProjectPath = projectInfo.Uri.LocalPath,
				Status = GetInternalProjectStatus(fileBasedProject),
				LanguagesForPoject =  new Dictionary<string, bool>(),
				ShouldBeExported = true
			};
			foreach (var language in projectInfo.TargetLanguages)
			{
				projectDetails.LanguagesForPoject.Add(language.DisplayName,true);
			}
			ProjectController.Close(fileBasedProject);

			return projectDetails;
		}
	}
}
