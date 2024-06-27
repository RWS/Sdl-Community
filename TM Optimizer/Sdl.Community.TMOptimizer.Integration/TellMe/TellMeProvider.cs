using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMOptimizer.Integration.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "tm", "optimizer", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "tm", "optimizer", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "tm", "optimizer", "source", "code" }
			},
            new SettingsAction
            {
                Keywords = new[] { "tm", "optimizer", "settings" }
            }
        };
	}
}
