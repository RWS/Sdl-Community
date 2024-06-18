using Sdl.Community.CleanUpTasks.TellMe.View;
using Sdl.TellMe.ProviderApi;
using System.Drawing;
using System.Media;

namespace SDLCommunityCleanUpTasks.TellMe
{
    public class CleanUpTasksSettingsAction : AbstractTellMeAction
    {
        public CleanUpTasksSettingsAction()
        {
            Name = "CleanUpTasks Settings";
        }

        private static void ShowDialog()
        {
            SystemSounds.Beep.Play();
            new WarningSettingsView("https://appstore.rws.com/Plugin/23?tab=documentation").ShowDialog();
        }

        public override void Execute()
        {
            ShowDialog();
        }

        public override bool IsAvailable => true;
        public override string Category => "CleanUpTasks results";
        public override Icon Icon => PluginResources.TellMe_Settings;
    }
}
