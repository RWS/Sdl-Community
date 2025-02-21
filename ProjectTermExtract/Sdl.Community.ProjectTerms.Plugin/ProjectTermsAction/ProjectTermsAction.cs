using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.ProjectTerms.Plugin.Views;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ProjectTerms.Plugin.ProjectTermsAction
{
	[Action("ExtractProjectTerms", Name = "Extract Project Terms", Description = "ProjectTerms_Description")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Large)]
	public class ProjectTermsAction : AbstractViewControllerAction<ProjectsController>
	{
		public override void Initialize()
		{
			StudioContext.ControllersAvailableEvent += StudioContextOnControllersAvailableEvent;
		}

		protected override void Execute()
		{
			if (Utils.Utils.VerifySingleFileProjectType())
			{
				MessageBox.Show(PluginResources.Error_SingleFileProject, PluginResources.MessageType_Info);
				return;
			}
			var projectTermsView = new ProjectTermsView();
			projectTermsView.ProjectSelected = true;

			var parent = projectTermsView.ParentForm;
			projectTermsView.ShowDialog(parent);
		}

		private void _projectsController_SelectedProjectsChanged(object sender, EventArgs e)
		{
			Enabled = false;
			// check if selected project having files available or not
			var project = StudioContext.ProjectsController.CurrentProject;
			if (project != null)
			{
				var files = GetFiles();
				// check current project name and select project both should be same then option should be enabled
				if (files != null && files.Count > 0 && StudioContext.ProjectsController.SelectedProjects.Count() > 0 && project.GetProjectInfo().Name == StudioContext.ProjectsController.SelectedProjects.First().GetProjectInfo().Name)
				{
					if (files.Any(file => File.Exists(file.LocalFilePath)))
					{
						Enabled = true;
					}
				}
			}
		}

		private List<ProjectFile> GetFiles()
		{
			List<ProjectFile> sourceFilesToProcessed = new List<ProjectFile>();
			var projectSourceFiles = StudioContext.ProjectsController.CurrentProject.GetSourceLanguageFiles();
			if (projectSourceFiles != null)
			{
				foreach (var file in projectSourceFiles)
				{
					if (!file.Name.Contains(StudioContext.ProjectsController.CurrentProject.GetProjectInfo().Name))
					{
						sourceFilesToProcessed.Add(file);
					}
				}
			}
			return sourceFilesToProcessed;
		}

		private void StudioContextOnControllersAvailableEvent()
		{
			StudioContext.ProjectsController.SelectedProjectsChanged -= _projectsController_SelectedProjectsChanged;
			StudioContext.ProjectsController.SelectedProjectsChanged += _projectsController_SelectedProjectsChanged;
		}
	}
}