using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TMBackup.TMBackupTellMe
{
	[TellMeProvider]
	public class TMBackupTellMeProvider : ITellMeProvider
	{
		public string Name => "SDLTMBackup";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new SDLTMBackupSupportAction
			{
				Keywords = new []{ "sdltmbackup", "sdltmbackup community", "sdltmbackup support" }
			},		
			new SDLTMBackupHelpAction
			{
				Keywords = new []{ "sdltmbackup", "sdltmbackup help", "sdltmbackup guide" }
			},
			new SDLTMBackupStoreAction
			{
				Keywords = new []{ "sdltmbackup", "sdltmbackup store", "sdltmbackup download" }
			}		
		};
	}
}