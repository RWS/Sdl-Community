using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.AntidoteVerifier.TellMe.Actions
{
    public class GuidesAction : AntidoteToolTellMeAction
    {
        public GuidesAction() : base("Guides")
        {
        }

        public override Icon Icon => PluginResources.guide;

        protected override void ExecuteTool()
            => SdlTradosStudio.Application.ExecuteAction<AntidoteGuideAction>();
    }
}
