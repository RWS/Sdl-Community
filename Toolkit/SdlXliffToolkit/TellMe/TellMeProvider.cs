using Sdl.TellMe.ProviderApi;

namespace SdlXliffToolkit.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "sdlxliff", "toolkit", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "sdlxliff", "toolkit", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "sdlxliff", "toolkit", "store", "download", "appstore" }}
		};
	}
}
