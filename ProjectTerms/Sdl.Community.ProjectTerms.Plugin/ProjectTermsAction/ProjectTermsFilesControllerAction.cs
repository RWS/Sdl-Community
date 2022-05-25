using System;
using System.Windows.Forms;
using Sdl.Community.ProjectTerms.Plugin.Views;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ProjectTerms.Plugin.ProjectTermsAction
{
	[Action("ExtractProjectTermsFiles", Name = "Extract Project Terms Files", Description = "ProjectTerms_Description")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 2, DisplayType.Large)]
    public class ProjectTermsFilesControllerAction : AbstractViewControllerAction<FilesController>
    {
	    public override void Initialize()
	    {
		    base.Initialize();
		    Text = "Extract Project Terms";
	    }
    
	    protected override void Execute()
        {
            if (Utils.Utils.VerifySingleFileProjectType())
            {
                MessageBox.Show(PluginResources.Error_SingleFileProject, PluginResources.MessageType_Info);
                return;
            }

            var projectTermsView = new ProjectTermsView();
            projectTermsView.ProjectSelected = false;
            var parent = projectTermsView.ParentForm;
            projectTermsView.ShowDialog(parent);
        }
    }
}
