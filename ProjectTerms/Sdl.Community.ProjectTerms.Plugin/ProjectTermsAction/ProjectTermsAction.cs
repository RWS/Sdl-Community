using Sdl.Community.ProjectTerms.Plugin.Views;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.Platform;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Windows.Forms;
using System.IO;
using Sdl.ProjectAutomation.FileBased;
using System.Linq;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;

namespace Sdl.Community.ProjectTerms.Plugin.ProjectTermsAction
{
    [Action("ExtractProjectTerms", Name = "Extract Project Terms", Description = "ProjectTerms_Description")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Large)]
    public class ProjectTermsAction : AbstractViewControllerAction<ProjectsController>
	{
		private ProjectsController _projectsController;

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
		public override void Initialize()
		{
			
			_projectsController =SdlTradosStudio.Application.GetController<ProjectsController>();
			_projectsController.SelectedProjectsChanged += _projectsController_SelectedProjectsChanged;		
		}
		private void _projectsController_SelectedProjectsChanged(object sender, EventArgs e)
		{
			Enabled = false;
			// check if selected project having files available or not 
			var project = _projectsController.CurrentProject;			
			if (project != null)
			{// change code check all the files from project	
				
				var files = GetFiles();
				// check current project name and select project both should be same then option should be enabled
				if (files != null && files.Count > 0 && _projectsController.SelectedProjects.Count() >0 && project.GetProjectInfo().Name== _projectsController.SelectedProjects.First().GetProjectInfo().Name)
				{					
					foreach(ProjectFile file in files.ToList())
					{
						if(File.Exists(file.LocalFilePath))
						{
							Enabled = true;
						}
					}
				}
			}
		}
		private List<ProjectFile> GetFiles()
		{
			List<ProjectFile> sourceFilesToProcessed = new List<ProjectFile>();

			var projectSourceFiles = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetSourceLanguageFiles();
			if (projectSourceFiles !=null)
			{
				foreach (var file in projectSourceFiles)
				{
					if (!file.Name.Contains(SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetProjectInfo().Name))
					{
						sourceFilesToProcessed.Add(file);
					}
				}
			}

			return sourceFilesToProcessed;
		}
	}
}
