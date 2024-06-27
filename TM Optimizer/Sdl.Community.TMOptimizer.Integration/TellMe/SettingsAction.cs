using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.TMOptimizer.Integration.TellMe
{
    public class SettingsAction : AbstractTellMeAction
    {
        public SettingsAction()
        {
            Name = $"{PluginResources.Plugin_Name} Settings";
        }

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.TellMe_Settings;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            SdlTradosStudio.Application.ExecuteAction<TmOptimizerViewPart>();
        }
    }
}
