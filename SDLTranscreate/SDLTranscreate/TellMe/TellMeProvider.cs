using Sdl.TellMe.ProviderApi;

namespace Trados.Transcreate.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format("{0} Tell Me provider", PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "trans", "create", "transcreate", "community", "support", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "trans", "create", "transcreate", "support", "forum" }
			},
			new CommunityAppStoreAction
			{
				Keywords = new[] { "trans", "create", "transcreate", "store", "download", "appstore" }}
		};
	}
}
