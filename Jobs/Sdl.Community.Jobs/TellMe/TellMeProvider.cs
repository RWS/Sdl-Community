using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Jobs.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] {"jobs", "trados jobs", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "jobs", "trados jobs", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "jobs", "trados jobs", "store", "download", "appstore" }}
		};
	}
}
