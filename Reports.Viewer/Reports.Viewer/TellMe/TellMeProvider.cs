using Sdl.TellMe.ProviderApi;

namespace Reports.Viewer.Plus.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "reports", "viewer", "plus", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "reports", "viewer", "plus", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "reports", "viewer", "plus", "source", "code" }
			},
			new SettingsAction
			{
				Keywords = new[] { "reports", "viewer", "plus", "settings" }
			}
		};
	}
}
