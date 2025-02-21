using Sdl.Community.Jobs.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Jobs.TellMe
{
    [TellMeProvider]
    public class TellMeProvider : ITellMeProvider
    {
        public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

        public AbstractTellMeAction[] ProviderActions =>
        [
            new SourceCodeAction { Keywords = ["jobs", "trados jobs source code"] },
            new JobsAction { Keywords = ["jobs", "trados jobs view"] },
            new DocumentationAction { Keywords = ["jobs", "trados jobs", "community", "support", "documentation"] },
            new AppStoreForumAction { Keywords = ["jobs", "trados jobs", "support", "forum"] }
        ];
    }
}
