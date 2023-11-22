using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Studio.Time.Tracker.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "time", "tracker", "timetracker", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "time", "tracker", "timetracker", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "time", "tracker", "timetracker", "store", "download", "appstore" }}
		};
	}
}
