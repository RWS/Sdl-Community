using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TranslationMemoryManagementUtility
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunityWikiAction
			{
				Keywords = new[] { "translation", "memory", "management", "utility", "translation memory management utility", "community", "support", "wiki" }
			},
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "translation", "memory", "management", "utility", "translation memory management utility", "support", "forum" }
			},
			new CommunityAppStoreAction
			{
				Keywords = new[] { "translation", "memory", "management", "utility", "translation memory management utility", "store", "download", "appstore" }

			}
		};
	}
}
