using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.Studio.Time.Tracker.TellMe
{
    internal class TimeTrackerAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "controller" };
        private static readonly string _actionName = Constants.TellMe_View_Name;
        private static readonly Icon _icon = PluginResources.StudioTimeTrackerApp_Icon;
        private static readonly bool _isAvailable = true;

        public TimeTrackerAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, customAction: ShowDialog) { }

        private static void ShowDialog()
        {
            SdlTradosStudio.Application.GetController<StudioTimeTrackerViewController>().Activate();
        }
    }
}