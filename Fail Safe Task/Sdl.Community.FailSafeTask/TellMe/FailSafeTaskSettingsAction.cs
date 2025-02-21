using Sdl.Community.FailSafeTask.View;
using Sdl.TellMe.ProviderApi;
using System.Drawing;
using System.Media;

namespace Sdl.Community.FailSafeTask.TellMe
{
    public class FailSafeTaskSettingsAction : AbstractTellMeAction
    {
        public FailSafeTaskSettingsAction()
        {
            Name = "Fail Safe Task Settings";
        }

        private static void ShowDialog()
        {
            SystemSounds.Beep.Play();
            new WarningSettingsView("https://appstore.rws.com/Plugin/28?tab=documentation").ShowDialog();
        }

        public override void Execute()
        {
            ShowDialog();
        }

        public override bool IsAvailable => true;
        public override string Category => string.Format("Fail Safe Task results", PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.TellMe_Settings;
    }
}

