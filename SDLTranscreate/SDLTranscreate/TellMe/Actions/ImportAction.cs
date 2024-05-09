using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Trados.Transcreate.TellMe.Actions
{
    public class ImportAction : AbstractTellMeAction
    {
        public ImportAction()
        {
            Name = $"{PluginResources.Plugin_Name} Import";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.Import;

        public override bool IsAvailable =>
            SdlTradosStudio.Application.GetAction<Transcreate.Actions.ImportAction>().Enabled &&
            SdlTradosStudio.Application.GetController<TranscreateViewController>().IsActive;

        public override void Execute() => SdlTradosStudio.Application.ExecuteAction<Transcreate.Actions.ImportAction>();
    }
}