using Sdl.Community.ProjectTerms.Plugin.Views;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Windows.Forms;

namespace Sdl.Community.ProjectTerms.Plugin.ProjectTermsAction
{
    [Action("ExtractProjectTerms", Name = "Extract Project Terms", Description = "ProjectTerms_Description")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Large)]
    public class ProjectTermsAction : AbstractViewControllerAction<ProjectsController>
    {
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
    }
}
