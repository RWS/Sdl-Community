using Sdl.Community.ProjectTerms.Plugin.ProjectTermsAction;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;
using System.Linq;

namespace Sdl.Community.ProjectTerms.Plugin.TellMe
{
    public class SettingsAction : AbstractTellMeAction
    {
        public SettingsAction()
        {
            Name = $"{PluginResources.Plugin_Name} Settings";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.TellMe_Settings;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            // first check if the files Controller is active
            if (ApplicationContext.FilesControllerIsActive)
            {
                if (!ApplicationContext.FilesController.SelectedFiles.Any())
                {
                    ApplicationContext.FilesController.SelectFiles(ApplicationContext.FilesController.CurrentVisibleFiles);
                }

                // if there are any files selected then load the File Terms Action
                if (ApplicationContext.FilesController.SelectedFiles.Any())
                {
                    SdlTradosStudio.Application.ExecuteAction<ProjectTermsFilesControllerAction>();
                    return;
                }
            }

            // if the files controller is not active then check if the projects controller is active.
            // if not, then activate it!
            if (!ApplicationContext.ProjectsControllerIsActive)
            {
                ApplicationContext.ProjectsController.Activate();
            }


            // check if there is a project active;
            // if not, then activate the selected, or first one in the list
            if (ApplicationContext.ProjectsController.CurrentProject == null)
            {
                if (ApplicationContext.ProjectsController.GetAllProjects().Any())
                {
                    ApplicationContext.ProjectsController.ActivateProject(
                        ApplicationContext.ProjectsController.SelectedProjects?.FirstOrDefault() ??
                        ApplicationContext.ProjectsController.GetAllProjects().FirstOrDefault());
                }
            }


            SdlTradosStudio.Application.ExecuteAction<ProjectTermsAction.ProjectTermsAction>();
        }
    }
}
