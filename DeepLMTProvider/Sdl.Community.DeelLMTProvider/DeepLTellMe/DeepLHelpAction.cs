using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLHelpAction : AbstractTellMeAction
    {
        public DeepLHelpAction()
        {
            Name = $"{PluginResources.Plugin_Name} Documentation";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";

        public override Icon Icon => PluginResources.TellmeDocumentation;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/3266/deepl-translation-provider");
        }
    }
}