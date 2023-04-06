using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ProjectTerms.Plugin.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "project", "term", "extract", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "project", "term", "extract", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "project", "term", "extract", "store", "download", "appstore" }}
		};
	}
}
