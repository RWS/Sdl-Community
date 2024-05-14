using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLContactAction : AbstractTellMeAction
    {
        public DeepLContactAction()
        {
            Name = "Contact DeepL";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";

        public override Icon Icon => PluginResources.deepLIcon;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://www.deepl.com/pro.html");
        }
    }
}