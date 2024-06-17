using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.PostEdit.Versions.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] {"post", "eidt", "post-edit", "compare", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "post", "eidt", "post-edit", "compare", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "post", "eidt", "post-edit", "compare", "source", "code" }
			},
			new SettingsAction
			{
				Keywords = new[] { "post", "eidt", "post-edit", "compare", "settings" }
			},
            new PluginViewAction
            {
                Keywords = new[] { "post", "eidt", "post-edit", "compare", "view", "controller" }
            },
            new NewProjectVersionAction
            {
                Keywords = new[] { "post", "eidt", "post-edit", "compare", "new", "project", "version" }
            }
        };
	}
}
