using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMBackup.TMBackupTellMe
{
	[TellMeProvider]
	public class TMBackupTellMeProvider : ITellMeProvider
	{
		public string Name => $"{PluginResources.Plugin_Name}";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new SDLTMBackupSupportAction
			{
				Keywords = new []{ "trados", "tm", "backup", "community", "support", "forum" }
			},
			new SDLTMBackupHelpAction
			{
				Keywords = new []{ "trados", "tm", "backup", "help", "community", "support", "wiki" }
			},
			new SDLTMBackupStoreAction
			{
				Keywords = new []{ "trados", "tm", "backup", "store", "download", "appstore" }
			}
		};
	}
}