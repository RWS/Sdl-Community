﻿using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Trados.TargetRenamer.TellMe
{
	public class HelpAction : AbstractTellMeAction
    {
        public HelpAction()
        {
            Name = $"{PluginResources.TargetRenamer_Name} wiki in the RWS Community";
        }

        public override string Category => $"{PluginResources.TargetRenamer_Name} results";

        public override Icon Icon => PluginResources.Question;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/5956/target-renamer");
        }
    }
}