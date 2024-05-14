using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TuToTm.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "tutotm", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "tutotm", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "tutotm", "source", "code" }
			},
			new SettingsAction
			{
				Keywords = new[] { "tutotm", "settings" }
			}
		};
	}
}
