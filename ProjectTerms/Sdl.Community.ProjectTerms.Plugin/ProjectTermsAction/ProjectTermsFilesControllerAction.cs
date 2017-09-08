using Sdl.Community.ProjectTerms.Plugin.Views;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Windows.Forms;

namespace Sdl.Community.ProjectTerms.Plugin.ProjectTermsAction
{
    [Action("ExtractProjectTermsFiles", Name = "Extract Project Terms", Description = "ProjectTerms_Description")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 2, DisplayType.Large)]
    public class ProjectTermsFilesControllerAction : AbstractViewControllerAction<FilesController>
    {
        protected override void Execute()
        {
            if (Utils.Utils.VerifySingleFileProjectType())
            {
                MessageBox.Show(PluginResources.Error_SingleFileProject, PluginResources.MessageType_Info);
                return;
            }

            var projectTermsView = new ProjectTermsView();
            var parent = projectTermsView.ParentForm;
            projectTermsView.ShowDialog(parent);
        }
    }
}
