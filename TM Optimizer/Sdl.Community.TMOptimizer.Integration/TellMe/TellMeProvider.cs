using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMOptimizer.Integration.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "tm", "optimizer", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "tm", "optimizer", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "tm", "optimizer", "store", "download", "appstore" }}
		};
	}
}
