using Sdl.Community.InSource.Service;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.InSource.Tellme.Actions
{
    public class CreateProjectsTellMeAction : AbstractTellMeAction
    {
        private MessageBoxService _messageBoxService;

        public CreateProjectsTellMeAction()
        {
            Name = $"{PluginResources.Plugin_Name} Create Projects";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";

        public override Icon Icon => PluginResources.CreateProjects_Icon;

        public override bool IsAvailable => true;

        private MessageBoxService MessageBoxService => _messageBoxService ??= new MessageBoxService();

        public override void Execute()
        {
            SdlTradosStudio.Application.GetController<InSourceViewController>().Activate();

            if (!SdlTradosStudio.Application.GetAction<CreateProjectsAction>().Enabled)
            {
                MessageBoxService.ShowInformation(
                    PluginResources.CreateProjectsTellMeAction_Execute_There_are_no_project_requests_,
                    PluginResources.InSourceViewController_CreateProjects_Create_Projects);
                return;
            }

            SdlTradosStudio.Application.ExecuteAction<CreateProjectsAction>();
        }
    }
}