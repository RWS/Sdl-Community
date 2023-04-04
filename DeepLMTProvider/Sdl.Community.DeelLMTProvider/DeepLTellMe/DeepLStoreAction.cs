using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLStoreAction : AbstractTellMeAction
    {
        public DeepLStoreAction()
        {
            Name = "Download DeepL from RWS AppStore";
        }

        public override string Category => "DeepL results";

        public override Icon Icon => PluginResources.Download;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://appstore.rws.com/Plugin/24");
        }
    }
}