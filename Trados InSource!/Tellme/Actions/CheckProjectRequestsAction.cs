using Sdl.TellMe.ProviderApi;
using System.Drawing;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.InSource.Tellme.Actions
{
    public class CheckProjectRequestsAction : AbstractTellMeAction
    {
        public CheckProjectRequestsAction()
        {
            Name = $"{PluginResources.Plugin_Name} Check Project Requests";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";

        public override Icon Icon => PluginResources.CheckForProjects;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            SdlTradosStudio.Application.GetController<InSourceViewController>().Activate();
            SdlTradosStudio.Application.ExecuteAction<CheckForProjectsAction>();
        }
    }
}