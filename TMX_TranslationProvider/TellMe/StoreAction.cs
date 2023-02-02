using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace TMX_TranslationProvider.Tellme
{
	public class TmxProviderStoreAction : AbstractTellMeAction
    {
        public TmxProviderStoreAction()
        {
            Name = "Download Tmx Provider from RWS AppStore";
        }

        public override string Category => "TmxProvider results";

        public override Icon Icon => PluginResources.Download;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://appstore.sdl.com/language/app/TmxProvider-translation-provider/847/");
        }
    }
}