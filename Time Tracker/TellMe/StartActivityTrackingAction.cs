using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.Community.Studio.Time.Tracker.Panels.Timers;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.Studio.Time.Tracker.TellMe
{
    internal class StartActivityTrackingAction : TellMeAction
    {
        private static readonly string[] HelpKeywords = { "start", "activity", "tracking" };
        private static readonly string ActionName = Constants.TellMe_StartActivityAction_Name;
        private static readonly Icon ActionIconIcon = PluginResources.StudioTimeTrackerStartTimer_Icon;
        private static readonly bool ActionIconIsAvailable = true;

        public StartActivityTrackingAction() : base(ActionName, ActionIconIcon, HelpKeywords, ActionIconIsAvailable, customAction: ShowDialog) { }

        private static void ShowDialog()
        {
            var controller = SdlTradosStudio.Application.GetController<StudioTimeTrackerViewController>();
            if (controller != null)
            {
                controller.Activate();
                var timeTrackerController = SdlTradosStudio.Application.GetController<StudioTimeTrackerViewTimerController>();
                if (timeTrackerController != null)
                {
                    timeTrackerController.Activate();

                    SdlTradosStudio.Application.ExecuteAction<StudioTimeTrackerStartTimer>();
                }
            }
        }

        public override bool IsAvailable
        {
            get
            {
                var controller = SdlTradosStudio.Application.GetController<StudioTimeTrackerViewController>();
                return controller?.ProjectActivityStartTrackerEnabled ?? false;
            }
        }
    }
}