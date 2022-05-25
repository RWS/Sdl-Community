﻿using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExportAnalysisReports.TellMe
{
	public class CommunitySupportAction : AbstractTellMeAction
	{
		public CommunitySupportAction()
		{
			Name = "RWS Community AppStore forum";
		}
		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/f/160");
		}

		public override bool IsAvailable => true;
		public override string Category => "Trados Export Analysis Reports results";

		public override Icon Icon => PluginResources.ForumIcon;
	}
}