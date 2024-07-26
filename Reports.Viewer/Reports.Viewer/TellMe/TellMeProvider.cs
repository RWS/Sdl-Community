using Sdl.TellMe.ProviderApi;

namespace Reports.Viewer.Plus.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "reports", "viewer", "plus", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "reports", "viewer", "plus", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "reports", "viewer", "plus", "store", "download", "appstore" }}
		};
	}
}
