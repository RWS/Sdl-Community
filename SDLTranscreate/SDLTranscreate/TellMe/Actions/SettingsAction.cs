using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;
using Trados.Transcreate.Actions;

namespace Trados.Transcreate.TellMe.Actions
{
    public class SettingsAction : AbstractTellMeAction
    {
        public SettingsAction()
        {
            Name = $"{PluginResources.Plugin_Name} Settings";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.Settings2;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            SdlTradosStudio.Application.GetController<TranscreateViewController>().Activate();
            SdlTradosStudio.Application.ExecuteAction<OpenSettingsAction>();
        }
    }
}