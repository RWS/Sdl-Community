using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.AhkPlugin.TellMe
{
    [TellMeProvider]
    public class TellMeProvider : ITellMeProvider
    {
        public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

        public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
        {
            new DocumentationAction
            {
                Keywords = new[] { "autohotkey","manager","ahk", "documentation" }
            },
            new AppStoreForumAction
            {
                Keywords = new[] { "autohotkey","manager","ahk", "support", "forum" }
            },
            new SourceCodeAction
            {
                Keywords = new[] { "autohotkey","manager","ahk", "source", "code" }
            },
            new SettingsAction
            {
                Keywords = new[] { "autohotkey","manager","ahk", "settings" }
            }
        };
    }
}
