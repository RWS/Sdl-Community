using Sdl.TellMe.ProviderApi;

namespace MicrosoftTranslatorProvider.Studio.TellMe
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
            var thirdPartyAction = new ThirdPartyRedirectAction();
            var forumAction = new CommunityForumAction();
            var helpAction = new DocumentationAction();
            var sourceCodeAction = new SourceCodeAction();
            var settingsAction = new SettingsAction();

            var providerActions = new AbstractTellMeAction[] { thirdPartyAction, forumAction, helpAction, sourceCodeAction, settingsAction };
            return providerActions;
        }
    }
}