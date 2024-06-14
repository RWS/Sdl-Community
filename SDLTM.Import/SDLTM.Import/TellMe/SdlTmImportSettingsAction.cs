using System.Drawing;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using SDLTM.Import.Studio;

namespace SDLTM.Import.TellMe
{
    public class SdlTmImportSettingsAction : AbstractTellMeAction
    {
        public SdlTmImportSettingsAction()
        {
            Name = "SDLTM Import Plus Settings";
        }

        public override string Category => "SDLTM Import Plus results";

        public override Icon Icon => PluginResources.TellMe_Settings;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            SdlTradosStudio.Application.ExecuteAction<SdlImportAction>();
        }
    }
}
