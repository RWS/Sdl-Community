using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.Studio.Time.Tracker.TellMe
{
    internal class NewTrackerAction : TellMeAction
    {
        private static readonly string[] HelpKeywords = { "new", "time", "tracker", "project" };
        private static readonly string ActionName = Constants.TellMe_NewTrackerAction_Name;
        private static readonly Icon ActionIcon = PluginResources.StudioTimeTrackerCreateProjectAction_Icon;
        private static readonly bool ActionIsAvailable = true;

        public NewTrackerAction() : base(ActionName, ActionIcon, HelpKeywords, ActionIsAvailable, customAction: ShowDialog) { }

        private static void ShowDialog()
        {
            var controller = SdlTradosStudio.Application.GetController<StudioTimeTrackerViewController>();
            if (controller == null)
            {
                return;
            }

            if (controller.ProjectsController == null)
            {
                controller.Activate();
            }

            SdlTradosStudio.Application.ExecuteAction<CreateProject>();
        }

        public override bool IsAvailable
        {
            get
            {
                var controller = SdlTradosStudio.Application.GetController<StudioTimeTrackerViewController>();
                return controller?.TimeTrackerProjectNewEnabled ?? false;
            }
        }
    }
}
