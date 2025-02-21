using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Trados.Transcreate.TellMe.Actions
{
    public class ShowTranscreateViewAction : AbstractTellMeAction
    {
        public ShowTranscreateViewAction()
        {
            Name = $"{PluginResources.Plugin_Name} View";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.Icon;
        public override bool IsAvailable => !SdlTradosStudio.Application.GetController<TranscreateViewController>().IsActive;

        public override void Execute() =>
            SdlTradosStudio.Application.GetController<TranscreateViewController>().Activate();
    }
}