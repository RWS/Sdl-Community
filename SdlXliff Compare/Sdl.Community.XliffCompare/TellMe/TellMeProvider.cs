using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.XliffCompare.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "sdlxliff compare", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "sdlxliff compare", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "sdlxliff compare", "store", "download", "appstore" }}
		};
	}
}
