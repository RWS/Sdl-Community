using InterpretBank.Studio.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace InterpretBank.Studio.TellMe
{
    [TellMeProvider]
    public class TellMeProvider : ITellMeProvider
    {
        public string Name => "DeepL Tell Me provider";

        public AbstractTellMeAction[] ProviderActions =>
        [
            new InterpretBankDocumentationAction(),
            new RwsCommunityAppStoreForumAction(),
            new SourceCodeAction(),
            new SettingsAction()
        ];
    }
}