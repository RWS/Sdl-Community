using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StudioViews.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => "Studio Views TellMe provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] {"studio", "views", "studioviews", "community", "support", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "studio", "views", "studioviews", "support", "forum" }
			},
			new CommunityAppStoreAction
			{
				Keywords = new[] { "studio", "views", "studioviews", "store", "download", "appstore" }}
		};
	}
}
