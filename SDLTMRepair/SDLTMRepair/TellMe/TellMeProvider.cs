using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMRepair.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "repaire", "sdltm repair", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "repaire", "sdltm repair", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "repaire", "sdltm repair", "store", "download", "appstore" }}
		};
	}
}
