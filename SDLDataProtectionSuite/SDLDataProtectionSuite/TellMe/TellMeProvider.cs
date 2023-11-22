using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "data", "protection", "suite", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "data", "protection", "suite", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "data", "protection", "suite", "store", "download", "appstore" }}
		};
	}
}
