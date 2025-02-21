using System.Drawing;
using System.Media;

namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
    internal class SettingsAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = ["settings"];
        private static readonly string _actionName = TellMeConstants.TellMe_Settings_Name;
        private static readonly Icon _icon = PluginResources.TellMe_Settings;
        private static readonly bool _isAvailable = true;

        public SettingsAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, customAction: ShowDialog) { }

        private static void ShowDialog()
        {
            SystemSounds.Beep.Play();
            ApplicationContext.ShowSettingsWindow();
        }
    }
}