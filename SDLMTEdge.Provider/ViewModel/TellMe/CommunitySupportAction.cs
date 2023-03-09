﻿using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeTellMe
{
	public class CommunitySupportAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
        public override string Category => "Language Weaver Edge results";
        public override Icon Icon => PluginResources.Question;

		public CommunitySupportAction()
		{
			Name = "RWS Community AppStore forum";
		}

		public override void Execute()
		{
			Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3450.ets-mt-provider");
		}
	}
}