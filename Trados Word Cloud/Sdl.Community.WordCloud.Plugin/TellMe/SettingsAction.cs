using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TellMe.ProviderApi;
using System.Drawing;

namespace Sdl.Community.WordCloud.Plugin.TellMe
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
            SdlTradosStudio.Application.ExecuteAction<GenerateWordCloudAction>();
        }
    }
}
