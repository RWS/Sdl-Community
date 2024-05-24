using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.Studio.Time.Tracker.TellMe
{
    internal class NewTrackerAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "New", "Time", "Tracker", "Project" };
        private static readonly string _actionName = Constants.TellMe_NewTrackerAction_Name;
        private static readonly Icon _icon = PluginResources.StudioTimeTrackeCreateProjectAction_Icon;
        private static readonly bool _isAvailable = true;

        public NewTrackerAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, customAction: ShowDialog) { }

        private static void ShowDialog()
        {
            SdlTradosStudio.Application.ExecuteAction<CreateProject>();
        }
    }
}