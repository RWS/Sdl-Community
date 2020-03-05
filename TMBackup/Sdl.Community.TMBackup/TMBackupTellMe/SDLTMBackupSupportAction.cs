using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMBackup.TMBackupTellMe
{
	public class SDLTMBackupSupportAction : AbstractTellMeAction
	{
		public SDLTMBackupSupportAction()
		{
			Name = "SDL Community AppStore forum";
		}
		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/f/160");
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLTMBackup results";

		public override Icon Icon => PluginResources.ForumIcon;
	}
}