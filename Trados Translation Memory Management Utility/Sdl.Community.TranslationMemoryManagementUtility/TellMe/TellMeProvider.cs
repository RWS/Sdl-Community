using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TranslationMemoryManagementUtility
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_String_TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
            new DocumentationAction
            {
                Keywords = new[] { "translation", "memory", "management", "utility", "documentation" }

            },
			new CommunityAppStoreForumAction
			{
				Keywords = new[] { "translation", "memory", "management", "utility", "support", "forum" }
			},
            new SourceCodeAction
            {
                Keywords = new[] { "translation", "memory", "management", "utility", "source", "code" }
            },
            new SettingsAction
            {
                Keywords = new[] { "translation", "memory", "management", "utility", "settings" }

            }
        };
	}
}
