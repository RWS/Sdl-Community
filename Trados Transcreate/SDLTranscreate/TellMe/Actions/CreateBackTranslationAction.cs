using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Trados.Transcreate.TellMe.Actions
{
    public class CreateBackTranslationAction : AbstractTellMeAction
    {
        public CreateBackTranslationAction()
        {
            Name = $"{PluginResources.Plugin_Name} Create Back-Translation";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.back_translation_small;

        public override bool IsAvailable =>
            SdlTradosStudio.Application.GetAction<Transcreate.Actions.CreateBackTranslationAction>().Enabled &&
            SdlTradosStudio.Application.GetController<TranscreateViewController>().IsActive;

        public override void Execute() => SdlTradosStudio.Application.ExecuteAction<Transcreate.Actions.CreateBackTranslationAction>();
    }
}