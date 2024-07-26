using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TuToTm.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "tutotm", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "tutotm", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "tutotm", "store", "download", "appstore" }
			}
		};
	}
}
