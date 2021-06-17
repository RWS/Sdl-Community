using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Versioning;

namespace Sdl.Community.StudioViews.Services
{
	public class ProjectService
	{
		private readonly ProjectsController _projectsController;
		private readonly StudioVersionService _studioVersionService;

		public ProjectService(ProjectsController projectsController, StudioVersionService studioVersionService)
		{
			_projectsController = projectsController;
			_studioVersionService = studioVersionService;
		}

		public bool IsServerProject(FileBasedProject project)
		{
			var projectInfo = project.GetProjectInfo();
			var isServerProject = !string.IsNullOrEmpty(projectInfo.ServerUri?.AbsoluteUri)
			                      && projectInfo.PublicationStatus == PublicationStatus.Published;

			return isServerProject;
		}

		public void ActivateProject(FileBasedProject project)
		{
			if (project == null)
			{
				return;
			}

			var projectId = project.GetProjectInfo().Id.ToString();
			var selectedProjectId = _projectsController.CurrentProject?.GetProjectInfo().Id.ToString();
			if (projectId != selectedProjectId)
			{
				var canActivateProject = CanActivateFileBasedProject(out var fullySupported);
				if (!canActivateProject)
				{
					return;
				}
				
				if (fullySupported)
				{
					_projectsController.ActivateProject(project);

					//var activateProjectMethod = _projectsController.GetType().GetMethod("ActivateProject",
					//	BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					//activateProjectMethod?.Invoke(_projectsController, new object[] { project });
				}
				else
				{
					var internalProjectType = typeof(FileBasedProject).GetProperty("InternalProject",
						BindingFlags.NonPublic | BindingFlags.Instance);
					var projectInstance = internalProjectType?.GetValue(project);

					var activateProjectMethod = _projectsController.GetType().GetMethod("ActivateProject",
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					activateProjectMethod?.Invoke(_projectsController, new[] { projectInstance });
				}
			}

			Dispatcher.CurrentDispatcher.Invoke(delegate { }, DispatcherPriority.ContextIdle);
		}

		public string GetProjectType(FileBasedProject project)
		{
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				return internalDynamicProject.ProjectType.ToString();
			}

			return null;
		}

		public List<Model.AnalysisBand> GetAnalysisBands(FileBasedProject project)
		{
			var regex = new Regex(@"(?<min>[\d]*)([^\d]*)(?<max>[\d]*)", RegexOptions.IgnoreCase);

			var analysisBands = new List<Model.AnalysisBand>();
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				foreach (var analysisBand in internalDynamicProject.AnalysisBands)
				{
					Match match = regex.Match(analysisBand.ToString());
					if (match.Success)
					{
						var min = match.Groups["min"].Value;
						var max = match.Groups["max"].Value;
						analysisBands.Add(new Model.AnalysisBand
						{
							MinimumMatchValue = Convert.ToInt32(min),
							MaximumMatchValue = Convert.ToInt32(max)
						});
					}
				}
			}

			return analysisBands;
		}

		private bool CanActivateFileBasedProject(out bool fullySupported)
		{
			fullySupported = true;
			var studioVersion = _studioVersionService.GetStudioVersion();
			if (studioVersion != null)
			{
				var version = studioVersion.ExecutableVersion;
				if (version.Major == 16 && version.Minor == 1 && version.Build == 3)
				{
					fullySupported = false;
					return true;
				}
				
				if (version.Major >= 16 && version.Minor >= 1 && version.Build >= 4)
				{
					return true;
				}
			}

			return false;
		}
	}
}
