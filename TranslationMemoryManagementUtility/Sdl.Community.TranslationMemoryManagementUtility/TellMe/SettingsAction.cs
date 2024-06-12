using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;
using static Sdl.Community.TranslationMemoryManagementUtility.TranslationMemoryProviderRibbon;

namespace Sdl.Community.TranslationMemoryManagementUtility
{
    public class SettingsAction : AbstractTellMeAction
    {
        public SettingsAction()
        {
            Name = $"{PluginResources.Plugin_Name} Settings";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.TellMe_Settings;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            SdlTradosStudio.Application.ExecuteAction<TMProviderAction>();
        }
    }
}
