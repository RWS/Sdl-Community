using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMBackup.TMBackupTellMe
{
	public class SDLTMBackupHelpAction : AbstractTellMeAction
	{
		public SDLTMBackupHelpAction()
		{
			Name = "Trados TM Backup wiki in the RWS Community";
		}

		public override string Category => "Trados TM Backup results";

		public override Icon Icon => PluginResources.Question;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/3134/trados-tmbackup");
		}
	}
}