using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.Studio.Time.Tracker.TellMe
{
    internal class StartActivityTrackingAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "start", "activity", "tracking" };
        private static readonly string _actionName = Constants.TellMe_StartActivityAction_Name;
        private static readonly Icon _icon = PluginResources.StudioTimeTrackeStartTimer_Icon;
        private static readonly bool _isAvailable = true;

        public StartActivityTrackingAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, customAction: ShowDialog) { }

        private static void ShowDialog()
        {
            SdlTradosStudio.Application.ExecuteAction<StudioTimeTrackeStartTimer>();
        }
    }
}