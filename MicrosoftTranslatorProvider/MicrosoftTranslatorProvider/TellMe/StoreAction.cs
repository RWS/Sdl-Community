using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace MicrosoftTranslatorProvider.TellMe
{
	public class StoreAction : AbstractTellMeAction
	{
		public StoreAction()
		{
			Name = "Download MT Enhanced Provider from the AppStore";
		}

		public override bool IsAvailable => true;

		public override string Category => "MT Enhanced Provider";

		public override Icon Icon => PluginResources.Download;

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/mt-enhanced-plugin-for-trados-studio/604/");
		}
	}
}