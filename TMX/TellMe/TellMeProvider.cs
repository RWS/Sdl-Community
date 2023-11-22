using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FileType.TMX
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "tmx", "community", "support", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] {  "tmx", "support", "forum" }
			},
			new CommunityAppStoreAction
			{
				Keywords = new[] { "tmx", "store", "download", "appstore" }

			}
		};
	}
}
