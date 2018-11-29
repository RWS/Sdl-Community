using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.MtEnhancedProvider.MTEnhancedTellMe
{
	public class MTEnhancedStoreAction : AbstractTellMeAction
	{
		public MTEnhancedStoreAction()
		{
			Name = "Download MTEnhanced Provider from the AppStore";
		}

		public override void Execute()
		{
			Process.Start(
				"https://appstore.sdl.com/language/app/mt-enhanced-plugin-for-trados-studio/604/");
		}

		public override bool IsAvailable => true;

		public override string Category => "MT Enhanced Provider";

		public override Icon Icon => PluginResources.AppStoreIcon;
	}
}
