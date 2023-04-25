using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.WordCloud.Plugin
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "trados","studio" ,"word", "cloud", "community", "support", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "trados", "studio", "word", "cloud", "support", "forum" }
			},
			new CommunityAppStoreAction
			{
				Keywords = new[] { "trados", "studio", "word", "cloud", "store", "download", "appstore" }
			}
		};
	}
}
