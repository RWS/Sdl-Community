using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Qualitivity.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "qualitivity", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "qualitivity", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "qualitivity", "store", "download", "appstore" }}
		};
	}
}
