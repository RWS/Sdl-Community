using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;
using System.Media;

namespace Sdl.Community.Studio.Time.Tracker.TellMe
{
    internal class SettingsAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "project", "settings" };
        private static readonly string _actionName = Constants.TellMe_Settings_Name;
        private static readonly Icon _icon = PluginResources.TellMe_Settings;
        private static readonly bool _isAvailable = true;

        public SettingsAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, customAction: ShowDialog) { }

        private static void ShowDialog()
        {
            SdlTradosStudio.Application.ExecuteAction<StudioTimeTrackerConfiguration>();
        }
    }
}