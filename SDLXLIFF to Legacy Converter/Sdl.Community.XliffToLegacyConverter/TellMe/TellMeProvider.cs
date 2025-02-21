using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.XliffToLegacyConverter.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "sdlxliff", "legacy", "converter", "legacy converter", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "sdlxliff", "legacy", "converter", "legacy converter", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] {"sdlxliff", "legacy", "converter", "legacy converter", "source", "code" }
			},
            new SettingsAction
            {
                Keywords = new[] {"sdlxliff", "legacy", "converter", "legacy converter", "settings" }
            }
        };
	}
}
