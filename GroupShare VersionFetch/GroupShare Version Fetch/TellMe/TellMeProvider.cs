using Sdl.Community.GSVersionFetch;
using Sdl.TellMe.ProviderApi;
using System;
using Sdl.Community.GSVersionFetch.TellMe.Actions;

namespace Sdl.Community.GSVersionFetch.TellMe
{
    [TellMeProvider]
    public class TellMeProvider : ITellMeProvider
    {
        public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

        public AbstractTellMeAction[] ProviderActions =>
        [
            new DocumentationAction
            {
                Keywords = ["gs", "groupshare", "version", "fetch", "community", "support", "documentation docs"]
            },
            new AppStoreForumAction
            {
                Keywords = ["gs", "groupshare", "version", "fetch", "support", "forum"]
            },
            new GroupshareVersionFetchAction
            {
                Keywords = ["gs", "groupshare", "version", "fetch"]
            }
        ];
    }
}
