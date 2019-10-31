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
	public class SDLTMBackupHelpAction : AbstractTellMeAction
	{
		public SDLTMBackupHelpAction()
		{
			Name = "SDLTMBackup wiki in the SDL Community";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3134/sdl-tmbackup");
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLTMBackup results";
		public override Icon Icon => PluginResources.ForumIcon;
	}
}
