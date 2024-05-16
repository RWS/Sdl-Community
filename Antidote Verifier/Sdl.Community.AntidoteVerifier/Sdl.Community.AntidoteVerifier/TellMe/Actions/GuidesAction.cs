using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.AntidoteVerifier.TellMe.Actions
{
    public class GuidesAction : AbstractTellMeAction
    {
        public GuidesAction()
        {
            Name = $"{PluginResources.Plugin_Name} Guides";
        }

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

        public override Icon Icon => PluginResources.guide;

        public override bool IsAvailable => true;

        public override void Execute() => SdlTradosStudio.Application.ExecuteAction<AntidoteGuideAction>();
    }
}