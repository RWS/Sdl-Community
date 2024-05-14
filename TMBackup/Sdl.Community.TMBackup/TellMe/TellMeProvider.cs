using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMBackup.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "tmbackup", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "tmbackup", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "tmbackup", "source", "code" }
			},
			new SettingsAction
			{
				Keywords = new[] { "tmbackup", "settings" }
			}
		};
	}
}
