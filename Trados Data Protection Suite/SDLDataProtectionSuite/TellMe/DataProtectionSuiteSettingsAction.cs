using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using System.Drawing;
using System.Media;

namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
    internal class DataProtectionSuiteSettingsAction()
        : TellMeAction(_actionName, _icon, _helpKeywords, _isAvailable, customAction: ShowDialog)
    {
        private static readonly string[] _helpKeywords = ["settings protect unprotect data protection suite trados"];
        private static readonly string _actionName = TellMeConstants.TellMe_Dps_Settings_Name;
        private static readonly Icon _icon = PluginResources.TellMe_Settings;
        private static readonly bool _isAvailable = true;

        private static void ShowDialog()
        {
            SystemSounds.Beep.Play();
            _ = new SettingsWarningView(TellMeConstants.TellMe_Documentation_Url).ShowDialog();

        }
    }
}