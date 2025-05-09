using Sdl.Community.GSVersionFetch.Studio;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.GSVersionFetch.TellMe.Actions
{
    public class GroupshareVersionFetchAction : AbstractTellMeAction
    {
        public GroupshareVersionFetchAction()
        {
            Name = $"{PluginResources.Plugin_Name}";
        }

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.gs_fetch_versions_Hep_icon;
        public override bool IsAvailable => true;

        public override void Execute() => SdlTradosStudio.Application.ExecuteAction<GsVersionFetchAction>();
    }
}