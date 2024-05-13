using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Qualitivity.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "qualitivity", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "qualitivity", "support", "forum" }
			},
			new SettingsAction
			{
				Keywords = new[] { "qualitivity", "settings" }}
		};
	}
}
