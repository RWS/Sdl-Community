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
				Keywords = new[] { "sdltm", "repair", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "sdltm", "repair", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "sdltm", "repair", "store", "download", "appstore" }}
		};
	}
}
