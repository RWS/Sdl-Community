using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.Studio.Time.Tracker.TellMe
{
    internal class NewActivityAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "new", "project", "activity" };
        private static readonly string _actionName = Constants.TellMe_NewActivityAction_Name;
        private static readonly Icon _icon = PluginResources.StudioTimeTrackeCreateProjectTaskAction_Icon;
        private static readonly bool _isAvailable = true;

        public NewActivityAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, customAction: ShowDialog) { }

        private static void ShowDialog()
        {
            SdlTradosStudio.Application.ExecuteAction<StudioTimeTrackeCreateProjectTaskAction>();
        }
    }
}
