using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace TMX_TranslationProvider.Tellme
{
	public class TmxProviderContactAction : AbstractTellMeAction
    {
        public TmxProviderContactAction()
        {
            Name = "Contact TmxProvider";
        }

        public override string Category => "TmxProvider results";

        public override Icon Icon => PluginResources.TMX_TM_Provider;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://www.TmxProvider.com/pro.html");
        }
    }
}