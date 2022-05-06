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
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3134/trados-tmbackup");
		}
	}
}