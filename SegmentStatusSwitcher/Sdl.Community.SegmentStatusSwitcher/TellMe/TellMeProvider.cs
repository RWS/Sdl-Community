using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SegmentStatusSwitcher.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "segment", "status", "switcher", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "segment", "status", "switcher", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "segment", "status", "switcher", "store", "download", "appstore" }}
		};
	}
}
