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
			_projectsController=SdlTradosStudio.Application.GetController<ProjectsController>();
			_projectsController.SelectedProjectsChanged += _projectsController_SelectedProjectsChanged;		
		}
		private void _projectsController_SelectedProjectsChanged(object sender, EventArgs e)
		{
			// check if selected project having files available or not 
			var project = _projectsController.CurrentProject;			
			if (project != null)
			{// change code check all the files from project			
				var projectFolderPath = Utils.Utils.GetProjectPath();
				var files = Directory.GetFiles(projectFolderPath);
				// check current project name and select project both should be same then option should be enabled
				if (files != null && files.Length > 0 && project.GetProjectInfo().Name== _projectsController.SelectedProjects.First().GetProjectInfo().Name)
				{
					EnableAction(true);
				}
				else
				{
					EnableAction(false);
				}
			}
			else
			{
				EnableAction(false);
			}
		}
		public void EnableAction(bool enable)
		{
			Enabled = enable;
		}
    }
}
