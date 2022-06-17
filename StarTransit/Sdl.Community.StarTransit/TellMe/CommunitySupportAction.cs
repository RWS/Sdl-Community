﻿using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StarTransit.TellMe
{
	public class CommunitySupportAction : AbstractTellMeAction
	{
		public CommunitySupportAction()
		{
			Name = "RWS Community AppStore forum";
		}
		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f/rws-appstore");
		}

		public override bool IsAvailable => true;
		public override string Category => "StarTransit results";

		public override Icon Icon => PluginResources.ForumIcon;
	}
}
