using Sdl.TellMe.ProviderApi;

namespace VariablesManager
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "variables", "manager", "community", "support", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "variables", "manager", "support", "forum" }
			},
			new CommunityAppStoreAction
			{
				Keywords = new[] { "variables", "manager", "store", "download", "appstore" }

			}
		};
	}
}
