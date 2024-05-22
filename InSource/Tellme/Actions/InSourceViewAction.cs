using Sdl.TellMe.ProviderApi;
using System.Drawing;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.InSource.Tellme.Actions
{
    public class InSourceViewAction : AbstractTellMeAction
    {
        public InSourceViewAction()
        {
            Name = $"{PluginResources.Plugin_Name} View";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";

        public override Icon Icon => PluginResources.InSource_large;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            SdlTradosStudio.Application.GetController<InSourceViewController>().Activate();
        }
    }
}