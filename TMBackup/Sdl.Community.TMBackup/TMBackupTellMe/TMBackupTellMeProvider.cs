using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMBackup.TMBackupTellMe
{
	[TellMeProvider]
	public class TMBackupTellMeProvider : ITellMeProvider
	{
		public string Name => "Trados TM Backup";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new SDLTMBackupSupportAction
			{
				Keywords = new []{ "trados tm backup", "trados tm backup community", "trados tm backup support" }
			},
			new SDLTMBackupHelpAction
			{
				Keywords = new []{ "trados tm backup", "trados tm backup help", "trados tm backup guide" }
			},
			new SDLTMBackupStoreAction
			{
				Keywords = new []{ "trados tm backup", "trados tm backup store", "trados tm backup download" }
			}
		};
	}
}