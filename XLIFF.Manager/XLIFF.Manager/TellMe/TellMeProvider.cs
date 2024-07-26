using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.XLIFF.Manager.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "xliff", "manager", "xliffmanager", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "xliff", "manager", "xliffmanager", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "xliff", "manager", "xliffmanager", "store", "download", "appstore" }}
		};
	}
}
