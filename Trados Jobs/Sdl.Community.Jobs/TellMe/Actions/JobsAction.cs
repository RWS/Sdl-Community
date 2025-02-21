using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.Jobs.TellMe.Actions
{
    public class JobsAction : AbstractTellMeAction
    {
        public JobsAction()
        {
            Name = $"{PluginResources.Plugin_Name} View";
        }

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.icon;
        public override bool IsAvailable => true;

        public override void Execute() => SdlTradosStudio.Application.GetController<JobsView>().Activate();
    }
}