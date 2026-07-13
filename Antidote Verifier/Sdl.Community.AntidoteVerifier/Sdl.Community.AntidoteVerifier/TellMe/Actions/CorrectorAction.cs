using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.AntidoteVerifier.TellMe.Actions
{
    public class CorrectorAction : AntidoteToolTellMeAction
    {
        public CorrectorAction() : base("Corrector")
        {
        }

        public override Icon Icon => PluginResources.boutonCorrecteur;

        protected override void ExecuteTool()
            => SdlTradosStudio.Application.ExecuteAction<AntidoteCorrectorAction>();
    }
}
