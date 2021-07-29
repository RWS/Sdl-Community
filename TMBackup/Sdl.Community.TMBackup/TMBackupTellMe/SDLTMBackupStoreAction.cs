using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMBackup.TMBackupTellMe
{
	public class SDLTMBackupStoreAction : AbstractTellMeAction
	{
		public SDLTMBackupStoreAction()
		{
			Name = "Download Trados TM Backup from AppStore";
		}

		public override string Category => "Trados TM Backup results";

		public override Icon Icon => PluginResources.Download;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/trados-tmbackup/869/");
		}
	}
}