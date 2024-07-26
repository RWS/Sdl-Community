using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMLifting.TranslationMemory.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "tm lifting", "tmlifting", "lifting", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "tm lifting", "tmlifting", "lifting", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "tm lifting", "tmlifting", "lifting", "store", "download", "appstore" }}
		};
	}
}
