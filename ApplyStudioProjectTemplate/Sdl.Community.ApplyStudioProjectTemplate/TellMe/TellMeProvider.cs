using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "apply", "studio","project", "community", "support", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "apply", "studio", "project", "support", "forum" }
			},
			new CommunityAppStoreAction
			{
				Keywords = new[] { "apply", "studio", "project", "store", "download", "appstore" }
			}
		};
	}
}
