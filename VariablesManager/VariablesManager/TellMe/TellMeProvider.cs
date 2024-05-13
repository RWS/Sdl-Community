using Sdl.TellMe.ProviderApi;

namespace VariablesManager.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "variables", "manager", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "variables", "manager", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "variables", "manager", "source", "code" }
			},
			new SettingsAction
			{
				Keywords = new[] { "variables", "manager", "settings" }
			}
		};
	}
}
