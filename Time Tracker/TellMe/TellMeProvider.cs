using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Studio.Time.Tracker.TellMe
{
    [TellMeProvider]
    public class TellMeProvider : ITellMeProvider
    {
        public TellMeProvider()
        {
            ProviderActions = GetProviderActions();
        }

        public string Name => $"{Constants.TellMe_Provider_Name}";

        public AbstractTellMeAction[] ProviderActions { get; }

        private AbstractTellMeAction[] GetProviderActions()
        {
            new StudioTimeTrackerViewController().Initialize();
            var forumAction = new CommunityForumAction();
            var helpAction = new DocumentationAction();
            var sourceCodeAction = new SourceCodeAction();
            var settingsAction = new SettingsAction();
            var timeTrackerAction = new TimeTrackerAction();
            //var newTrackerAction = new NewTrackerAction();
            //var newActivityAction = new NewActivityAction();
            //var startActivityAction = new StartActivityTrackingAction();
            //var stopActivityAction = new StopActivityTrackingAction();

            var providerActions = new AbstractTellMeAction[] { forumAction, helpAction, sourceCodeAction, settingsAction, timeTrackerAction };
            return providerActions;
        }
    }
}