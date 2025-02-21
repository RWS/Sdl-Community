using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Trados.Transcreate.TellMe.Actions
{
    public class ExportAction : AbstractTellMeAction
    {
        public ExportAction()
        {
            Name = $"{PluginResources.Plugin_Name} Export";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.Export;

        public override bool IsAvailable =>
            SdlTradosStudio.Application.GetAction<Transcreate.Actions.ExportAction>().Enabled &&
            SdlTradosStudio.Application.GetController<TranscreateViewController>().IsActive;

        public override void Execute() => SdlTradosStudio.Application.ExecuteAction<Transcreate.Actions.ExportAction>();
    }
}