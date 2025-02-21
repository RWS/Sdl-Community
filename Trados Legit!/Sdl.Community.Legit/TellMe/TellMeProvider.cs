using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Legit.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "trados", "legit", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "trados", "legit", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "trados", "legit", "source", "code" }
			},
			new SettingsAction
			{
				Keywords = new[] { "trados", "legit", "settings" }
			}
		};
	}
}
