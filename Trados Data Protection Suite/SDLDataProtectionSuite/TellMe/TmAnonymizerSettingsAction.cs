using System.Drawing;
using System.Media;

namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
    internal class TmAnonymizerSettingsAction()
        : TellMeAction(_actionName, _icon, _helpKeywords, _isAvailable, customAction: ShowDialog)
    {
        private static readonly string[] _helpKeywords = ["settings tm anonymizer tmanonymizer"];
        private static readonly string _actionName = TellMeConstants.TellMe_Settings_Name;
        private static readonly Icon _icon = PluginResources.TellMe_Settings;
        private static readonly bool _isAvailable = true;

        private static void ShowDialog()
        {
            SystemSounds.Beep.Play();
            ApplicationContext.ShowSettingsWindow();
        }
    }
}