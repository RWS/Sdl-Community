using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace TMX_TranslationProvider.Tellme
{
	public class TmxProviderHelpAction : AbstractTellMeAction
    {
        public TmxProviderHelpAction()
        {
            Name = "TmxProvider wiki in the RWS Community";
        }

        public override string Category => "TmxProvider results";

        public override Icon Icon => PluginResources.Question;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/6587/tmx-translation-provider");
        }
    }
}