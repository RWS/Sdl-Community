using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FileTypeSupport.TXML
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "wordfast", "txml", "community", "support", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "wordfast", "txml", "support", "forum" }
			},
			new CommunityAppStoreAction
			{
				Keywords = new[] { "wordfast", "txml", "store", "download", "appstore" }

			}
		};
	}
}
