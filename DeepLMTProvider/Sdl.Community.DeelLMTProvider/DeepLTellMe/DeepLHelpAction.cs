﻿using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLHelpAction : AbstractTellMeAction
    {
        public DeepLHelpAction()
        {
            Name = "DeepL wiki in the RWS Community";
        }

        public override string Category => "DeepL results";

        public override Icon Icon => PluginResources.Question;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3266.deepl-mt-provider");
        }
    }
}