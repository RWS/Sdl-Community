using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.YourProductivity
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "your", "productivity", "community", "support", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "your", "productivity", "support", "forum" }
			},
			new CommunityAppStoreAction
			{
				Keywords = new[] { "your", "productivity", "store", "download", "appstore" }
			}
		};
	}
}
