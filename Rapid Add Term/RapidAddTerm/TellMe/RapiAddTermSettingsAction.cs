using Sdl.Community.RapidAddTerm.TellMe.View;
using Sdl.TellMe.ProviderApi;
using System.Drawing;
using System.Media;

namespace Sdl.Community.RapidAddTerm.TellMe
{
    public class RapiAddTermSettingsAction : AbstractTellMeAction
    {
        public RapiAddTermSettingsAction()
        {
            Name = "Rapid Add Term Settings";
        }

        private static void ShowDialog()
        {
            SystemSounds.Beep.Play();
            new WarningSettingsView("https://appstore.rws.com/Plugin/35?tab=documentation").ShowDialog();
        }

        public override void Execute()
        {
            ShowDialog();
        }

        public override bool IsAvailable => true;
        public override string Category => "Rapid Add Terms results";
        public override Icon Icon => PluginResources.TellMe_Settings;
    }
}
