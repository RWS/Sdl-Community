using Sdl.TellMe.ProviderApi;
using System.Drawing;

namespace SDLCommunityCleanUpTasks.TellMe
{
    public class CleanUpTasksSettingsAction : AbstractTellMeAction
    {
        CleanUpTasksSettingsAction()
        {
            Name = "CleanUpTasks Settings";
        }

        public override bool IsAvailable => true;
        public override string Category => "CleanUpTasks results";
        public override Icon Icon => PluginResources.TellMe_Settings;

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
