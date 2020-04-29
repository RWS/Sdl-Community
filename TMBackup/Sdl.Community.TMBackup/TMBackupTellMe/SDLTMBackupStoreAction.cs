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
	public class SDLTMBackupStoreAction : AbstractTellMeAction
	{
		public SDLTMBackupStoreAction()
		{
			Name = "Download SDLTMBackup from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/sdl-tmbackup/869/");
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLMachineTranslationCloud results";
		public override Icon Icon => PluginResources.Download;
	}
}