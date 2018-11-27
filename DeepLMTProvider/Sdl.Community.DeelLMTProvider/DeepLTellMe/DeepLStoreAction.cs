using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLStoreAction : AbstractTellMeAction
	{
		public DeepLStoreAction()
		{
			Name = "Download DeepL from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/deepl-translation-provider/847/");
		}

		public override bool IsAvailable => true;
		public override string Category => "DeepL results";
		public override Icon Icon => PluginResources.SDLAppStore;
	}
}
