using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.XliffToLegacyConverter.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "sdlxliff", "legacy", "converter", "legacy converter", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "sdlxliff", "legacy", "converter", "legacy converter", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] {"sdlxliff", "legacy", "converter", "legacy converter", "store", "download", "appstore" }}
		};
	}
}
