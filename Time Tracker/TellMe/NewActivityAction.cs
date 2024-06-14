using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.Studio.Time.Tracker.TellMe
{
    internal class NewActivityAction : TellMeAction
    {
        private static readonly string[] HelpKeywords = { "new", "project", "activity" };
        private static readonly string ActionName = Constants.TellMe_NewActivityAction_Name;
        private static readonly Icon ActionIcon = PluginResources.StudioTimeTrackerCreateProjectTaskAction_Icon;
        private static readonly bool ActionIsAvailable = true;

        public NewActivityAction() : base(ActionName, ActionIcon, HelpKeywords, ActionIsAvailable, customAction: ShowDialog) { }

        private static void ShowDialog()
        {
            var controller = SdlTradosStudio.Application.GetController<StudioTimeTrackerViewController>();
            if (controller != null)
            {
                controller.Activate();
                SdlTradosStudio.Application.ExecuteAction<StudioTimeTrackerCreateProjectTaskAction>();
            }
        }

        public override bool IsAvailable
        {
            get
            {
                var controller = SdlTradosStudio.Application.GetController<StudioTimeTrackerViewController>();
                return controller?.ProjectActivityNewEnabled ?? false;
            }
        }
    }
}
