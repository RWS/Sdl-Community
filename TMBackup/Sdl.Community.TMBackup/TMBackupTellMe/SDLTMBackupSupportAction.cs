using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMBackup.TMBackupTellMe
{
	public class SDLTMBackupSupportAction : AbstractTellMeAction
	{
		public SDLTMBackupSupportAction()
		{
			Name = "RWS Community AppStore forum";
		}

		public override string Category => $"{PluginResources.Plugin_Name} results";

		public override Icon Icon => PluginResources.ForumIcon;

		public override bool IsAvailable => true;

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/");
		}
	}
}