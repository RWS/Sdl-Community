using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMRepair.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "sdltm repaire", "repaire", "tm repaire", "tmrepaire", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "sdltm repaire", "repaire", "tm repaire", "tmrepaire", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "sdltm repaire", "repaire", "tm repaire", "tmrepaire", "source", "code" }
			},
			new SettingsAction
			{
				Keywords = new[] { "sdltm repaire", "repaire", "tm repaire", "tmrepaire", "settings" }
			}
		};
	}
}
