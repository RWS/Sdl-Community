using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ProjectTerms.Plugin.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityDocumentationAction
			{
				Keywords = new[] { "project", "term", "extract", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "project", "term", "extract", "support", "forum" }
			},
			new AppStoreSourceCodeAction
			{
				Keywords = new[] { "project", "term", "extract", "source", "code" }
			},
            new SettingsAction
            {
                Keywords = new[] { "project", "term", "extract", "settings" }
            }
        };
	}
}
