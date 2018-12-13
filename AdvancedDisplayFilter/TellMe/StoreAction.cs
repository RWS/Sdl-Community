using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.TellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = "Download Advanced Display Filter";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/community-advanced-display-filter/849/");
		}

		public override bool IsAvailable => true;
		public override string Category => "Community Advanced Display Filter results";
		public override Icon Icon => PluginResources.Download;
	}
}
