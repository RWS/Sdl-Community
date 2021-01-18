using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StudioViews.Services
{
	public class ProjectHelper
	{
		private readonly ProjectsController _projectsController;

		public ProjectHelper(ProjectsController projectsController)
		{
			_projectsController = projectsController;
		}

		public void ActivateProject(FileBasedProject project)
		{
			var projectId = project.GetProjectInfo().Id.ToString();
			var selectedProjectId = _projectsController.CurrentProject?.GetProjectInfo().Id.ToString();
			if (projectId != selectedProjectId)
			{
				Dispatcher.CurrentDispatcher.Invoke(delegate
				{
					var internalProjectType = typeof(FileBasedProject).GetProperty("InternalProject",
						BindingFlags.NonPublic | BindingFlags.Instance);
					var projectInstance = internalProjectType?.GetValue(project);

					var activateProjectMethod = _projectsController.GetType().GetMethod("ActivateProject",
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					activateProjectMethod?.Invoke(_projectsController, new[] { projectInstance });

				}, DispatcherPriority.ContextIdle);
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
	}
}
