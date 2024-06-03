using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMLifting.Studio.TellMe
{
    [TellMeProvider]
    public class TellMeProvider : ITellMeProvider
    {
        public TellMeProvider()
        {
            ProviderActions = GetProviderActions();
        }

        public string Name => $"{TellMeConstants.TellMe_Provider_Name}";

        public AbstractTellMeAction[] ProviderActions { get; }

        private static AbstractTellMeAction[] GetProviderActions()
        {
            var forumAction = new CommunityForumAction();
            var helpAction = new DocumentationAction();
            var sourceCodeAction = new SourceCodeAction();
            var settingsAction = new SettingsAction();
            // ...

            var providerActions = new AbstractTellMeAction[] { forumAction, helpAction, sourceCodeAction, settingsAction };
            return providerActions;
        }
    }
}