﻿using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ApplyTMTemplate.TellMe
{
	public class ApplyTMTemplateStoreAction : AbstractTellMeAction
	{
		public ApplyTMTemplateStoreAction()
		{
			Name = "Download Apply TM Template from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/21");
		}

		public override bool IsAvailable => true;

		public override string Category => "Apply TM Template results";

		public override Icon Icon => PluginResources.Download;
	}
}