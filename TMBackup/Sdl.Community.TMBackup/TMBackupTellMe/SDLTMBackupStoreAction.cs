using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMBackup.TMBackupTellMe
{
	public class SDLTMBackupStoreAction : AbstractTellMeAction
	{
		public SDLTMBackupStoreAction()
		{
			Name = $"Download {PluginResources.Plugin_Name} from AppStore";
		}

		public override string Category => $"{PluginResources.Plugin_Name} results";

		public override Icon Icon => PluginResources.Download;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/71");
		}
	}
}